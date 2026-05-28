using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebIeltsFree.Models;
using WebIeltsFree.Services;

namespace WebIeltsFree.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IGeminiService _gemini;

    public TestsController(AppDbContext context, IGeminiService gemini)
    {
        _context = context;
        _gemini = gemini;
    }

    /// <summary>
    /// Get all available tests
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TestSummaryDto>>>> GetTests()
    {
        var userId = GetCurrentUserId();

        var tests = await _context.Tests
            .Where(t => !t.IsDeleted && (!t.IsTeacherCreated || t.Status == "published"))
            .Include(t => t.Sections)
            .Select(t => new TestSummaryDto
            {
                TestId = t.TestId,
                Title = t.Title,
                Difficulty = t.Difficulty,
                DurationMinutes = t.DurationMinutes,
                SkillType = t.Sections.FirstOrDefault() != null ? t.Sections.First().SkillType : null,
                HasAttempted = _context.UserTestAttempts.Any(a => a.UserId == userId && a.TestId == t.TestId),
                BestScore = _context.UserTestAttempts
                    .Where(a => a.UserId == userId && a.TestId == t.TestId)
                    .Max(a => (float?)a.BandScore)
            })
            .ToListAsync();

        return Ok(ApiResponse<List<TestSummaryDto>>.Ok(tests));
    }

    /// <summary>
    /// Get test details with questions
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TestDetailDto>>> GetTest(int id)
    {
        var test = await _context.Tests
            .Include(t => t.Sections)
                .ThenInclude(s => s.Questions)
            .FirstOrDefaultAsync(t => t.TestId == id);

        if (test == null || test.IsDeleted)
            return NotFound(ApiResponse<TestDetailDto>.Fail("Test not found"));

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (test.IsTeacherCreated && test.Status != "published" && userRole != "teacher" && userRole != "admin")
        {
            return StatusCode(403, ApiResponse<TestDetailDto>.Fail("This test has not been published yet."));
        }

        var fallbackAudio = await _context.ListeningMaterials
            .Where(m => m.AudioUrl != null)
            .OrderByDescending(m => m.MaterialId)
            .Select(m => m.AudioUrl)
            .FirstOrDefaultAsync();

        var testDto = new TestDetailDto
        {
            TestId = test.TestId,
            Title = test.Title,
            DurationMinutes = test.DurationMinutes,
            Sections = test.Sections.Select(s => new TestSectionDto
            {
                SectionId = s.SectionId,
                SkillType = s.SkillType,
                AudioUrl = s.AudioUrl ?? (s.SkillType == "listening" ? fallbackAudio : null),
                Questions = s.Questions.Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Difficulty = q.Difficulty
                }).ToList()
            }).ToList()
        };

        return Ok(ApiResponse<TestDetailDto>.Ok(testDto));
    }

    /// <summary>
    /// Submit test answers and get band score
    /// </summary>
    [HttpPost("submit")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<TestResultDto>>> SubmitTest([FromBody] SubmitTestRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<TestResultDto>.Fail("Not authenticated"));

        var test = await _context.Tests
            .Include(t => t.Sections)
                .ThenInclude(s => s.Questions)
                    .ThenInclude(q => q.Answer)
            .FirstOrDefaultAsync(t => t.TestId == request.TestId);

        if (test == null || test.IsDeleted)
            return NotFound(ApiResponse<TestResultDto>.Fail("Test not found"));

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (test.IsTeacherCreated && test.Status != "published" && userRole != "teacher" && userRole != "admin")
        {
            return StatusCode(403, ApiResponse<TestResultDto>.Fail("This test has not been published yet."));
        }

        // Create attempt
        var attempt = new UserTestAttempt
        {
            UserId = userId,
            TestId = request.TestId,
            StartedAt = DateTime.UtcNow
        };
        _context.UserTestAttempts.Add(attempt);
        await _context.SaveChangesAsync();

        // Process answers
        var answerResults = new List<AnswerResultDto>();
        int correctCount = 0;
        int totalQuestions = 0;

        foreach (var section in test.Sections)
        {
            foreach (var question in section.Questions)
            {
                totalQuestions++;
                var userAnswerText = request.Answers
                    .FirstOrDefault(a => a.QuestionId == question.QuestionId)?.Answer ?? "";

                var isCorrect = question.Answer?.CorrectAnswer?.Trim().ToLower() == userAnswerText.Trim().ToLower();
                if (isCorrect) correctCount++;

                // Save user answer
                var userAnswer = new UserAnswer
                {
                    AttemptId = attempt.AttemptId,
                    QuestionId = question.QuestionId,
                    UserAnswerText = userAnswerText,
                    IsCorrect = isCorrect
                };
                _context.UserAnswers.Add(userAnswer);

                answerResults.Add(new AnswerResultDto
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.QuestionText,
                    UserAnswer = userAnswerText,
                    CorrectAnswer = question.Answer?.CorrectAnswer,
                    IsCorrect = isCorrect
                });
            }
        }

        // Calculate band score (0-9 scale based on correct percentage)
        float bandScore = CalculateBandScore(correctCount, totalQuestions);
        attempt.BandScore = bandScore;
        attempt.FinishedAt = DateTime.UtcNow;

        // Sync with Teacher Assignment if exists
        var assignment = await _context.TeacherAssignments
            .FirstOrDefaultAsync(a => a.StudentId == userId 
                                   && a.TestId == request.TestId 
                                   && (a.Status == "pending" || a.Status == "overdue")
                                   && !a.IsDeleted);
        if (assignment != null)
        {
            assignment.Status = "completed";
            assignment.CompletedAt = DateTime.UtcNow;
            assignment.AttemptId = attempt.AttemptId;
        }

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<TestResultDto>.Ok(new TestResultDto
        {
            AttemptId = attempt.AttemptId,
            TestId = test.TestId,
            TestTitle = test.Title,
            BandScore = bandScore,
            CorrectAnswers = correctCount,
            TotalQuestions = totalQuestions,
            StartedAt = attempt.StartedAt,
            FinishedAt = attempt.FinishedAt,
            XPEarned = correctCount * 10 + 50,
            AnswerResults = answerResults
        }));
    }

    /// <summary>
    /// Get test history
    /// </summary>
    [HttpGet("history")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<TestHistoryDto>>>> GetTestHistory()
    {
        var userId = GetCurrentUserId();

        var history = await _context.UserTestAttempts
            .Where(a => a.UserId == userId)
            .Include(a => a.Test)
            .OrderByDescending(a => a.StartedAt)
            .Select(a => new TestHistoryDto
            {
                AttemptId = a.AttemptId,
                TestTitle = a.Test != null ? a.Test.Title : null,
                BandScore = a.BandScore,
                StartedAt = a.StartedAt,
                FinishedAt = a.FinishedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<TestHistoryDto>>.Ok(history));
    }

    /// <summary>
    /// Get current user's teacher assignments
    /// </summary>
    [HttpGet("assignments")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<TeacherAssignmentDto>>>> GetAssignments()
    {
        var userId = GetCurrentUserId();
        var assignments = await _context.TeacherAssignments
            .Include(a => a.Test)
            .Include(a => a.Teacher)
                .ThenInclude(t => t.Profile)
            .Where(a => a.StudentId == userId && !a.IsDeleted)
            .OrderByDescending(a => a.AssignedAt)
            .Select(a => new TeacherAssignmentDto
            {
                AssignmentId = a.AssignmentId,
                TestId = a.TestId,
                Title = a.Title,
                Instructions = a.Instructions,
                Deadline = a.Deadline,
                Status = a.Status,
                AssignedAt = a.AssignedAt,
                CompletedAt = a.CompletedAt,
                TeacherName = a.Teacher != null && a.Teacher.Profile != null ? a.Teacher.Profile.FullName : "Teacher",
                DurationMinutes = a.Test != null ? a.Test.DurationMinutes : 60,
                AttemptId = a.AttemptId
            })
            .ToListAsync();

        return Ok(ApiResponse<List<TeacherAssignmentDto>>.Ok(assignments));
    }

    /// <summary>
    /// <summary>
    /// Get placement test (adaptive diagnostic)
    /// </summary>
    [HttpGet("placement")]
    public async Task<ActionResult<ApiResponse<TestDetailDto>>> GetPlacementTest()
    {
        var targetSkills = new[] { "listening", "reading", "writing", "speaking" };
        var sectionDtos = new List<TestSectionDto>();

        // Fallback audio: prefer actual Cambridge material, else any with audio
        var fallbackAudio = await _context.ListeningMaterials
            .Where(m => m.AudioUrl != null && m.AudioUrl.StartsWith("/audio/"))
            .OrderByDescending(m => m.MaterialId)
            .Select(m => m.AudioUrl)
            .FirstOrDefaultAsync()
            ?? await _context.ListeningMaterials
                .Where(m => m.AudioUrl != null)
                .Select(m => m.AudioUrl)
                .FirstOrDefaultAsync();

        foreach (var skill in targetSkills)
        {
            var section = await _context.TestSections
                .Where(s => s.SkillType == skill)
                .Include(s => s.Questions)
                .OrderByDescending(s => s.Questions.Count)
                .ThenByDescending(s => s.SectionId)
                .FirstOrDefaultAsync();

            if (section == null) continue;

            // Determine question count limit based on skill to keep test under 30 minutes
            int questionLimit = skill switch
            {
                "listening" => 5,
                "reading" => 5,
                "writing" => 1,
                "speaking" => 1,
                _ => 5
            };

            var questions = section.Questions
                .OrderBy(q => q.QuestionId)
                .Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Difficulty = q.Difficulty
                })
                .Take(questionLimit)
                .ToList();

            sectionDtos.Add(new TestSectionDto
            {
                SectionId = section.SectionId,
                SkillType = section.SkillType,
                AudioUrl = section.AudioUrl ?? (skill == "listening" ? fallbackAudio : null),
                Questions = questions
            });
        }

        var placementTest = new TestDetailDto
        {
            TestId = 0, // Special ID for placement test
            Title = "IELTS Short Mock Placement (Academic)",
            DurationMinutes = 30,
            Sections = sectionDtos
        };

        return Ok(ApiResponse<TestDetailDto>.Ok(placementTest));
    }

    /// <summary>
    /// Submit placement test and get initial band score
    /// </summary>
    [HttpPost("placement/submit")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<TestResultDto>>> SubmitPlacementTest([FromBody] SubmitTestRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<TestResultDto>.Fail("Not authenticated"));

        var questions = await _context.Questions
            .Include(q => q.Answer)
            .Include(q => q.Section)
            .Where(q => request.Answers.Select(a => a.QuestionId).Contains(q.QuestionId))
            .ToListAsync();

        int correctCount = 0;
        var answerResults = new List<AnswerResultDto>();
        var skillStats = new Dictionary<string, (int Correct, int Total, List<string> WrongItems)>(StringComparer.OrdinalIgnoreCase);

        foreach (var question in questions)
        {
            var userAnswerText = request.Answers
                .FirstOrDefault(a => a.QuestionId == question.QuestionId)?.Answer ?? "";

            var isCorrect = question.Answer?.CorrectAnswer?.Trim().ToLower() == userAnswerText.Trim().ToLower();
            if (isCorrect) correctCount++;
            var skill = question.Section?.SkillType ?? "mixed";
            if (!skillStats.TryGetValue(skill, out var stat))
            {
                stat = (0, 0, new List<string>());
            }
            stat.Total += 1;
            if (isCorrect) stat.Correct += 1;
            else stat.WrongItems.Add($"Q{question.QuestionId}: {question.QuestionText} | User: {userAnswerText} | Correct: {question.Answer?.CorrectAnswer}");
            skillStats[skill] = stat;

            answerResults.Add(new AnswerResultDto
            {
                QuestionId = question.QuestionId,
                QuestionText = question.QuestionText,
                UserAnswer = userAnswerText,
                CorrectAnswer = question.Answer?.CorrectAnswer,
                IsCorrect = isCorrect
            });
        }

        var readingBand = GetSkillBand(skillStats, "reading");
        var listeningBand = GetSkillBand(skillStats, "listening");
        var writingBand = 0.0f;
        var speakingBand = 0.0f;

        var writingQuestion = questions.FirstOrDefault(q => q.Section?.SkillType?.ToLower() == "writing");
        if (writingQuestion != null)
        {
            var userAnswer = request.Answers.FirstOrDefault(a => a.QuestionId == writingQuestion.QuestionId)?.Answer ?? "";
            if (!string.IsNullOrWhiteSpace(userAnswer))
            {
                writingBand = await GradeOpenEndedSectionAsync(writingQuestion.QuestionText ?? "IELTS Writing Prompt", userAnswer, "Writing");
            }
        }

        var speakingQuestion = questions.FirstOrDefault(q => q.Section?.SkillType?.ToLower() == "speaking");
        if (speakingQuestion != null)
        {
            var userAnswer = request.Answers.FirstOrDefault(a => a.QuestionId == speakingQuestion.QuestionId)?.Answer ?? "";
            if (!string.IsNullOrWhiteSpace(userAnswer))
            {
                speakingBand = await GradeOpenEndedSectionAsync(speakingQuestion.QuestionText ?? "IELTS Speaking Prompt", userAnswer, "Speaking");
            }
        }

        // Add dummy correct stats to skillStats for writing/speaking if we graded them, so they show up as non-zero in recommendations
        if (writingQuestion != null && writingBand > 0)
        {
            var stat = (Correct: (int)Math.Round(writingBand), Total: 9, WrongItems: new List<string>());
            skillStats["writing"] = stat;
        }
        if (speakingQuestion != null && speakingBand > 0)
        {
            var stat = (Correct: (int)Math.Round(speakingBand), Total: 9, WrongItems: new List<string>());
            skillStats["speaking"] = stat;
        }

        var availableBands = new[] { readingBand, listeningBand, writingBand, speakingBand }
            .Where(b => b > 0)
            .ToList();
        var bandScore = availableBands.Any()
            ? RoundToIeltsBand(availableBands.Average())
            : CalculateBandScore(correctCount, questions.Count);

        // Update user's current band
        var userGoal = await _context.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);
        if (userGoal != null)
        {
            userGoal.CurrentBand = bandScore;
        }

        await UpsertPlacementAndAiAnalysis(userId, bandScore, readingBand, listeningBand, writingBand, speakingBand, skillStats);
        await UpsertRoadmapFromWeakness(userId, bandScore, skillStats);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<TestResultDto>.Ok(new TestResultDto
        {
            AttemptId = 0,
            TestId = 0,
            TestTitle = "IELTS Placement Test",
            BandScore = bandScore,
            CorrectAnswers = correctCount,
            TotalQuestions = questions.Count,
            StartedAt = DateTime.UtcNow,
            FinishedAt = DateTime.UtcNow,
            XPEarned = 100,
            AnswerResults = answerResults
        }));
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    private static float GetSkillBand(
        IDictionary<string, (int Correct, int Total, List<string> WrongItems)> skillStats,
        string skill)
    {
        if (!skillStats.TryGetValue(skill, out var stat) || stat.Total == 0) return 0;
        return CalculateBandScore(stat.Correct, stat.Total);
    }

    private async Task UpsertPlacementAndAiAnalysis(
        int userId,
        float overallBand,
        float readingBand,
        float listeningBand,
        float writingBand,
        float speakingBand,
        IDictionary<string, (int Correct, int Total, List<string> WrongItems)> skillStats)
    {
        var placement = new UserPlacementResult
        {
            UserId = userId,
            TestId = null,
            ReadingScore = skillStats.TryGetValue("reading", out var r) ? r.Correct : null,
            ListeningScore = skillStats.TryGetValue("listening", out var l) ? l.Correct : null,
            WritingScore = skillStats.TryGetValue("writing", out var w) ? w.Correct : null,
            SpeakingScore = skillStats.TryGetValue("speaking", out var s) ? s.Correct : null,
            OverallBand = (decimal)overallBand,
            Recommendations = BuildRecommendationText(skillStats)
        };
        _context.UserPlacementResults.Add(placement);

        var weakSkills = skillStats
            .Where(kv => kv.Value.Total > 0 && (float)kv.Value.Correct / kv.Value.Total < 0.6f)
            .Select(kv => kv.Key)
            .ToList();

        var strengths = string.Join(", ", skillStats
            .Where(kv => kv.Value.Total > 0 && (float)kv.Value.Correct / kv.Value.Total >= 0.75f)
            .Select(kv => kv.Key));
        if (string.IsNullOrWhiteSpace(strengths)) strengths = "No dominant skill yet";

        var weaknesses = weakSkills.Any() ? string.Join(", ", weakSkills) : "No critical weakness";

        var aiRecommendations = BuildRecommendationText(skillStats);
        var wrongSummary = string.Join("\n", skillStats.Values.SelectMany(v => v.WrongItems).Take(20));

        if (!string.IsNullOrWhiteSpace(wrongSummary))
        {
            var prompt = $"""
            You are an IELTS diagnostic engine.
            Analyze this placement test summary and produce concise actionable recommendations.
            Return plain text, max 8 lines.

            Bands:
            Reading: {readingBand}
            Listening: {listeningBand}
            Writing: {writingBand}
            Speaking: {speakingBand}
            Overall: {overallBand}

            Wrong answers:
            {wrongSummary}
            """;
            aiRecommendations = await _gemini.GenerateContentAsync(prompt, "Output practical IELTS coaching actions only.");
        }

        _context.AISkillAnalyses.Add(new AISkillAnalysis
        {
            UserId = userId,
            ReadingScore = readingBand,
            ListeningScore = listeningBand,
            WritingScore = writingBand,
            SpeakingScore = speakingBand,
            Strengths = strengths,
            Weaknesses = weaknesses,
            Recommendations = aiRecommendations,
            AnalyzedAt = DateTime.UtcNow
        });
    }

    private async Task UpsertRoadmapFromWeakness(
        int userId,
        float currentBand,
        IDictionary<string, (int Correct, int Total, List<string> WrongItems)> skillStats)
    {
        var targetBand = await _context.UserGoals
            .Where(g => g.UserId == userId)
            .Select(g => (float?)g.TargetBand)
            .FirstOrDefaultAsync() ?? 7.0f;

        var weakSkills = skillStats
            .Where(kv => kv.Value.Total > 0 && (float)kv.Value.Correct / kv.Value.Total < 0.65f)
            .OrderBy(kv => (float)kv.Value.Correct / kv.Value.Total)
            .Select(kv => kv.Key)
            .ToList();
        if (!weakSkills.Any())
        {
            weakSkills = new List<string> { "writing", "speaking", "reading", "listening" };
        }

        var estimatedWeeks = Math.Max(4, (int)Math.Ceiling((targetBand - currentBand) * 8));
        var roadmap = new AIRoadmap
        {
            UserId = userId,
            TargetBand = targetBand,
            EstimatedWeeks = estimatedWeeks,
            GeneratedAt = DateTime.UtcNow
        };
        _context.AIRoadmaps.Add(roadmap);
        await _context.SaveChangesAsync();

        var lessons = await _context.Lessons
            .Where(l => l.SkillType != null && weakSkills.Contains(l.SkillType))
            .OrderBy(l => l.DifficultyLevel)
            .Take(16)
            .ToListAsync();

        var stepIndex = 0;
        foreach (var lesson in lessons)
        {
            _context.AIRoadmapSteps.Add(new AIRoadmapStep
            {
                RoadmapId = roadmap.RoadmapId,
                LessonId = lesson.LessonId,
                WeekNumber = (stepIndex / 2) + 1,
                Priority = stepIndex + 1,
                IsCompleted = false
            });
            stepIndex++;
        }
    }

    private static string BuildRecommendationText(
        IDictionary<string, (int Correct, int Total, List<string> WrongItems)> skillStats)
    {
        var recs = new List<string>();
        foreach (var (skill, stat) in skillStats)
        {
            if (stat.Total == 0) continue;
            var accuracy = (float)stat.Correct / stat.Total;
            if (accuracy < 0.6f)
            {
                recs.Add($"{skill}: ưu tiên luyện nền tảng theo dạng câu hỏi sai nhiều.");
            }
            else if (accuracy < 0.75f)
            {
                recs.Add($"{skill}: cần tăng tốc độ và độ chính xác dưới áp lực thời gian.");
            }
            else
            {
                recs.Add($"{skill}: duy trì bằng bài mock định kỳ và nâng độ khó.");
            }
        }
        return string.Join(" ", recs);
    }

    /// <summary>
    /// Calculate IELTS band score (0-9) based on correct answers percentage
    /// </summary>
    private static float CalculateBandScore(int correct, int total)
    {
        if (total == 0) return 0;

        float percentage = (float)correct / total * 100;

        // IELTS-style band score calculation
        return percentage switch
        {
            >= 90 => 9.0f,
            >= 85 => 8.5f,
            >= 80 => 8.0f,
            >= 75 => 7.5f,
            >= 70 => 7.0f,
            >= 65 => 6.5f,
            >= 60 => 6.0f,
            >= 55 => 5.5f,
            >= 50 => 5.0f,
            >= 45 => 4.5f,
            >= 40 => 4.0f,
            >= 35 => 3.5f,
            >= 30 => 3.0f,
            >= 25 => 2.5f,
            >= 20 => 2.0f,
            >= 15 => 1.5f,
            >= 10 => 1.0f,
            _ => 0.5f
        };
    }

    private static float RoundToIeltsBand(float score)
    {
        float decimalPart = score - (int)score;
        if (decimalPart < 0.25f)
            return (int)score;
        else if (decimalPart < 0.75f)
            return (int)score + 0.5f;
        else
            return (int)score + 1.0f;
    }

    private async Task<float> GradeOpenEndedSectionAsync(string promptText, string userResponse, string skill)
    {
        try
        {
            var aiPrompt = $$"""
            You are an official IELTS {{skill}} examiner.
            Evaluate the following student response for a placement test.
            
            Prompt:
            {{promptText}}
            
            Student Response/Transcript:
            {{userResponse}}
            
            Determine the IELTS band score (between 1.0 and 9.0, rounded to the nearest 0.5) according to standard criteria.
            Response format: Return ONLY a JSON object containing the float value 'band' and a brief 'feedback' string.
            Example:
            { "band": 5.5, "feedback": "Good coherence but vocabulary is limited." }
            """;

            var resultText = await _gemini.GenerateContentAsync(aiPrompt, "Output JSON format only.");
            var match = System.Text.RegularExpressions.Regex.Match(resultText, @"""band""\s*:\s*([0-9.]+)");
            if (match.Success && float.TryParse(match.Groups[1].Value, out var band))
            {
                return RoundToIeltsBand(band);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error grading open ended {skill}: " + ex.Message);
        }
        return 5.0f; // Default fallback band
    }

    #endregion
}
