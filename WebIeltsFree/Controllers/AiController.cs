using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WebIeltsFree.Models;
using WebIeltsFree.Services;
using WebIeltsFree.Middleware;

namespace WebIeltsFree.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly IGeminiService _gemini;
    private readonly IChatbotService _chatbot;
    private readonly ISpacedRepetitionService _spacedRepetition;
    private readonly ILogger<AiController> _logger;

    public AiController(
        AppDbContext context, 
        IConfiguration config,
        IGeminiService gemini,
        IChatbotService chatbot,
        ISpacedRepetitionService spacedRepetition,
        ILogger<AiController> logger)
    {
        _context = context;
        _config = config;
        _gemini = gemini;
        _chatbot = chatbot;
        _spacedRepetition = spacedRepetition;
        _logger = logger;
    }

    /// <summary>
    /// Chat with AI tutor (RAG-based)
    /// </summary>
    [HttpPost("chat")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AiChatResponse>>> Chat([FromBody] AiChatRequest request)
    {
        var userId = GetCurrentUserIdOrDefault();

        // Sanitize input to prevent XSS/injection
        var sanitizedMessage = InputSanitizer.StripHtmlTags(request.Message);
        if (string.IsNullOrWhiteSpace(sanitizedMessage))
            return BadRequest(ApiResponse<AiChatResponse>.Fail("Message cannot be empty"));

        // Use RAG chatbot service
        var history = request.Context?.Split('\n')
            .Select(line => new ChatMessage { Role = "user", Content = InputSanitizer.StripHtmlTags(line) })
            .ToList();

        var chatResponse = await _chatbot.GetResponseAsync(userId, sanitizedMessage, history);

        return Ok(ApiResponse<AiChatResponse>.Ok(new AiChatResponse
        {
            Response = chatResponse.Message,
            SuggestedFollowUps = chatResponse.SuggestedQuestions
        }));
    }

    /// <summary>
    /// Get user's existing learning path
    /// </summary>
    [HttpGet("learning-path")]
    public async Task<ActionResult<ApiResponse<LearningPathDto>>> GetLearningPath()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(ApiResponse<LearningPathDto>.Fail("Please log in"));

            // Query only legacy-safe columns to avoid hard failures on older schemas.
            var roadmapSummary = await _context.AIRoadmaps
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.GeneratedAt)
                .Select(r => new { r.RoadmapId, r.GeneratedAt })
                .FirstOrDefaultAsync();

            if (roadmapSummary == null)
                return NotFound(ApiResponse<LearningPathDto>.Fail("No learning path found. Please generate one first."));

            var targetBandFromGoal = await _context.UserGoals
                .Where(g => g.UserId == userId)
                .Select(g => (float?)g.TargetBand)
                .FirstOrDefaultAsync();
            var targetBand = targetBandFromGoal.GetValueOrDefault(7.0f);

            var estimatedWeeksFromSteps = await _context.AIRoadmapSteps
                .Where(s => s.RoadmapId == roadmapSummary.RoadmapId)
                .Select(s => s.WeekNumber ?? 0)
                .DefaultIfEmpty(0)
                .MaxAsync();

            var estimatedWeeks = estimatedWeeksFromSteps > 0 ? estimatedWeeksFromSteps : 12;
            var weeklyPlan = GenerateWeeklyPlan(estimatedWeeks, null, targetBand);
            
            var weeksSinceCreation = (int)((DateTime.UtcNow - roadmapSummary.GeneratedAt).TotalDays / 7) + 1;
            var currentWeek = Math.Max(1, Math.Min(weeksSinceCreation, estimatedWeeks));

            return Ok(ApiResponse<LearningPathDto>.Ok(new LearningPathDto
            {
                RoadmapId = roadmapSummary.RoadmapId,
                TargetBand = targetBand,
                EstimatedWeeks = estimatedWeeks,
                CurrentWeek = currentWeek,
                WeeklyPlan = weeklyPlan
            }));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetLearningPath error: {ex.Message}");
            return StatusCode(500, ApiResponse<LearningPathDto>.Fail("Failed to load learning path"));
        }
    }

    [HttpPost("generate-learning-path")]
    public async Task<ActionResult<ApiResponse<LearningPathDto>>> GenerateLearningPath([FromBody] GenerateLearningPathRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(ApiResponse<LearningPathDto>.Fail("Please log in to generate a learning path"));

            float currentBand = 5.0f;
            try
            {
                var skillAnalysis = await _context.AISkillAnalyses
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.AnalyzedAt)
                    .FirstOrDefaultAsync();
                currentBand = skillAnalysis?.OverallBand ?? 5.0f;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error getting skill analysis");
                return Ok(ApiResponse<LearningPathDto>.Fail("An error occurred. Please try again."));
            }

            float targetBand = request.TargetBand;
            float bandGap = targetBand - currentBand;

            int estimatedWeeks = (int)Math.Ceiling(bandGap * 8);
            estimatedWeeks = Math.Max(4, Math.Min(52, estimatedWeeks)); // Between 4-52 weeks

            var generatedAt = DateTime.UtcNow;

            // Insert with SQL that works for both legacy and newer schemas.
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO tb_ai_roadmaps (user_id, generated_at)
                VALUES ({userId}, {generatedAt})");

            var roadmap = await _context.AIRoadmaps
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.GeneratedAt)
                .Select(r => new { r.RoadmapId })
                .FirstOrDefaultAsync();

            var roadmapId = roadmap?.RoadmapId ?? 0;

            // Generate weekly plan (simplified - in production, use AI to customize)
            var weeklyPlan = GenerateWeeklyPlan(estimatedWeeks, request.FocusAreas, targetBand);

            return Ok(ApiResponse<LearningPathDto>.Ok(new LearningPathDto
            {
                RoadmapId = roadmapId,
                TargetBand = targetBand,
                EstimatedWeeks = estimatedWeeks,
                WeeklyPlan = weeklyPlan
            }));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GenerateLearningPath error: {ex.Message}");
            return Ok(ApiResponse<LearningPathDto>.Ok(new LearningPathDto
            {
                RoadmapId = 0,
                TargetBand = request.TargetBand,
                EstimatedWeeks = 12,
                WeeklyPlan = GenerateWeeklyPlan(12, null, request.TargetBand)
            }, "Learning path generated (some features may be limited)"));
        }
    }

    [HttpGet("recommendations")]
    public async Task<ActionResult<ApiResponse<List<AiRecommendationDto>>>> GetRecommendations()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Ok(ApiResponse<List<AiRecommendationDto>>.Ok(GetDefaultRecommendations()));
            }

            // Get user's progress data
            var progress = await _context.UserLearningProgress
                .Where(p => p.UserId == userId)
                .Include(p => p.Lesson)
                .ToListAsync();

            var testAttempts = await _context.UserTestAttempts
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var goal = await _context.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);
            var targetBand = goal?.TargetBand ?? 6.5f;

            // Generate recommendations based on analysis
            var recommendations = GenerateRecommendations(progress, testAttempts, targetBand);

            return Ok(ApiResponse<List<AiRecommendationDto>>.Ok(recommendations));
        }
        catch
        {
            return Ok(ApiResponse<List<AiRecommendationDto>>.Ok(GetDefaultRecommendations()));
        }
    }

    private List<AiRecommendationDto> GetDefaultRecommendations()
    {
        return new List<AiRecommendationDto>
        {
            new() { RecommendationType = "placement", Title = "Take Placement Test", Description = "Find your current level to get personalized recommendations", Priority = 1 },
            new() { RecommendationType = "course", Title = "Explore Courses", Description = "Browse our IELTS preparation courses", Priority = 2 },
            new() { RecommendationType = "practice", Title = "Try Writing Practice", Description = "Submit an essay for AI feedback", Priority = 3 }
        };
    }

    /// <summary>
    /// Predict band score based on current performance
    /// </summary>
    [HttpGet("predict-band")]
    public async Task<ActionResult<ApiResponse<BandPredictionDto>>> PredictBand()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Ok(ApiResponse<BandPredictionDto>.Ok(new BandPredictionDto
                {
                    PredictedBand = 0,
                    ConfidencePercent = 0,
                    Message = "Not enough data yet"
                }));
            }

            // Get recent test attempts
            var recentTests = await _context.UserTestAttempts
                .Where(a => a.UserId == userId && a.BandScore != null)
                .OrderByDescending(a => a.FinishedAt)
                .Take(5)
                .ToListAsync();

            // Get recent speaking/writing scores - use try/catch for potential schema issues
            List<SpeakingSession> recentSpeaking = new();
            List<WritingSubmission> recentWriting = new();
            
            try
            {
                recentSpeaking = await _context.SpeakingSessions
                    .Where(s => s.UserId == userId && s.FluencyScore != null)
                    .OrderByDescending(s => s.CreatedAt)
                    .Take(3)
                    .ToListAsync();
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, "Error getting speaking sessions");
                return Ok(ApiResponse<BandPredictionDto>.Fail("An error occurred. Please try again."));
            }
            
            try
            {
                recentWriting = await _context.WritingSubmissions
                    .Where(w => w.UserId == userId && w.BandScore != null)
                    .OrderByDescending(w => w.CreatedAt)
                    .Take(3)
                    .ToListAsync();
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, "Error getting writing submissions");
                return Ok(ApiResponse<BandPredictionDto>.Fail("An error occurred. Please try again."));
            }

            // If no data at all, return safe response
            if (!recentTests.Any() && !recentSpeaking.Any() && !recentWriting.Any())
            {
                return Ok(ApiResponse<BandPredictionDto>.Ok(new BandPredictionDto
                {
                    PredictedBand = 0,
                    ConfidencePercent = 0,
                    Message = "Not enough data yet. Take a placement test to get started."
                }));
            }

            // Calculate predicted scores
            float readingListening = recentTests.Any() ? (float)Math.Round(recentTests.Average(t => t.BandScore ?? 0) * 2, MidpointRounding.AwayFromZero) / 2 : 5.5f;
            float speaking = recentSpeaking.Any() 
                ? (float)Math.Round(recentSpeaking.Average(s => ((s.FluencyScore ?? 0) + (s.PronunciationScore ?? 0) + (s.GrammarScore ?? 0)) / 3) * 2, MidpointRounding.AwayFromZero) / 2
                : 5.5f;
            float writing = recentWriting.Any() ? (float)Math.Round((float)recentWriting.Average(w => w.BandScore ?? 5.5m) * 2, MidpointRounding.AwayFromZero) / 2 : 5.5f;

            float predictedBand = (readingListening + readingListening + speaking + writing) / 4;
            predictedBand = (float)Math.Round(predictedBand * 2) / 2; // Round to nearest 0.5

            return Ok(ApiResponse<BandPredictionDto>.Ok(new BandPredictionDto
            {
                PredictedBand = predictedBand,
                ConfidencePercent = recentTests.Count + recentSpeaking.Count + recentWriting.Count >= 5 ? 85 : 60,
                PredictedSkills = new SkillBreakdown
                {
                    Reading = readingListening,
                    Listening = readingListening,
                    Speaking = speaking,
                    Writing = writing
                },
                Insights = GenerateBandInsights(predictedBand, readingListening, speaking, writing)
            }));
        }
        catch (Exception ex)
        {
            // Return safe fallback instead of 500 error
            return Ok(ApiResponse<BandPredictionDto>.Ok(new BandPredictionDto
            {
                PredictedBand = 0,
                ConfidencePercent = 0,
                Message = "Not enough data yet"
            }));
        }
    }

    /// <summary>
    /// Get AI-generated insights about learning progress
    /// </summary>
    [HttpGet("insights")]
    public async Task<ActionResult<ApiResponse<AiInsightsDto>>> GetInsights()
    {
        try
        {
            var userId = GetCurrentUserId();

            var progress = await _context.UserLearningProgress
                .Where(p => p.UserId == userId)
                .Include(p => p.Lesson)
                .ToListAsync();

            AISkillAnalysis? skillAnalysis = null;
            try
            {
                skillAnalysis = await _context.AISkillAnalyses
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.AnalyzedAt)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, "Error getting skill analysis");
                return Ok(ApiResponse<AiInsightsDto>.Fail("An error occurred. Please try again."));
            }

            var insights = new AiInsightsDto
            {
                OverallAssessment = GenerateOverallAssessment(progress, skillAnalysis),
                Strengths = IdentifyStrengths(skillAnalysis),
                AreasForImprovement = IdentifyWeaknesses(skillAnalysis),
                StudyTips = GenerateStudyTips(skillAnalysis)
            };

            return Ok(ApiResponse<AiInsightsDto>.Ok(insights));
        }
        catch
        {
            return Ok(ApiResponse<AiInsightsDto>.Ok(new AiInsightsDto
            {
                OverallAssessment = "Start your IELTS journey by taking a placement test!",
                Strengths = new List<string> { "Ready to learn" },
                AreasForImprovement = new List<string> { "Take placement test to identify areas" },
                StudyTips = new List<string> { "Begin with the placement test", "Set your target band score" }
            }));
        }
    }

    /// <summary>
    /// Get vocabulary suggestions based on current level
    /// </summary>
    [HttpPost("vocabulary-suggestions")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetVocabularySuggestions([FromBody] string topic)
    {
        var userId = GetCurrentUserId();

        // Get user's level
        var goal = await _context.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);
        var targetBand = goal?.TargetBand ?? 6.5f;

        // Suggest vocabulary based on topic and level
        var suggestions = GetVocabularyForTopic(topic, targetBand);

        return Ok(ApiResponse<List<string>>.Ok(suggestions));
    }

    /// <summary>
    /// Convert text to speech using Google Cloud Text-to-Speech or Gemini
    /// </summary>
    [HttpPost("text-to-speech")]
    public async Task<ActionResult<ApiResponse<TextToSpeechResponse>>> TextToSpeech([FromBody] TextToSpeechRequest request)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(ApiResponse<TextToSpeechResponse>.Fail("Text cannot be empty"));

            if (request.Text.Length > 5000)
                return BadRequest(ApiResponse<TextToSpeechResponse>.Fail("Text too long (max 5000 characters)"));

            // Get API key for Google Cloud TTS
            var apiKey = _config["Google:CloudTTSKey"] ?? _config["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                return StatusCode(500, ApiResponse<TextToSpeechResponse>.Fail("Text-to-speech service not configured"));

            // Use Google Cloud Text-to-Speech API
            using (var client = new HttpClient())
            {
                var url = "https://texttospeech.googleapis.com/v1/text:synthesize";
                var requestBody = new
                {
                    input = new { text = request.Text },
                    voice = new
                    {
                        languageCode = request.Language ?? "en-US",
                        name = request.Voice ?? "en-US-Neural2-C"  // Female voice
                    },
                    audioConfig = new
                    {
                        audioEncoding = "MP3",
                        pitch = request.Pitch ?? 0.0,
                        speakingRate = request.SpeakingRate ?? 1.0
                    }
                };

                var jsonContent = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{url}?key={apiKey}", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    // Fallback: return error but don't crash
                    return StatusCode(503, ApiResponse<TextToSpeechResponse>.Fail(
                        "Text-to-speech service temporarily unavailable"));
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = System.Text.Json.JsonDocument.Parse(responseContent);
                var audioContent = jsonResponse.RootElement.GetProperty("audioContent").GetString();

                // Convert to data URL for direct playback
                var dataUrl = $"data:audio/mp3;base64,{audioContent}";

                return Ok(ApiResponse<TextToSpeechResponse>.Ok(new TextToSpeechResponse
                {
                    AudioUrl = dataUrl,
                    Duration = request.Text.Length / 10 // Rough estimate: ~10 chars per second
                }));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Text-to-speech error: {ex.Message}");
            return StatusCode(500, ApiResponse<TextToSpeechResponse>.Fail(
                "Error generating speech. Please try again."));
        }
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        return GetCurrentUserIdOrDefault();
    }

    private int GetCurrentUserIdOrDefault()
    {
        var candidateClaims = new[]
        {
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            User.FindFirstValue("nameid"),
            User.FindFirstValue("sub"),
            User.FindFirstValue("user_id")
        };

        foreach (var claimValue in candidateClaims)
        {
            if (int.TryParse(claimValue, out var id) && id > 0)
                return id;
        }

        return 0;
    }

    private static string GenerateAIResponse(string message, string? context)
    {
        var lowerMessage = message.ToLower();

        // IELTS-specific responses
        if (lowerMessage.Contains("writing task 1"))
            return "For IELTS Writing Task 1, you need to describe visual information (graphs, charts, diagrams). " +
                   "Key tips:\n• Write at least 150 words\n• Include an overview paragraph\n• Use data to support your description\n• Don't give opinions";

        if (lowerMessage.Contains("writing task 2"))
            return "For IELTS Writing Task 2, you write an essay in response to an argument or problem. " +
                   "Structure:\n• Introduction (paraphrase + thesis)\n• Body paragraph 1 (main idea + support)\n• Body paragraph 2 (main idea + support)\n• Conclusion (summarize + final thought)\n\nAim for 250-280 words.";

        if (lowerMessage.Contains("speaking"))
            return "IELTS Speaking has 3 parts:\n• Part 1: Introduction questions (4-5 mins)\n• Part 2: Cue card - speak for 2 mins\n• Part 3: Discussion (4-5 mins)\n\nTips: Speak naturally, extend your answers, and use varied vocabulary.";

        if (lowerMessage.Contains("band") || lowerMessage.Contains("score"))
            return "IELTS scores range from 0-9 in 0.5 increments. For most universities:\n• Band 6.0-6.5: Minimum for undergraduate\n• Band 7.0+: Required for postgraduate\n\nThe overall band is an average of 4 skills (Reading, Listening, Writing, Speaking).";

        if (lowerMessage.Contains("improve") || lowerMessage.Contains("better"))
            return "To improve your IELTS score:\n1. Practice all 4 skills daily\n2. Use official Cambridge practice tests\n3. Focus on your weakest skill\n4. Learn topic-specific vocabulary\n5. Time yourself during practice\n6. Get feedback on Writing & Speaking";

        // Default helpful response
        return "I'm your IELTS AI tutor! I can help you with:\n• Writing Task 1 & 2 tips\n• Speaking practice advice\n• Reading & Listening strategies\n• Vocabulary building\n• Band score requirements\n\nWhat would you like to know?";
    }

    private static List<string> GetSuggestedFollowUps(string message)
    {
        var lowerMessage = message.ToLower();

        if (lowerMessage.Contains("writing"))
            return new List<string> { "How to structure an essay?", "Common Writing mistakes", "Writing vocabulary tips" };

        if (lowerMessage.Contains("speaking"))
            return new List<string> { "Speaking Part 2 tips", "How to extend answers?", "Common Speaking topics" };

        return new List<string> { "How to improve my band score?", "Writing Task 2 structure", "Speaking practice tips" };
    }

    private static List<WeekPlanDto> GenerateWeeklyPlan(int totalWeeks, List<string>? focusAreas, float targetBand)
    {
        var plan = new List<WeekPlanDto>();
        var skills = new[] { "Reading", "Listening", "Writing", "Speaking" };

        for (int week = 1; week <= Math.Min(totalWeeks, 12); week++)
        {
            var focusSkill = skills[(week - 1) % 4];
            plan.Add(new WeekPlanDto
            {
                Week = week,
                WeekNumber = week,
                Focus = $"Focus on {focusSkill} - Target Band {targetBand}",
                Lessons = new List<LessonSummaryDto>
                {
                    new() { Title = $"{focusSkill} Fundamentals", SkillType = focusSkill.ToLower(), EstimatedMinutes = 30 },
                    new() { Title = $"{focusSkill} Practice Test", SkillType = focusSkill.ToLower(), EstimatedMinutes = 45 },
                    new() { Title = "Vocabulary Building", SkillType = "vocabulary", EstimatedMinutes = 20 }
                }
            });
        }

        return plan;
    }

    private static List<AiRecommendationDto> GenerateRecommendations(
        List<UserLearningProgress> progress, 
        List<UserTestAttempt> testAttempts, 
        float targetBand)
    {
        var recommendations = new List<AiRecommendationDto>();

        // Check for incomplete lessons
        var incompleteLessons = progress.Where(p => p.CompletionPercent < 100).Count();
        if (incompleteLessons > 0)
        {
            recommendations.Add(new AiRecommendationDto
            {
                RecommendationType = "Completion",
                Title = "Complete Your Lessons",
                Description = $"You have {incompleteLessons} lessons in progress. Completing them will improve your overall score.",
                Priority = 1
            });
        }

        // Check test performance
        if (testAttempts.Any())
        {
            var avgScore = (float)Math.Round(testAttempts.Where(t => t.BandScore.HasValue).Average(t => t.BandScore ?? 0) * 2, MidpointRounding.AwayFromZero) / 2;
            if (avgScore < targetBand)
            {
                recommendations.Add(new AiRecommendationDto
                {
                    RecommendationType = "Practice",
                    Title = "More Practice Tests Needed",
                    Description = $"Your average test score is {avgScore:F1}. Take more tests to reach your target of {targetBand}.",
                    Priority = 2
                });
            }
        }
        else
        {
            recommendations.Add(new AiRecommendationDto
            {
                RecommendationType = "Assessment",
                Title = "Take a Practice Test",
                Description = "Taking a practice test will help identify your current level and areas for improvement.",
                Priority = 1
            });
        }

        // General recommendations
        recommendations.Add(new AiRecommendationDto
        {
            RecommendationType = "Daily",
            Title = "Daily Vocabulary Practice",
            Description = "Learn 10 new IELTS words daily to expand your vocabulary range.",
            Priority = 3
        });

        return recommendations.OrderBy(r => r.Priority).ToList();
    }

    private static string GenerateBandInsights(float predicted, float rl, float speaking, float writing)
    {
        var insights = new List<string>();
        var skills = new[] { ("Reading/Listening", rl), ("Speaking", speaking), ("Writing", writing) };
        var weakest = skills.OrderBy(s => s.Item2).First();
        var strongest = skills.OrderByDescending(s => s.Item2).First();

        insights.Add($"Your strongest skill is {strongest.Item1} ({strongest.Item2:F1}).");
        insights.Add($"Focus more on {weakest.Item1} ({weakest.Item2:F1}) to improve your overall band.");

        if (predicted >= 7.0f)
            insights.Add("You're on track for a competitive score for most universities!");
        else if (predicted >= 6.0f)
            insights.Add("With consistent practice, you can reach band 7+ in a few months.");
        else
            insights.Add("Daily practice across all skills is essential. Consider structured courses.");

        return string.Join(" ", insights);
    }

    private static string GenerateOverallAssessment(List<UserLearningProgress> progress, AISkillAnalysis? analysis)
    {
        int completedCount = progress.Count(p => p.CompletionPercent >= 100);
        float avgCompletion = progress.Any() ? progress.Average(p => p.CompletionPercent) : 0;

        if (completedCount >= 10 && analysis?.OverallBand >= 6.5f)
            return "Excellent progress! You're on track to achieve your target band score. Keep up the consistent practice.";
        
        if (completedCount >= 5)
            return "Good start! Continue completing lessons regularly to build a strong foundation across all skills.";

        return "You're just getting started. Focus on completing foundational lessons and taking regular practice tests.";
    }

    private static List<string> IdentifyStrengths(AISkillAnalysis? analysis)
    {
        if (analysis == null)
            return new List<string> { "Complete more exercises to identify your strengths" };

        var strengths = new List<string>();
        if (analysis.ReadingScore >= 6.5f) strengths.Add("Strong reading comprehension skills");
        if (analysis.ListeningScore >= 6.5f) strengths.Add("Good listening comprehension");
        if (analysis.WritingScore >= 6.5f) strengths.Add("Solid writing abilities");
        if (analysis.SpeakingScore >= 6.5f) strengths.Add("Confident speaking skills");

        return strengths.Any() ? strengths : new List<string> { "Keep practicing to develop your strengths" };
    }

    private static List<string> IdentifyWeaknesses(AISkillAnalysis? analysis)
    {
        if (analysis == null)
            return new List<string> { "Take placement test to identify areas for improvement" };

        var weaknesses = new List<string>();
        if (analysis.ReadingScore < 6.0f) weaknesses.Add("Reading speed and comprehension needs improvement");
        if (analysis.ListeningScore < 6.0f) weaknesses.Add("Listening for details needs practice");
        if (analysis.WritingScore < 6.0f) weaknesses.Add("Writing structure and coherence needs work");
        if (analysis.SpeakingScore < 6.0f) weaknesses.Add("Speaking fluency and confidence needs development");

        return weaknesses.Any() ? weaknesses : new List<string> { "No major weaknesses identified - maintain your skills!" };
    }

    private static List<string> GenerateStudyTips(AISkillAnalysis? analysis)
    {
        return new List<string>
        {
            "Practice under timed conditions to simulate real exam pressure",
            "Read English news articles daily to improve reading speed",
            "Listen to English podcasts during commute",
            "Record yourself speaking and analyze for improvement",
            "Write at least one essay per week and get feedback",
            "Learn 10-15 new vocabulary words daily with examples"
        };
    }

    private static List<string> GetVocabularyForTopic(string topic, float targetBand)
    {
        var topicVocab = new Dictionary<string, List<string>>
        {
            ["education"] = new() { "curriculum", "pedagogy", "literacy", "vocational", "scholarship", "cognitive", "rote learning", "academic" },
            ["technology"] = new() { "innovation", "automation", "artificial intelligence", "digitalization", "cybersecurity", "obsolete", "cutting-edge" },
            ["environment"] = new() { "sustainability", "biodiversity", "carbon footprint", "renewable", "conservation", "ecosystem", "pollution" },
            ["health"] = new() { "epidemic", "prevention", "nutrition", "sedentary", "well-being", "diagnosis", "chronic", "obesity" },
            ["society"] = new() { "inequality", "demographics", "urbanization", "globalization", "integration", "discrimination", "diversity" }
        };

        var lowerTopic = topic.ToLower();
        foreach (var kvp in topicVocab)
        {
            if (lowerTopic.Contains(kvp.Key))
                return kvp.Value;
        }

        // Default academic vocabulary
        return new List<string> { "significant", "fundamental", "consequently", "furthermore", "nevertheless", "predominantly", "substantial" };
    }

    #endregion
}
