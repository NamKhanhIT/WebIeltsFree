using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReadingController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReadingController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get reading practice passage by ID without answers
    /// </summary>
    [HttpGet("passage/{passageId}")]
    public async Task<ActionResult<ApiResponse<ReadingPracticeSetDto>>> GetPassage(int passageId)
    {
        var passage = await _context.ReadingPassages
            .Include(p => p.Questions)
            .FirstOrDefaultAsync(p => p.PassageId == passageId);

        if (passage == null)
            return NotFound(ApiResponse<ReadingPracticeSetDto>.Fail("Passage not found"));

        var dto = new ReadingPracticeSetDto
        {
            Passage = new ReadingPassageDto
            {
                PassageId = passage.PassageId,
                PassageTitle = passage.PassageTitle,
                PassageText = passage.PassageText,
                WordCount = passage.WordCount,
                DifficultyLevel = passage.DifficultyLevel,
                TopicCategory = passage.TopicCategory,
                Source = passage.Source,
                ImageUrl = passage.ImageUrl
            },
            Questions = passage.Questions
                .OrderBy(q => q.QuestionNumber)
                .Select(q => new ReadingQuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionNumber = q.QuestionNumber,
                    QuestionType = q.QuestionType,
                    QuestionText = q.QuestionText,
                    Options = q.Options != null ? JsonSerializer.Deserialize<List<string>>(q.Options) : null,
                    ParagraphReference = q.ParagraphReference
                })
                .ToList()
        };

        return Ok(ApiResponse<ReadingPracticeSetDto>.Ok(dto));
    }

    /// <summary>
    /// Get reading practice set by difficulty level
    /// </summary>
    [HttpGet("practice/{difficulty}")]
    public async Task<ActionResult<ApiResponse<ReadingPracticeSetDto>>> GetPracticeSet(string difficulty)
    {
        var validDifficulties = new[] { "band_3_4", "band_5_6", "band_7_8", "band_8_9" };
        if (!validDifficulties.Contains(difficulty.ToLower()))
            return BadRequest(ApiResponse<ReadingPracticeSetDto>.Fail("Invalid difficulty level"));

        var passage = await _context.ReadingPassages
            .Include(p => p.Questions)
            .Where(p => p.DifficultyLevel.ToLower() == difficulty.ToLower())
            .OrderBy(p => Guid.NewGuid())
            .FirstOrDefaultAsync();

        if (passage == null)
            return NotFound(ApiResponse<ReadingPracticeSetDto>.Fail($"No practice passages available for {difficulty}"));

        var dto = new ReadingPracticeSetDto
        {
            Passage = new ReadingPassageDto
            {
                PassageId = passage.PassageId,
                PassageTitle = passage.PassageTitle,
                PassageText = passage.PassageText,
                WordCount = passage.WordCount,
                DifficultyLevel = passage.DifficultyLevel,
                TopicCategory = passage.TopicCategory,
                Source = passage.Source,
                ImageUrl = passage.ImageUrl
            },
            Questions = passage.Questions
                .OrderBy(q => q.QuestionNumber)
                .Select(q => new ReadingQuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionNumber = q.QuestionNumber,
                    QuestionType = q.QuestionType,
                    QuestionText = q.QuestionText,
                    Options = q.Options != null ? JsonSerializer.Deserialize<List<string>>(q.Options) : null,
                    ParagraphReference = q.ParagraphReference
                })
                .ToList()
        };

        return Ok(ApiResponse<ReadingPracticeSetDto>.Ok(dto));
    }

    /// <summary>
    /// List all available reading passages
    /// </summary>
    [HttpGet("passages")]
    public async Task<ActionResult<ApiResponse<List<ReadingPassageDto>>>> ListPassages([FromQuery] string? difficulty = null)
    {
        var query = _context.ReadingPassages.AsQueryable();

        if (!string.IsNullOrWhiteSpace(difficulty))
            query = query.Where(p => p.DifficultyLevel.ToLower() == difficulty.ToLower());

        var passages = await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ReadingPassageDto
            {
                PassageId = p.PassageId,
                PassageTitle = p.PassageTitle,
                PassageText = p.PassageText,
                WordCount = p.WordCount,
                DifficultyLevel = p.DifficultyLevel,
                TopicCategory = p.TopicCategory,
                Source = p.Source,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();

        return Ok(ApiResponse<List<ReadingPassageDto>>.Ok(passages));
    }

    /// <summary>
    /// Submit reading answer and get immediate feedback
    /// Supports all 5 IELTS question types:
    /// 1. True/False/Not Given
    /// 2. Multiple Choice
    /// 3. Matching Headings
    /// 4. Sentence Completion
    /// 5. Summary Completion (word bank)
    /// </summary>
    [HttpPost("submit")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ReadingAnswerResultDto>>> SubmitAnswer([FromBody] SubmitReadingAnswerRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<ReadingAnswerResultDto>.Fail("Not authenticated"));

        var question = await _context.ReadingQuestions
            .FirstOrDefaultAsync(q => q.QuestionId == request.QuestionId);

        if (question == null)
            return NotFound(ApiResponse<ReadingAnswerResultDto>.Fail("Question not found"));

        var isCorrect = ValidateReadingAnswer(question, request.UserAnswer);

        // Record attempt
        var attempt = new UserPracticeAttempt
        {
            UserId = userId,
            SkillType = "reading",
            ReadingQuestionId = question.QuestionId,
            UserAnswer = request.UserAnswer,
            IsCorrect = isCorrect,
            TimeSpentSeconds = request.TimeSpentSeconds,
            AttemptDate = DateTime.UtcNow
        };

        _context.UserPracticeAttempts.Add(attempt);
        await _context.SaveChangesAsync();

        var result = new ReadingAnswerResultDto
        {
            QuestionId = question.QuestionId,
            QuestionNumber = question.QuestionNumber,
            IsCorrect = isCorrect,
            UserAnswer = request.UserAnswer,
            CorrectAnswer = question.CorrectAnswer,
            Explanation = question.Explanation,
            BandTarget = question.BandTarget,
            QuestionType = question.QuestionType
        };

        return Ok(ApiResponse<ReadingAnswerResultDto>.Ok(result));
    }

    /// <summary>
    /// Validate reading answer based on question type (supports 5 IELTS types)
    /// </summary>
    private bool ValidateReadingAnswer(ReadingQuestion question, string userAnswer)
    {
        if (string.IsNullOrWhiteSpace(userAnswer) || string.IsNullOrWhiteSpace(question.CorrectAnswer))
            return false;

        var userAns = userAnswer.Trim();
        var correctAns = question.CorrectAnswer.Trim();
        var qType = question.QuestionType.ToLower();

        return qType switch
        {
            "true_false_not_given" => ValidateTrueFalseNotGiven(userAns, correctAns),
            "multiple_choice" => ValidateMultipleChoice(userAns, correctAns),
            "matching_heading" => ValidateMatchingHeading(userAns, correctAns),
            "sentence_completion" => ValidateSentenceCompletion(userAns, correctAns),
            "summary_completion" => ValidateSummaryCompletion(userAns, correctAns),
            _ => userAns.Equals(correctAns, StringComparison.OrdinalIgnoreCase)
        };
    }

    private bool ValidateTrueFalseNotGiven(string userAns, string correctAns)
    {
        // Exact match for True, False, Not Given
        var valid = new[] { "true", "false", "not given" };
        var normalized = userAns.ToLower();
        return valid.Contains(normalized) && normalized == correctAns.ToLower();
    }

    private bool ValidateMultipleChoice(string userAns, string correctAns)
    {
        // Case-insensitive match for option letter (A, B, C, D)
        return userAns.ToUpper() == correctAns.ToUpper();
    }

    private bool ValidateMatchingHeading(string userAns, string correctAns)
    {
        // Can be multiple answers separated by comma or semicolon
        var userAnswers = userAns.Split(',', ';').Select(a => a.Trim().ToUpper()).ToList();
        var correctAnswers = correctAns.Split(',', ';').Select(a => a.Trim().ToUpper()).ToList();

        // Check if all correct answers are provided
        return correctAnswers.All(ca => userAnswers.Contains(ca)) &&
               userAnswers.All(ua => correctAnswers.Contains(ua));
    }

    private bool ValidateSentenceCompletion(string userAns, string correctAns)
    {
        // Case-insensitive, whitespace-trimmed, allow synonyms
        // Accept alternative answers separated by comma
        var alternatives = correctAns.Split(',').Select(a => a.Trim().ToLower());
        return alternatives.Any(alt => userAns.ToLower() == alt);
    }

    private bool ValidateSummaryCompletion(string userAns, string correctAns)
    {
        // Exact match from word bank (case-insensitive)
        // May allow multiple valid answers separated by comma
        var alternatives = correctAns.Split(',').Select(a => a.Trim().ToLower());
        return alternatives.Any(alt => userAns.ToLower() == alt);
    }

    /// <summary>
    /// Get user's reading practice history
    /// </summary>
    [HttpGet("history")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<ReadingAnswerResultDto>>>> GetHistory([FromQuery] int limit = 20)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<List<ReadingAnswerResultDto>>.Fail("Not authenticated"));

        var attempts = await _context.UserPracticeAttempts
            .Where(a => a.UserId == userId && a.SkillType == "reading")
            .Include(a => a.ReadingQuestion)
            .OrderByDescending(a => a.AttemptDate)
            .Take(limit)
            .Select(a => new ReadingAnswerResultDto
            {
                QuestionId = a.ReadingQuestion!.QuestionId,
                QuestionNumber = a.ReadingQuestion.QuestionNumber,
                IsCorrect = a.IsCorrect,
                UserAnswer = a.UserAnswer ?? string.Empty,
                CorrectAnswer = a.ReadingQuestion.CorrectAnswer,
                Explanation = a.ReadingQuestion.Explanation,
                BandTarget = a.ReadingQuestion.BandTarget,
                QuestionType = a.ReadingQuestion.QuestionType
            })
            .ToListAsync();

        return Ok(ApiResponse<List<ReadingAnswerResultDto>>.Ok(attempts));
    }

    /// <summary>
    /// Get reading practice statistics
    /// </summary>
    [HttpGet("stats")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PracticeSessionSummaryDto>>> GetStats()
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<PracticeSessionSummaryDto>.Fail("Not authenticated"));

        var attempts = await _context.UserPracticeAttempts
            .Where(a => a.UserId == userId && a.SkillType == "reading")
            .Include(a => a.ReadingQuestion)
            .ToListAsync();

        if (!attempts.Any())
            return Ok(ApiResponse<PracticeSessionSummaryDto>.Ok(new PracticeSessionSummaryDto
            {
                SkillType = "reading",
                TotalQuestions = 0,
                CorrectAnswers = 0,
                AccuracyPercent = 0,
                TotalTimeSeconds = 0
            }));

        var totalCorrect = attempts.Count(a => a.IsCorrect);
        var totalTime = attempts.Sum(a => a.TimeSpentSeconds ?? 0);
        var avgBand = attempts.Average(a => (float?)a.ReadingQuestion?.BandTarget) ?? 0;

        var summary = new PracticeSessionSummaryDto
        {
            SkillType = "reading",
            TotalQuestions = attempts.Count,
            CorrectAnswers = totalCorrect,
            AccuracyPercent = (float)(totalCorrect * 100.0 / attempts.Count),
            AverageBandTarget = avgBand,
            TotalTimeSeconds = totalTime
        };

        return Ok(ApiResponse<PracticeSessionSummaryDto>.Ok(summary));
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim?.Value, out var userId) ? userId : 0;
    }

    #region Teacher Content CRUD

    /// <summary>
    /// Teacher: Create a reading passage
    /// </summary>
    [HttpPost("passages")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<ReadingPassageDto>>> CreatePassage([FromBody] CreateReadingPassageRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<ReadingPassageDto>.Fail("Invalid request"));

        var passage = new ReadingPassage
        {
            PassageTitle = request.PassageTitle,
            PassageText = request.PassageText,
            WordCount = request.PassageText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
            DifficultyLevel = request.DifficultyLevel,
            TopicCategory = request.TopicCategory,
            Source = request.Source,
            CreatedAt = DateTime.UtcNow
        };

        _context.ReadingPassages.Add(passage);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "create_reading_passage", "ReadingPassage", passage.PassageId);

        return Ok(ApiResponse<ReadingPassageDto>.Ok(new ReadingPassageDto
        {
            PassageId = passage.PassageId,
            PassageTitle = passage.PassageTitle,
            PassageText = passage.PassageText,
            WordCount = passage.WordCount,
            DifficultyLevel = passage.DifficultyLevel,
            TopicCategory = passage.TopicCategory,
            Source = passage.Source
        }, "Reading passage created"));
    }

    /// <summary>
    /// Teacher: Update a reading passage
    /// </summary>
    [HttpPut("passages/{id}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdatePassage(int id, [FromBody] CreateReadingPassageRequest request)
    {
        var passage = await _context.ReadingPassages.FindAsync(id);
        if (passage == null)
            return NotFound(ApiResponse<bool>.Fail("Passage not found"));

        passage.PassageTitle = request.PassageTitle;
        passage.PassageText = request.PassageText;
        passage.WordCount = request.PassageText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        passage.DifficultyLevel = request.DifficultyLevel;
        passage.TopicCategory = request.TopicCategory;
        passage.Source = request.Source;

        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "update_reading_passage", "ReadingPassage", id);

        return Ok(ApiResponse<bool>.Ok(true, "Reading passage updated"));
    }

    /// <summary>
    /// Teacher: Soft-delete a reading passage
    /// </summary>
    [HttpDelete("passages/{id}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePassage(int id)
    {
        var passage = await _context.ReadingPassages.FindAsync(id);
        if (passage == null)
            return NotFound(ApiResponse<bool>.Fail("Passage not found"));

        // ReadingPassage doesn't have IsDeleted yet — remove from DB
        // For teachers, we'll archive by removing lesson link
        _context.ReadingPassages.Remove(passage);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "delete_reading_passage", "ReadingPassage", id);

        return Ok(ApiResponse<bool>.Ok(true, "Reading passage deleted"));
    }

    /// <summary>
    /// Teacher: Add a question to a passage
    /// </summary>
    [HttpPost("passages/{passageId}/questions")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> CreateQuestion(int passageId, [FromBody] CreateReadingQuestionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<bool>.Fail("Invalid request"));

        var passage = await _context.ReadingPassages.FindAsync(passageId);
        if (passage == null)
            return NotFound(ApiResponse<bool>.Fail("Passage not found"));

        var question = new ReadingQuestion
        {
            PassageId = passageId,
            QuestionType = request.QuestionType,
            QuestionNumber = request.QuestionNumber,
            QuestionText = request.QuestionText,
            CorrectAnswer = request.CorrectAnswer,
            Options = request.Options,
            Explanation = request.Explanation,
            BandTarget = request.BandTarget,
            ParagraphReference = request.ParagraphReference,
            CreatedAt = DateTime.UtcNow
        };

        _context.ReadingQuestions.Add(question);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "create_reading_question", "ReadingQuestion", question.QuestionId);

        return Ok(ApiResponse<bool>.Ok(true, "Reading question added"));
    }

    /// <summary>
    /// Teacher: Update a reading question
    /// </summary>
    [HttpPut("passages/{passageId}/questions/{questionId}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateQuestion(int passageId, int questionId, [FromBody] CreateReadingQuestionRequest request)
    {
        var question = await _context.ReadingQuestions
            .FirstOrDefaultAsync(q => q.QuestionId == questionId && q.PassageId == passageId);
        if (question == null)
            return NotFound(ApiResponse<bool>.Fail("Question not found"));

        question.QuestionType = request.QuestionType;
        question.QuestionNumber = request.QuestionNumber;
        question.QuestionText = request.QuestionText;
        question.CorrectAnswer = request.CorrectAnswer;
        question.Options = request.Options;
        question.Explanation = request.Explanation;
        question.BandTarget = request.BandTarget;
        question.ParagraphReference = request.ParagraphReference;

        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "update_reading_question", "ReadingQuestion", questionId);

        return Ok(ApiResponse<bool>.Ok(true, "Reading question updated"));
    }

    /// <summary>
    /// Teacher: Delete a reading question
    /// </summary>
    [HttpDelete("passages/{passageId}/questions/{questionId}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteQuestion(int passageId, int questionId)
    {
        var question = await _context.ReadingQuestions
            .FirstOrDefaultAsync(q => q.QuestionId == questionId && q.PassageId == passageId);
        if (question == null)
            return NotFound(ApiResponse<bool>.Fail("Question not found"));

        _context.ReadingQuestions.Remove(question);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "delete_reading_question", "ReadingQuestion", questionId);

        return Ok(ApiResponse<bool>.Ok(true, "Reading question deleted"));
    }

    #endregion
}
