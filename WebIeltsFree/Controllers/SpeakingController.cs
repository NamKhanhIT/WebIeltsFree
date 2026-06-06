using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using WebIeltsFree.Models;
using WebIeltsFree.Services;
using WebIeltsFree.Middleware;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WebIeltsFree.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SpeakingController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPythonAiService _pythonAi;
    private readonly IConfiguration _configuration;
    private readonly IGeminiService _gemini;

    public SpeakingController(AppDbContext context, IPythonAiService pythonAi, IConfiguration configuration, IGeminiService gemini)
    {
        _context = context;
        _pythonAi = pythonAi;
        _configuration = configuration;
        _gemini = gemini;
    }

    /// <summary>
    /// Get secure D-ID token/API key
    /// </summary>
    [HttpGet("did-token")]
    public ActionResult<object> GetDidToken()
    {
        var apiKey = _configuration["DidSettings:ApiKey"] ?? Environment.GetEnvironmentVariable("DID_API_KEY");
        if (string.IsNullOrEmpty(apiKey) || apiKey.StartsWith("${"))
        {
            apiKey = Environment.GetEnvironmentVariable("DID_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                return NotFound(ApiResponse<object>.Fail("D-ID API configuration is missing on the server."));
            }
        }
        return Ok(new { token = apiKey });
    }

    /// <summary>
    /// Get speaking topics
    /// </summary>
    [HttpGet("topics")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<SpeakingTopicDto>>>> GetTopics([FromQuery] int? part = null)
    {
        var query = _context.SpeakingTopics
            .Where(t => t.IsActive);

        if (part is >= 1 and <= 3)
        {
            query = query.Where(t => t.PartNumber == part.Value);
        }

        var topics = await query
            .OrderBy(t => t.PartNumber)
            .ThenBy(t => t.TargetBand)
            .Select(t => new SpeakingTopicDto
            {
                TopicId = t.TopicId,
                Topic = t.TopicName,
                Category = $"Part {t.PartNumber}",
                DifficultyLevel = (int)t.TargetBand
            })
            .ToListAsync();

        return Ok(ApiResponse<List<SpeakingTopicDto>>.Ok(topics));
    }

    /// <summary>
    /// Start a speaking practice session
    /// </summary>
    [HttpPost("start")]
    public async Task<ActionResult<ApiResponse<StartSpeakingSessionResponse>>> StartSession([FromBody] StartSpeakingSessionRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<StartSpeakingSessionResponse>.Fail("Please log in"));

        // Sanitize topic input
        var sanitizedTopic = InputSanitizer.StripHtmlTags(request.Topic) ?? "Free speaking practice";
        sanitizedTopic = InputSanitizer.Truncate(sanitizedTopic, 500);

        var session = new SpeakingSession
        {
            UserId = userId,
            Topic = sanitizedTopic,
            CreatedAt = DateTime.UtcNow
        };

        _context.SpeakingSessions.Add(session);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<StartSpeakingSessionResponse>.Ok(new StartSpeakingSessionResponse
        {
            SessionId = session.SessionId,
            Topic = session.Topic,
            Instructions = GetSpeakingInstructions(session.Topic)
        }));
    }

    /// <summary>
    /// Submit audio recording for AI evaluation
    /// </summary>
    [HttpPost("submit-audio")]
    public async Task<ActionResult<ApiResponse<SpeakingResultDto>>> SubmitAudio([FromBody] SubmitSpeakingRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<SpeakingResultDto>.Fail("Please log in"));

        var session = await _context.SpeakingSessions.FindAsync(request.SessionId);
        if (session == null || session.UserId != userId)
            return NotFound(ApiResponse<SpeakingResultDto>.Fail("Session not found"));

        // Sanitize and use transcript
        var transcript = InputSanitizer.StripHtmlTags(request.Transcript) ?? "Transcription would appear here after processing.";
        transcript = InputSanitizer.Truncate(transcript, 5000);
        
        WebIeltsFree.Services.SpeakingEvaluation aiEvaluation;
        try
        {
            aiEvaluation = await _pythonAi.EvaluateSpeakingAsync(transcript, session.Topic ?? "General topic");
        }
        catch
        {
            // Call the highly detailed Gemini evaluation for a professional serious grade
            var geminiEvaluation = await EvaluateSpeakingWithGeminiAsync(transcript, session.Topic ?? "General topic");
            if (geminiEvaluation != null)
            {
                aiEvaluation = geminiEvaluation;
            }
            else
            {
                var fallback = SimulateAIEvaluation();
                aiEvaluation = new WebIeltsFree.Services.SpeakingEvaluation
                {
                    OverallBand = Math.Clamp((fallback.Fluency + fallback.Pronunciation + fallback.Grammar) / 3f, 0f, 9f),
                    Fluency = fallback.Fluency,
                    Pronunciation = fallback.Pronunciation,
                    LexicalResource = Math.Clamp(fallback.Grammar + 0.3f, 0f, 9f),
                    GrammaticalRange = fallback.Grammar,
                    Feedback = fallback.Feedback,
                    Strengths = new List<string> { "Clear effort to communicate", "Topic relevance maintained" },
                    Improvements = new List<string> { "Increase response depth", "Improve pronunciation clarity" }
                };
            }
        }

        session.FluencyScore = aiEvaluation.Fluency;
        session.PronunciationScore = aiEvaluation.Pronunciation;
        session.GrammarScore = aiEvaluation.GrammaticalRange;
        session.AudioUrl = $"audio/{session.SessionId}.webm";
        session.Transcript = transcript;
        session.AiFeedback = aiEvaluation.Feedback + "\n\nStrengths: " + string.Join(", ", aiEvaluation.Strengths) +
                            "\n\nAreas for Improvement: " + string.Join(", ", aiEvaluation.Improvements);

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<SpeakingResultDto>.Ok(new SpeakingResultDto
        {
            SessionId = session.SessionId,
            Topic = session.Topic,
            FluencyScore = session.FluencyScore,
            PronunciationScore = session.PronunciationScore,
            GrammarScore = session.GrammarScore,
            OverallBand = aiEvaluation.OverallBand,
            Transcript = session.Transcript,
            AiFeedback = session.AiFeedback,
            CreatedAt = session.CreatedAt
        }));
    }

    private async Task<WebIeltsFree.Services.SpeakingEvaluation> EvaluateSpeakingWithGeminiAsync(string transcript, string topic)
    {
        try
        {
            var prompt = $$"""
            You are an expert IELTS Speaking examiner.
            Seriously evaluate the following student response/transcript for the given topic.
            
            Topic:
            {{topic}}
            
            Student Transcript:
            {{transcript}}
            
            Evaluate according to the four official IELTS Speaking criteria:
            1. Fluency and Coherence (fluency)
            2. Lexical Resource (lexicalResource)
            3. Grammatical Range and Accuracy (grammaticalRange)
            4. Pronunciation (pronunciation)
            
            Determine the band score (1.0 to 9.0, rounded to the nearest 0.5) for each criterion and calculate the overall band score.
            Generate a detailed feedback paragraph, 2-3 specific strengths, and 2-3 specific areas for improvement.
            
            Response format: Return ONLY a valid JSON object matching the structure below (do not include markdown wrapping or other text):
            {
              "overallBand": 6.5,
              "fluency": 6.5,
              "pronunciation": 6.0,
              "lexicalResource": 7.0,
              "grammaticalRange": 6.0,
              "feedback": "Your response is clear and directly addresses the prompt. However, you can improve by using more complex vocabulary...",
              "strengths": ["Good topic relevance", "Clear pronunciation of key vocabulary"],
              "improvements": ["Try to reduce pauses between ideas", "Use a wider range of cohesive devices"]
            }
            """;

            var jsonResponse = await _gemini.GenerateContentAsync(prompt, "Output JSON format only.");
            
            // Basic regex parsing to ensure fallback safety if JSON is not perfectly formatted
            var matchOverall = System.Text.RegularExpressions.Regex.Match(jsonResponse, @"""overallBand""\s*:\s*([0-9.]+)");
            var matchFluency = System.Text.RegularExpressions.Regex.Match(jsonResponse, @"""fluency""\s*:\s*([0-9.]+)");
            var matchPron = System.Text.RegularExpressions.Regex.Match(jsonResponse, @"""pronunciation""\s*:\s*([0-9.]+)");
            var matchLex = System.Text.RegularExpressions.Regex.Match(jsonResponse, @"""lexicalResource""\s*:\s*([0-9.]+)");
            var matchGram = System.Text.RegularExpressions.Regex.Match(jsonResponse, @"""grammaticalRange""\s*:\s*([0-9.]+)");
            var matchFeedback = System.Text.RegularExpressions.Regex.Match(jsonResponse, @"""feedback""\s*:\s*""([^""]+)""");

            float overall = matchOverall.Success ? float.Parse(matchOverall.Groups[1].Value) : 6.0f;
            float fluency = matchFluency.Success ? float.Parse(matchFluency.Groups[1].Value) : 6.0f;
            float pron = matchPron.Success ? float.Parse(matchPron.Groups[1].Value) : 6.0f;
            float lexical = matchLex.Success ? float.Parse(matchLex.Groups[1].Value) : 6.0f;
            float grammar = matchGram.Success ? float.Parse(matchGram.Groups[1].Value) : 6.0f;
            string feedback = matchFeedback.Success ? matchFeedback.Groups[1].Value : "Good effort. Keep practicing to build confidence and fluency.";

            // Attempt fully typed deserialization
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;
                
                if (root.TryGetProperty("overallBand", out var pBand)) overall = (float)pBand.GetDouble();
                if (root.TryGetProperty("fluency", out var pFlu)) fluency = (float)pFlu.GetDouble();
                if (root.TryGetProperty("pronunciation", out var pPro)) pron = (float)pPro.GetDouble();
                if (root.TryGetProperty("lexicalResource", out var pLex)) lexical = (float)pLex.GetDouble();
                if (root.TryGetProperty("grammaticalRange", out var pGra)) grammar = (float)pGra.GetDouble();
                if (root.TryGetProperty("feedback", out var pFeed)) feedback = pFeed.GetString() ?? feedback;
                
                var strengths = new List<string>();
                if (root.TryGetProperty("strengths", out var pStren) && pStren.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in pStren.EnumerateArray())
                    {
                        strengths.Add(item.GetString() ?? "");
                    }
                }
                
                var improvements = new List<string>();
                if (root.TryGetProperty("improvements", out var pImprov) && pImprov.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in pImprov.EnumerateArray())
                    {
                        improvements.Add(item.GetString() ?? "");
                    }
                }

                return new WebIeltsFree.Services.SpeakingEvaluation
                {
                    OverallBand = RoundToIeltsBand(overall),
                    Fluency = RoundToIeltsBand(fluency),
                    Pronunciation = RoundToIeltsBand(pron),
                    LexicalResource = RoundToIeltsBand(lexical),
                    GrammaticalRange = RoundToIeltsBand(grammar),
                    Feedback = feedback,
                    Strengths = strengths.Count > 0 ? strengths : new List<string> { "Relevant ideas presented", "Natural speaking flow" },
                    Improvements = improvements.Count > 0 ? improvements : new List<string> { "Expand vocabulary", "Enhance grammar precision" }
                };
            }
            catch
            {
                // Fallback to parsed parameters
                return new WebIeltsFree.Services.SpeakingEvaluation
                {
                    OverallBand = RoundToIeltsBand(overall),
                    Fluency = RoundToIeltsBand(fluency),
                    Pronunciation = RoundToIeltsBand(pron),
                    LexicalResource = RoundToIeltsBand(lexical),
                    GrammaticalRange = RoundToIeltsBand(grammar),
                    Feedback = feedback,
                    Strengths = new List<string> { "Relevant ideas presented", "Natural speaking flow" },
                    Improvements = new List<string> { "Expand vocabulary", "Enhance grammar precision" }
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Speaking Gemini fallback failed: " + ex.Message);
            return null!;
        }
    }

    private static float RoundToIeltsBand(float band)
    {
        return (float)Math.Round(band * 2, MidpointRounding.AwayFromZero) / 2f;
    }

    /// <summary>
    /// Get speaking session history
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<List<SpeakingResultDto>>>> GetHistory()
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<List<SpeakingResultDto>>.Fail("Please log in"));

        var history = await _context.SpeakingSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Take(20)
            .Select(s => new SpeakingResultDto
            {
                SessionId = s.SessionId,
                Topic = s.Topic,
                FluencyScore = s.FluencyScore,
                PronunciationScore = s.PronunciationScore,
                GrammarScore = s.GrammarScore,
                OverallBand = (s.FluencyScore + s.PronunciationScore + s.GrammarScore) / 3,
                Transcript = s.Transcript,
                AiFeedback = s.AiFeedback,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<SpeakingResultDto>>.Ok(history));
    }

    private async Task<HttpResponseMessage> CallDidApiAsync(HttpMethod method, string path, string jsonContent = null)
    {
        var apiKey = _configuration["DidSettings:ApiKey"] ?? Environment.GetEnvironmentVariable("DID_API_KEY");
        if (string.IsNullOrEmpty(apiKey) || apiKey.StartsWith("${"))
        {
            apiKey = Environment.GetEnvironmentVariable("DID_API_KEY");
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("D-ID API key is not configured.");
        }

        string authHeaderValue;
        if (apiKey.Contains(":"))
        {
            var parts = apiKey.Split(':');
            string email;
            try
            {
                // Decode base64 email if it is encoded
                var base64Str = parts[0];
                // Add padding if missing
                if (base64Str.Length % 4 != 0)
                {
                    base64Str = base64Str.PadRight(base64Str.Length + (4 - base64Str.Length % 4), '=');
                }
                var emailBytes = Convert.FromBase64String(base64Str);
                email = Encoding.UTF8.GetString(emailBytes);
            }
            catch
            {
                email = parts[0];
            }
            var rawCredentials = $"{email}:{parts[1]}";
            authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawCredentials));
        }
        else
        {
            authHeaderValue = apiKey;
        }

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
        
        var url = $"https://api.d-id.com/{path.TrimStart('/')}";
        HttpRequestMessage request = new HttpRequestMessage(method, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (jsonContent != null)
        {
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }

        return await client.SendAsync(request);
    }

    [HttpPost("did-stream/start")]
    public async Task<IActionResult> StartDidStream([FromBody] JsonElement body)
    {
        try
        {
            var jsonStr = body.ToString();
            var response = await CallDidApiAsync(HttpMethod.Post, "talks/streams", jsonStr);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, content);
            }
            
            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Error($"Failed to proxy D-ID stream: {ex.Message}"));
        }
    }

    /// <summary>
    /// Proxy call to submit D-ID ICE candidate
    /// </summary>
    [HttpPost("did-stream/{streamId}/ice")]
    public async Task<IActionResult> SubmitDidIce(string streamId, [FromBody] JsonElement body)
    {
        try
        {
            var jsonStr = body.ToString();
            var response = await CallDidApiAsync(HttpMethod.Post, $"talks/streams/{streamId}/ice", jsonStr);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, content);
            }
            
            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Error($"Failed to proxy D-ID ICE candidate: {ex.Message}"));
        }
    }

    /// <summary>
    /// Proxy call to submit D-ID SDP answer
    /// </summary>
    [HttpPost("did-stream/{streamId}/sdp")]
    public async Task<IActionResult> SubmitDidSdp(string streamId, [FromBody] JsonElement body)
    {
        try
        {
            var jsonStr = body.ToString();
            var response = await CallDidApiAsync(HttpMethod.Post, $"talks/streams/{streamId}/sdp", jsonStr);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, content);
            }
            
            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Error($"Failed to proxy D-ID SDP answer: {ex.Message}"));
        }
    }

    /// <summary>
    /// Proxy call to submit D-ID talk request
    /// </summary>
    [HttpPost("did-stream/{streamId}/talk")]
    public async Task<IActionResult> SubmitDidTalk(string streamId, [FromBody] JsonElement body)
    {
        try
        {
            var jsonStr = body.ToString();
            var response = await CallDidApiAsync(HttpMethod.Post, $"talks/streams/{streamId}", jsonStr);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, content);
            }
            
            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Error($"Failed to proxy D-ID talk request: {ex.Message}"));
        }
    }

    /// <summary>
    /// Proxy call to close or stop D-ID streaming session
    /// </summary>
    [HttpDelete("did-stream/{streamId}")]
    public async Task<IActionResult> StopDidStream(string streamId, [FromBody] JsonElement? body = null)
    {
        try
        {
            var jsonStr = body?.ToString();
            var response = await CallDidApiAsync(HttpMethod.Delete, $"talks/streams/{streamId}", jsonStr);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, content);
            }
            
            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Error($"Failed to proxy D-ID stream stop: {ex.Message}"));
        }
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) return 0;
        return int.TryParse(claim.Value, out var userId) ? userId : 0;
    }

    private static string GetSpeakingInstructions(string topic)
    {
        if (topic.StartsWith("Describe"))
        {
            return "You have 1 minute to prepare. Then speak for 1-2 minutes about the topic. " +
                   "Try to cover: what it is, why it's important to you, and how it affected you.";
        }
        if (topic.StartsWith("Talk about"))
        {
            return "This is Part 1. Answer naturally for about 30-60 seconds. " +
                   "Give details and examples to support your answer.";
        }
        if (topic.StartsWith("Discuss"))
        {
            return "This is Part 3 - Discussion. Give your opinion and support it with reasons. " +
                   "Try to speak for 1-2 minutes with clear arguments.";
        }
        return "Speak clearly and naturally. Try to use varied vocabulary and complex sentences.";
    }

    private static (float Fluency, float Pronunciation, float Grammar, string Feedback) SimulateAIEvaluation()
    {
        // Simulated AI scores (in production, use actual AI analysis)
        var random = new Random();
        var baseScore = 5.5f + (float)random.NextDouble() * 2; // 5.5-7.5 range

        return (
            Fluency: (float)Math.Round(baseScore + (random.NextDouble() - 0.5) * 1, 1),
            Pronunciation: (float)Math.Round(baseScore + (random.NextDouble() - 0.5) * 1, 1),
            Grammar: (float)Math.Round(baseScore + (random.NextDouble() - 0.5) * 1, 1),
            Feedback: GenerateFeedback(baseScore)
        );
    }

    private static string GenerateFeedback(float score)
    {
        if (score >= 7)
            return "Excellent performance! Your speech was fluent with good vocabulary range. " +
                   "Minor improvements: Try to use more complex grammatical structures.";
        if (score >= 6)
            return "Good effort! You communicated your ideas clearly. " +
                   "Areas for improvement: Work on pronunciation of longer words and use more linking words.";
        return "Keep practicing! Focus on speaking more fluently without long pauses. " +
               "Try to expand your vocabulary and practice common IELTS topics regularly.";
    }

    #endregion

    #region Teacher Content CRUD

    /// <summary>
    /// Teacher: Create a speaking topic
    /// </summary>
    [HttpPost("topics")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<SpeakingTopicDto>>> CreateTopic([FromBody] CreateSpeakingTopicRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<SpeakingTopicDto>.Fail("Invalid request"));

        var topic = new SpeakingTopic
        {
            PartNumber = int.TryParse(request.Part, out var p) ? p : 1,
            TopicName = request.TopicTitle,
            TargetBand = request.BandTarget,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.SpeakingTopics.Add(topic);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "create_speaking_topic", "SpeakingTopic", topic.TopicId);

        return Ok(ApiResponse<SpeakingTopicDto>.Ok(new SpeakingTopicDto
        {
            TopicId = topic.TopicId,
            Topic = topic.TopicName,
            Category = $"Part {topic.PartNumber}",
            DifficultyLevel = (int)topic.TargetBand
        }, "Speaking topic created"));
    }

    /// <summary>
    /// Teacher: Update a speaking topic
    /// </summary>
    [HttpPut("topics/{id}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateTopic(int id, [FromBody] CreateSpeakingTopicRequest request)
    {
        var topic = await _context.SpeakingTopics.FindAsync(id);
        if (topic == null)
            return NotFound(ApiResponse<bool>.Fail("Topic not found"));

        topic.PartNumber = int.TryParse(request.Part, out var p) ? p : 1;
        topic.TopicName = request.TopicTitle;
        topic.TargetBand = request.BandTarget;
        topic.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "update_speaking_topic", "SpeakingTopic", id);

        return Ok(ApiResponse<bool>.Ok(true, "Speaking topic updated"));
    }

    /// <summary>
    /// Teacher: Soft-delete a speaking topic
    /// </summary>
    [HttpDelete("topics/{id}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTopic(int id)
    {
        var topic = await _context.SpeakingTopics.FindAsync(id);
        if (topic == null)
            return NotFound(ApiResponse<bool>.Fail("Topic not found"));

        topic.IsActive = false;
        topic.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "soft_delete_speaking_topic", "SpeakingTopic", id);

        return Ok(ApiResponse<bool>.Ok(true, "Speaking topic archived"));
    }

    /// <summary>
    /// Teacher: Add a part/question to a speaking topic
    /// </summary>
    [HttpPost("topics/{topicId}/parts")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> CreateTopicPart(int topicId, [FromBody] CreateSpeakingTopicPartRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<bool>.Fail("Invalid request"));

        var topic = await _context.SpeakingTopics.FindAsync(topicId);
        if (topic == null)
            return NotFound(ApiResponse<bool>.Fail("Topic not found"));

        var part = new SpeakingTopicPart
        {
            TopicId = topicId,
            PartNumber = request.PartNumber,
            ContentText = request.ContentText,
            TimeLimitSeconds = request.TimeLimitSeconds,
            SequenceOrder = request.SequenceOrder,
            IsFollowUp = request.IsFollowUp
        };

        _context.SpeakingTopicParts.Add(part);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "create_speaking_topic_part", "SpeakingTopicPart", part.PartId);

        return Ok(ApiResponse<bool>.Ok(true, "Speaking topic part added"));
    }

    /// <summary>
    /// Teacher: Update a speaking topic part
    /// </summary>
    [HttpPut("topics/{topicId}/parts/{partId}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateTopicPart(int topicId, int partId, [FromBody] CreateSpeakingTopicPartRequest request)
    {
        var part = await _context.SpeakingTopicParts
            .FirstOrDefaultAsync(p => p.PartId == partId && p.TopicId == topicId);
        if (part == null)
            return NotFound(ApiResponse<bool>.Fail("Topic part not found"));

        part.PartNumber = request.PartNumber;
        part.ContentText = request.ContentText;
        part.TimeLimitSeconds = request.TimeLimitSeconds;
        part.SequenceOrder = request.SequenceOrder;
        part.IsFollowUp = request.IsFollowUp;

        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "update_speaking_topic_part", "SpeakingTopicPart", partId);

        return Ok(ApiResponse<bool>.Ok(true, "Speaking topic part updated"));
    }

    /// <summary>
    /// Teacher: Delete a speaking topic part
    /// </summary>
    [HttpDelete("topics/{topicId}/parts/{partId}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTopicPart(int topicId, int partId)
    {
        var part = await _context.SpeakingTopicParts
            .FirstOrDefaultAsync(p => p.PartId == partId && p.TopicId == topicId);
        if (part == null)
            return NotFound(ApiResponse<bool>.Fail("Topic part not found"));

        _context.SpeakingTopicParts.Remove(part);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "delete_speaking_topic_part", "SpeakingTopicPart", partId);

        return Ok(ApiResponse<bool>.Ok(true, "Speaking topic part deleted"));
    }

    #endregion
}
