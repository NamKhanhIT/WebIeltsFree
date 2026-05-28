using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebIeltsFree.Models;
using WebIeltsFree.Services;
using WebIeltsFree.Middleware;

namespace WebIeltsFree.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WritingController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPythonAiService _pythonAi;

    public WritingController(AppDbContext context, IPythonAiService pythonAi)
    {
        _context = context;
        _pythonAi = pythonAi;
    }

    /// <summary>
    /// Get writing prompts
    /// </summary>
    [HttpGet("prompts")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<WritingPromptDto>>>> GetPrompts([FromQuery] int? taskType = null)
    {
        try
        {
            var query = _context.WritingPrompts.AsQueryable();

            if (taskType is 1 or 2)
            {
                var taskTypeValue = taskType == 1 ? "task1" : "task2";
                query = query.Where(p => p.TaskType == taskTypeValue);
            }

            var prompts = await query
                .OrderBy(p => p.TaskType)
                .ThenBy(p => p.TargetBand)
                .Select(p => new WritingPromptDto
                {
                    PromptId = p.PromptId,
                    TaskType = p.TaskType == "task1" ? 1 : 2,
                    DifficultyLevel = (int)p.TargetBand,
                    Prompt = p.PromptText,
                    PromptImageUrl = p.PromptImageUrl
                })
                .Take(30)
                .ToListAsync();

            if (prompts.Count > 0)
                return Ok(ApiResponse<List<WritingPromptDto>>.Ok(prompts));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Writing prompts error: {ex.Message}");
        }

        // DB fallback to keep UI usable when schema/data is incomplete
        var fallbackPrompts = new List<WritingPromptDto>
        {
            new()
            {
                PromptId = 0,
                TaskType = 1,
                DifficultyLevel = 5,
                Prompt = "The chart below shows changes in the percentage of households with internet access in three countries between 2000 and 2020. Summarize the information by selecting and reporting the main features, and make comparisons where relevant."
            },
            new()
            {
                PromptId = 0,
                TaskType = 2,
                DifficultyLevel = 6,
                Prompt = "Some people think that university education should be free for everyone. Others believe students should pay for their studies. Discuss both views and give your own opinion."
            }
        };
        if (taskType is 1 or 2)
            fallbackPrompts = fallbackPrompts.Where(p => p.TaskType == taskType.Value).ToList();

        return Ok(ApiResponse<List<WritingPromptDto>>.Ok(fallbackPrompts));
    }

    /// <summary>
    /// Submit essay for AI evaluation (4 IELTS criteria)
    /// </summary>
    [HttpPost("submit")]
    public async Task<ActionResult<ApiResponse<WritingResultDto>>> SubmitEssay([FromBody] SubmitWritingRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<WritingResultDto>.Fail("Please log in"));

        // Sanitize input while preserving essay structure
        var sanitizedEssay = InputSanitizer.StripHtmlTags(request.EssayText);
        if (string.IsNullOrWhiteSpace(sanitizedEssay) || sanitizedEssay.Length < 50)
            return BadRequest(ApiResponse<WritingResultDto>.Fail("Essay must be at least 50 characters"));

        // Limit essay length to prevent abuse
        sanitizedEssay = InputSanitizer.Truncate(sanitizedEssay, 10000);

        var wordCount = sanitizedEssay.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

        // Perform content validation check
        var (isValid, validationMsg, warning) = ValidateEssayContent(sanitizedEssay, request.Prompt, request.TaskType, wordCount);
        if (!isValid)
        {
            return BadRequest(ApiResponse<WritingResultDto>.Fail(validationMsg));
        }

        bool isOffTopic = !string.IsNullOrEmpty(warning);

        WebIeltsFree.Services.WritingEvaluation aiEvaluation;
        try
        {
            aiEvaluation = await _pythonAi.EvaluateWritingAsync(sanitizedEssay, request.TaskType, request.Prompt);
        }
        catch
        {
            // Safe fallback when AI provider is unavailable
            var fallback = SimulateAIWritingEvaluation(sanitizedEssay, request.TaskType, wordCount);
            aiEvaluation = new WebIeltsFree.Services.WritingEvaluation
            {
                TaskAchievement = fallback.TaskAchievement,
                CoherenceCohesion = fallback.CoherenceCohesion,
                LexicalResource = fallback.LexicalResource,
                GrammaticalRange = fallback.GrammarRange,
                OverallBand = fallback.OverallBand,
                Feedback = fallback.Feedback,
                Strengths = new List<string> { "Clear structure", "Relevant response" },
                Improvements = new List<string> { "Expand lexical range", "Use more complex sentences" }
            };
        }

        // Apply off-topic cap on Task Achievement if appropriate
        if (isOffTopic)
        {
            aiEvaluation.TaskAchievement = Math.Min(4.5f, aiEvaluation.TaskAchievement);
            aiEvaluation.OverallBand = (float)Math.Round((aiEvaluation.TaskAchievement + aiEvaluation.CoherenceCohesion + aiEvaluation.LexicalResource + aiEvaluation.GrammaticalRange) / 4.0, 1);
            if (aiEvaluation.Improvements == null)
            {
                aiEvaluation.Improvements = new List<string>();
            }
            aiEvaluation.Improvements.Insert(0, "Align your content to address all aspects of the writing prompt topic.");
        }

        var aiFeedbackText = aiEvaluation.Feedback + "\n\nStrengths: " + string.Join(", ", aiEvaluation.Strengths) +
                             "\n\nAreas for Improvement: " + string.Join(", ", aiEvaluation.Improvements);

        if (isOffTopic)
        {
            aiFeedbackText = $"⚠️ WARNING: {warning}\n\n" + aiFeedbackText;
        }
        var createdAt = DateTime.UtcNow;

        var submission = new WritingSubmission
        {
            UserId = userId,
            Prompt = InputSanitizer.Truncate(request.Prompt, 500),
            EssayText = sanitizedEssay,
            TaskType = request.TaskType,
            WordCount = wordCount,
            BandScore = (decimal?)aiEvaluation.OverallBand,
            TaScore = (decimal?)aiEvaluation.TaskAchievement,
            CcScore = (decimal?)aiEvaluation.CoherenceCohesion,
            LrScore = (decimal?)aiEvaluation.LexicalResource,
            GraScore = (decimal?)aiEvaluation.GrammaticalRange,
            AiFeedback = aiFeedbackText,
            CreatedAt = createdAt
        };

        try
        {
            _context.WritingSubmissions.Add(submission);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUnknownColumnError(ex))
        {
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO tb_writing_submissions (user_id, prompt, essay_text, band_score, created_at)
                VALUES ({userId}, {submission.Prompt}, {sanitizedEssay}, {aiEvaluation.OverallBand}, {createdAt})");
            
            try
            {
                var conn = _context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT LAST_INSERT_ID();";
                    var lastIdObj = await cmd.ExecuteScalarAsync();
                    if (lastIdObj != null && lastIdObj != DBNull.Value)
                    {
                        submission.SubmissionId = Convert.ToInt32(lastIdObj);
                    }
                }
            }
            catch (Exception dbEx)
            {
                Console.WriteLine($"Failed to retrieve last insert ID via ADO.NET fallback: {dbEx.Message}");
                submission.SubmissionId = 0;
            }
        }

        return Ok(ApiResponse<WritingResultDto>.Ok(new WritingResultDto
        {
            SubmissionId = submission.SubmissionId,
            BandScore = (float?)submission.BandScore,
            TaskAchievementScore = (float?)submission.TaScore,
            CoherenceCohesionScore = (float?)submission.CcScore,
            LexicalResourceScore = (float?)submission.LrScore,
            GrammarAccuracyScore = (float?)submission.GraScore,
            AiFeedback = submission.AiFeedback,
            WordCount = submission.WordCount,
            CreatedAt = submission.CreatedAt
        }));
    }

    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<List<WritingResultDto>>>> GetHistory()
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<List<WritingResultDto>>.Fail("Please log in"));

        try
        {
            var history = await _context.WritingSubmissions
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .Take(20)
                .Select(w => new WritingResultDto
                {
                    SubmissionId = w.SubmissionId,
                    Prompt = w.Prompt,
                    BandScore = (float?)w.BandScore,
                    AiFeedback = w.AiFeedback,
                    WordCount = w.WordCount,
                    CreatedAt = w.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<WritingResultDto>>.Ok(history));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Writing history error: {ex.Message}");
            if (IsUnknownColumnError(ex))
            {
                var historyFallback = await _context.WritingSubmissions
                    .Where(w => w.UserId == userId)
                    .OrderByDescending(w => w.CreatedAt)
                    .Take(20)
                    .Select(w => new WritingResultDto
                    {
                        SubmissionId = w.SubmissionId,
                        Prompt = w.Prompt,
                        BandScore = (float?)w.BandScore,
                        CreatedAt = w.CreatedAt
                    })
                    .ToListAsync();
                return Ok(ApiResponse<List<WritingResultDto>>.Ok(historyFallback));
            }
            return Ok(ApiResponse<List<WritingResultDto>>.Ok(new List<WritingResultDto>()));
        }
    }

    [HttpGet("history/{id}")]
    public async Task<ActionResult<ApiResponse<WritingResultDto>>> GetSubmission(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<WritingResultDto>.Fail("Please log in"));

        WritingResultDto? submission;
        try
        {
            submission = await _context.WritingSubmissions
                .Where(w => w.SubmissionId == id && w.UserId == userId)
                .Select(w => new WritingResultDto
                {
                    SubmissionId = w.SubmissionId,
                    Prompt = w.Prompt,
                    BandScore = (float?)w.BandScore,
                    TaskAchievementScore = (float?)w.TaScore,
                    CoherenceCohesionScore = (float?)w.CcScore,
                    LexicalResourceScore = (float?)w.LrScore,
                    GrammarAccuracyScore = (float?)w.GraScore,
                    AiFeedback = w.AiFeedback,
                    WordCount = w.WordCount,
                    CreatedAt = w.CreatedAt
                })
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Writing submission detail error: {ex.Message}");
            if (IsUnknownColumnError(ex))
            {
                submission = await _context.WritingSubmissions
                    .Where(w => w.SubmissionId == id && w.UserId == userId)
                    .Select(w => new WritingResultDto
                    {
                        SubmissionId = w.SubmissionId,
                        Prompt = w.Prompt,
                        BandScore = (float?)w.BandScore,
                        CreatedAt = w.CreatedAt
                    })
                    .FirstOrDefaultAsync();
            }
            else
            {
                return StatusCode(500, ApiResponse<WritingResultDto>.Fail("Failed to load submission detail"));
            }
        }

        if (submission == null)
            return NotFound(ApiResponse<WritingResultDto>.Fail("Submission not found"));

        return Ok(ApiResponse<WritingResultDto>.Ok(submission));
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) return 0;
        return int.TryParse(claim.Value, out var userId) ? userId : 0;
    }

    private static (bool IsValid, string Message, string? Warning) ValidateEssayContent(string essay, string prompt, int taskType, int wordCount)
    {
        if (string.IsNullOrWhiteSpace(essay))
        {
            return (false, "Essay content cannot be empty.", null);
        }

        if (taskType == 1 && wordCount < 50)
        {
            return (false, "Your Task 1 essay must be at least 50 words to be analyzed.", null);
        }
        if (taskType == 2 && wordCount < 100)
        {
            return (false, "Your Task 2 essay must be at least 100 words to be analyzed.", null);
        }

        var vietnameseProfanities = new[] { "địt", "đéo", "lồn", "cặc", "buồi", "đm", "đcm", "vl", "vcl", "chó đẻ", "đĩ", "mẹ kiếp" };
        var englishProfanities = new[] { "fuck", "shit", "bitch", "asshole", "bastard", "cunt", "dick" };

        var essayLower = essay.ToLowerInvariant();

        foreach (var badWord in vietnameseProfanities)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(essayLower, @"\b" + System.Text.RegularExpressions.Regex.Escape(badWord) + @"\b") || essayLower.Contains(badWord))
            {
                return (false, "Your essay contains inappropriate or offensive language. Please maintain an academic tone for IELTS writing.", null);
            }
        }
        foreach (var badWord in englishProfanities)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(essayLower, @"\b" + System.Text.RegularExpressions.Regex.Escape(badWord) + @"\b"))
            {
                return (false, "Your essay contains inappropriate or offensive language. Please maintain an academic tone for IELTS writing.", null);
            }
        }

        var words = essay.Split(new[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?', ';', ':', '(', ')', '[', ']', '{', '}', '"', '\'' }, StringSplitOptions.RemoveEmptyEntries);
        int longWordsCount = 0;
        int vowellessLongWordsCount = 0;
        
        var vowels = new System.Collections.Generic.HashSet<char>("aeiouyáàảãạâầẩẫậăằẳẵặéèẻẽẹêềểễệíìỉĩịóòỏõọôồổỗộơờởỡợúùủũụưừửữựýỳỷỹỵ");

        foreach (var w in words)
        {
            var cleanW = w.Trim().ToLowerInvariant();
            if (cleanW.Length > 5)
            {
                longWordsCount++;
                bool hasVowel = false;
                foreach (var c in cleanW)
                {
                    if (vowels.Contains(c))
                    {
                        hasVowel = true;
                        break;
                    }
                }
                if (!hasVowel)
                {
                    vowellessLongWordsCount++;
                }
            }
        }

        if (longWordsCount > 0 && ((double)vowellessLongWordsCount / longWordsCount) > 0.15)
        {
            return (false, "Your essay contains too many invalid words (gibberish detected). Please write clear, meaningful sentences.", null);
        }

        if (wordCount > 15)
        {
            var cleanWords = new System.Collections.Generic.List<string>();
            foreach (var w in words)
            {
                var cleanW = w.Trim().ToLowerInvariant();
                if (cleanW.Length > 0)
                {
                    cleanWords.Add(cleanW);
                }
            }

            if (cleanWords.Count > 15)
            {
                var uniqueWords = new System.Collections.Generic.HashSet<string>(cleanWords);
                if ((double)uniqueWords.Count / cleanWords.Count < 0.25)
                {
                    return (false, "Your essay has excessive repetition. Please expand your ideas and vary your vocabulary.", null);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(prompt))
        {
            var stopWords = new System.Collections.Generic.HashSet<string>(new[]
            {
                "shows", "below", "chart", "graph", "table", "diagram", "percentage", "proportion", "information",
                "selecting", "reporting", "main", "features", "make", "comparisons", "relevant", "summarize",
                "about", "that", "should", "some", "others", "believe", "discuss", "both", "views", "give",
                "your", "opinion", "agree", "disagree", "extent", "explain", "describe", "illustration",
                "depicts", "illustrates", "and", "the", "for", "with", "from", "between", "how", "what", "which",
                "why", "who", "whom", "this", "these", "those", "their", "them", "they", "our", "you", "were",
                "was", "been", "have", "has", "had", "are", "isn", "aren", "wasn", "weren", "haven", "hasn", "hadn"
            });

            var promptWords = prompt.Split(new[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?', ';', ':', '(', ')', '[', ']', '{', '}', '"', '\'' }, StringSplitOptions.RemoveEmptyEntries);
            var promptStems = new System.Collections.Generic.HashSet<string>();

            foreach (var pw in promptWords)
            {
                var cleanPw = pw.Trim().ToLowerInvariant();
                if (cleanPw.Length >= 3 && !stopWords.Contains(cleanPw))
                {
                    var stem = StemWord(cleanPw);
                    if (stem.Length >= 3)
                    {
                        promptStems.Add(stem);
                    }
                }
            }

            if (promptStems.Count > 0)
            {
                bool hasOverlap = false;
                foreach (var ew in words)
                {
                    var cleanEw = ew.Trim().ToLowerInvariant();
                    if (cleanEw.Length >= 3)
                    {
                        var stem = StemWord(cleanEw);
                        if (promptStems.Contains(stem))
                        {
                            hasOverlap = true;
                            break;
                        }
                    }
                }

                if (!hasOverlap)
                {
                    return (true, "Valid", "Your essay may not fully address the topic.");
                }
            }
        }

        return (true, "Valid", null);
    }

    private static string StemWord(string word)
    {
        if (string.IsNullOrEmpty(word)) return string.Empty;
        word = word.ToLowerInvariant();
        if (word.EndsWith("ies")) word = word.Substring(0, word.Length - 3) + "i";
        else if (word.EndsWith("ing")) word = word.Substring(0, word.Length - 3);
        else if (word.EndsWith("ed")) word = word.Substring(0, word.Length - 2);
        else if (word.EndsWith("es")) word = word.Substring(0, word.Length - 2);
        else if (word.EndsWith("s") && !word.EndsWith("ss")) word = word.Substring(0, word.Length - 1);
        else if (word.EndsWith("tion")) word = word.Substring(0, word.Length - 4);
        else if (word.EndsWith("ment")) word = word.Substring(0, word.Length - 4);
        else if (word.EndsWith("ly")) word = word.Substring(0, word.Length - 2);
        return word;
    }

    private static bool IsUnknownColumnError(Exception ex)
    {
        var text = ex.ToString();
        return text.Contains("Unknown column", StringComparison.OrdinalIgnoreCase);
    }

    private static LocalWritingEvaluation SimulateAIWritingEvaluation(string essay, int taskType, int wordCount)
    {
        // Simulated AI evaluation (fallback when Gemini API key not configured)
        var random = new Random();
        var baseScore = 5.5f;

        // Adjust base score based on word count
        if (taskType == 1 && wordCount >= 150) baseScore += 0.5f;
        if (taskType == 2 && wordCount >= 250) baseScore += 0.5f;
        if (wordCount >= 300) baseScore += 0.5f;

        // Random variation
        baseScore += (float)(random.NextDouble() * 1.5);
        baseScore = Math.Min(8.5f, baseScore); // Cap at 8.5 for simulation

        var taskAchievement = (float)Math.Round(baseScore + (random.NextDouble() - 0.5) * 1, 1);
        var coherenceCohesion = (float)Math.Round(baseScore + (random.NextDouble() - 0.5) * 1, 1);
        var lexicalResource = (float)Math.Round(baseScore + (random.NextDouble() - 0.5) * 1, 1);
        var grammarAccuracy = (float)Math.Round(baseScore + (random.NextDouble() - 0.5) * 1, 1);

        // Ensure scores are within 0-9 range
        taskAchievement = Math.Clamp(taskAchievement, 4.0f, 9.0f);
        coherenceCohesion = Math.Clamp(coherenceCohesion, 4.0f, 9.0f);
        lexicalResource = Math.Clamp(lexicalResource, 4.0f, 9.0f);
        grammarAccuracy = Math.Clamp(grammarAccuracy, 4.0f, 9.0f);

        var overallBand = (float)Math.Round((taskAchievement + coherenceCohesion + lexicalResource + grammarAccuracy) / 4, 1);

        return new LocalWritingEvaluation
        {
            TaskAchievement = taskAchievement,
            CoherenceCohesion = coherenceCohesion,
            LexicalResource = lexicalResource,
            GrammarRange = grammarAccuracy,
            OverallBand = overallBand,
            Feedback = GenerateWritingFeedback(taskAchievement, coherenceCohesion, lexicalResource, grammarAccuracy, wordCount, taskType)
        };
    }

    private static string GenerateWritingFeedback(float ta, float cc, float lr, float ga, int wordCount, int taskType)
    {
        var feedback = new List<string>();

        // Task Achievement feedback
        if (ta >= 7)
            feedback.Add("✅ Task Achievement: Excellent! You addressed all parts of the task with relevant, extended ideas.");
        else if (ta >= 6)
            feedback.Add("✓ Task Achievement: Good coverage of the task. Try to develop your main ideas more fully.");
        else
            feedback.Add("⚠ Task Achievement: Ensure you address all parts of the question. Provide more specific examples.");

        // Coherence & Cohesion feedback
        if (cc >= 7)
            feedback.Add("✅ Coherence & Cohesion: Well-organized essay with clear progression of ideas.");
        else if (cc >= 6)
            feedback.Add("✓ Coherence & Cohesion: Generally well-organized. Use more varied linking words.");
        else
            feedback.Add("⚠ Coherence & Cohesion: Work on paragraph structure and use more connectors (However, Furthermore, etc.).");

        // Lexical Resource feedback
        if (lr >= 7)
            feedback.Add("✅ Lexical Resource: Good vocabulary range with some less common words.");
        else if (lr >= 6)
            feedback.Add("✓ Lexical Resource: Adequate vocabulary. Try to use more topic-specific vocabulary.");
        else
            feedback.Add("⚠ Lexical Resource: Expand your vocabulary. Avoid repetition of words.");

        // Grammar feedback
        if (ga >= 7)
            feedback.Add("✅ Grammatical Range: Good variety of sentence structures with few errors.");
        else if (ga >= 6)
            feedback.Add("✓ Grammatical Range: Generally accurate. Try using more complex sentences.");
        else
            feedback.Add("⚠ Grammatical Range: Review basic grammar rules. Practice complex sentence structures.");

        // Word count advice
        var minWords = taskType == 1 ? 150 : 250;
        if (wordCount < minWords)
            feedback.Add($"📝 Word Count: {wordCount} words. Minimum required: {minWords}. Develop your ideas further.");
        else if (wordCount > minWords + 100)
            feedback.Add($"📝 Word Count: {wordCount} words. Good length!");
        else
            feedback.Add($"📝 Word Count: {wordCount} words. Meets the requirement.");

        return string.Join("\n\n", feedback);
    }

    private class LocalWritingEvaluation
    {
        public float TaskAchievement { get; set; }
        public float CoherenceCohesion { get; set; }
        public float LexicalResource { get; set; }
        public float GrammarRange { get; set; }
        public float OverallBand { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }

    #endregion

    #region Teacher Content CRUD

    /// <summary>
    /// Teacher: Create a new writing prompt
    /// </summary>
    [HttpPost("prompts")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<WritingPromptDto>>> CreatePrompt([FromBody] CreateWritingPromptRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<WritingPromptDto>.Fail("Invalid request"));

        var prompt = new WritingPrompt
        {
            TaskType = request.TaskType,
            PromptText = request.PromptText,
            PromptImageUrl = request.PromptImageUrl,
            ChartType = request.ChartType,
            TargetBand = request.DifficultyLevel,
            SampleAnswer = request.SampleAnswer,
            NotesForTeacher = request.NotesForTeacher,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.WritingPrompts.Add(prompt);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "create_writing_prompt", "WritingPrompt", prompt.PromptId);

        return Ok(ApiResponse<WritingPromptDto>.Ok(new WritingPromptDto
        {
            PromptId = prompt.PromptId,
            TaskType = prompt.TaskType == "task1" ? 1 : 2,
            DifficultyLevel = (int)prompt.TargetBand,
            Prompt = prompt.PromptText,
            PromptImageUrl = prompt.PromptImageUrl
        }, "Writing prompt created"));
    }

    /// <summary>
    /// Teacher: Update an existing writing prompt
    /// </summary>
    [HttpPut("prompts/{id}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdatePrompt(int id, [FromBody] CreateWritingPromptRequest request)
    {
        var prompt = await _context.WritingPrompts.FindAsync(id);
        if (prompt == null)
            return NotFound(ApiResponse<bool>.Fail("Prompt not found"));

        prompt.TaskType = request.TaskType;
        prompt.PromptText = request.PromptText;
        prompt.PromptImageUrl = request.PromptImageUrl;
        prompt.ChartType = request.ChartType;
        prompt.TargetBand = request.DifficultyLevel;
        prompt.SampleAnswer = request.SampleAnswer;
        prompt.NotesForTeacher = request.NotesForTeacher;
        prompt.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "update_writing_prompt", "WritingPrompt", id);

        return Ok(ApiResponse<bool>.Ok(true, "Writing prompt updated"));
    }

    /// <summary>
    /// Teacher: Soft-delete a writing prompt (set IsActive = false)
    /// </summary>
    [HttpDelete("prompts/{id}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePrompt(int id)
    {
        var prompt = await _context.WritingPrompts.FindAsync(id);
        if (prompt == null)
            return NotFound(ApiResponse<bool>.Fail("Prompt not found"));

        prompt.IsActive = false;
        prompt.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "soft_delete_writing_prompt", "WritingPrompt", id);

        return Ok(ApiResponse<bool>.Ok(true, "Writing prompt archived"));
    }

    #endregion
}
