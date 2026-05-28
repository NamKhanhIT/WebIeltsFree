using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListeningController : ControllerBase
{
    private readonly AppDbContext _context;

    public ListeningController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get listening practice material by ID without answers
    /// </summary>
    [HttpGet("material/{materialId}")]
    public async Task<ActionResult<ApiResponse<ListeningPracticeSetDto>>> GetMaterial(int materialId)
    {
        var material = await _context.ListeningMaterials
            .Include(m => m.Questions)
            .FirstOrDefaultAsync(m => m.MaterialId == materialId);

        if (material == null)
            return NotFound(ApiResponse<ListeningPracticeSetDto>.Fail("Material not found"));

        var dto = new ListeningPracticeSetDto
        {
            Material = new ListeningMaterialDto
            {
                MaterialId = material.MaterialId,
                LessonId = material.LessonId,
                MaterialType = material.MaterialType,
                Title = material.Title,
                AudioUrl = material.AudioUrl,
                DurationSeconds = material.DurationSeconds,
                Topics = material.Topics,
                DifficultyLevel = material.DifficultyLevel,
                Source = material.Source
            },
            Questions = material.Questions
                .OrderBy(q => q.QuestionOrder)
                .Select(q => new ListeningQuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionType = q.QuestionType,
                    QuestionText = q.QuestionText,
                    TimeCodeStart = q.TimeCodeStart,
                    TimeCodeEnd = q.TimeCodeEnd,
                    Options = q.Options != null ? JsonSerializer.Deserialize<List<string>>(q.Options) : null,
                    QuestionOrder = q.QuestionOrder
                })
                .ToList(),
            Transcript = material.Transcript
        };

        return Ok(ApiResponse<ListeningPracticeSetDto>.Ok(dto));
    }

    /// <summary>
    /// Get listening practice material by Lesson ID
    /// </summary>
    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<ApiResponse<ListeningPracticeSetDto>>> GetMaterialByLesson(int lessonId)
    {
        var material = await _context.ListeningMaterials
            .Include(m => m.Questions)
            .FirstOrDefaultAsync(m => m.LessonId == lessonId);

        if (material == null)
            return NotFound(ApiResponse<ListeningPracticeSetDto>.Fail("Material for this lesson not found"));

        var dto = new ListeningPracticeSetDto
        {
            Material = new ListeningMaterialDto
            {
                MaterialId = material.MaterialId,
                LessonId = material.LessonId,
                MaterialType = material.MaterialType,
                Title = material.Title,
                AudioUrl = material.AudioUrl,
                DurationSeconds = material.DurationSeconds,
                Topics = material.Topics,
                DifficultyLevel = material.DifficultyLevel,
                Source = material.Source
            },
            Questions = material.Questions
                .OrderBy(q => q.QuestionOrder)
                .Select(q => new ListeningQuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionType = q.QuestionType,
                    QuestionText = q.QuestionText,
                    TimeCodeStart = q.TimeCodeStart,
                    TimeCodeEnd = q.TimeCodeEnd,
                    Options = q.Options != null ? JsonSerializer.Deserialize<List<string>>(q.Options) : null,
                    QuestionOrder = q.QuestionOrder
                })
                .ToList(),
            Transcript = material.Transcript
        };

        return Ok(ApiResponse<ListeningPracticeSetDto>.Ok(dto));
    }

    /// <summary>
    /// Get listening practice set by difficulty level (mixed questions, no answers)
    /// </summary>
    [HttpGet("practice/{difficulty}")]
    public async Task<ActionResult<ApiResponse<ListeningPracticeSetDto>>> GetPracticeSet(string difficulty)
    {
        var validDifficulties = new[] { "band_3_4", "band_5_6", "band_7_8", "band_8_9" };
        if (!validDifficulties.Contains(difficulty.ToLower()))
            return BadRequest(ApiResponse<ListeningPracticeSetDto>.Fail("Invalid difficulty level"));

        var material = await _context.ListeningMaterials
            .Include(m => m.Questions)
            .Where(m => m.DifficultyLevel.ToLower() == difficulty.ToLower())
            .OrderBy(m => Guid.NewGuid())
            .FirstOrDefaultAsync();

        if (material == null)
            return NotFound(ApiResponse<ListeningPracticeSetDto>.Fail($"No practice materials available for {difficulty}"));

        var dto = new ListeningPracticeSetDto
        {
            Material = new ListeningMaterialDto
            {
                MaterialId = material.MaterialId,
                LessonId = material.LessonId,
                MaterialType = material.MaterialType,
                Title = material.Title,
                AudioUrl = material.AudioUrl,
                DurationSeconds = material.DurationSeconds,
                Topics = material.Topics,
                DifficultyLevel = material.DifficultyLevel,
                Source = material.Source
            },
            Questions = material.Questions
                .OrderBy(q => q.QuestionOrder)
                .Select(q => new ListeningQuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionType = q.QuestionType,
                    QuestionText = q.QuestionText,
                    TimeCodeStart = q.TimeCodeStart,
                    TimeCodeEnd = q.TimeCodeEnd,
                    Options = q.Options != null ? JsonSerializer.Deserialize<List<string>>(q.Options) : null,
                    QuestionOrder = q.QuestionOrder
                })
                .ToList(),
            Transcript = material.Transcript
        };

        return Ok(ApiResponse<ListeningPracticeSetDto>.Ok(dto));
    }

    /// <summary>
    /// List all available listening materials
    /// </summary>
    [HttpGet("materials")]
    public async Task<ActionResult<ApiResponse<List<ListeningMaterialDto>>>> ListMaterials([FromQuery] string? difficulty = null)
    {
        var query = _context.ListeningMaterials.AsQueryable();

        if (!string.IsNullOrWhiteSpace(difficulty))
            query = query.Where(m => m.DifficultyLevel.ToLower() == difficulty.ToLower());

        var materials = await query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new ListeningMaterialDto
            {
                MaterialId = m.MaterialId,
                LessonId = m.LessonId,
                MaterialType = m.MaterialType,
                Title = m.Title,
                AudioUrl = m.AudioUrl,
                DurationSeconds = m.DurationSeconds,
                Topics = m.Topics,
                DifficultyLevel = m.DifficultyLevel,
                Source = m.Source
            })
            .ToListAsync();

        return Ok(ApiResponse<List<ListeningMaterialDto>>.Ok(materials));
    }

    /// <summary>
    /// Submit listening answer and get immediate feedback
    /// </summary>
    [HttpPost("submit")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ListeningAnswerResultDto>>> SubmitAnswer([FromBody] SubmitListeningAnswerRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<ListeningAnswerResultDto>.Fail("Not authenticated"));

        var question = await _context.ListeningQuestions
            .FirstOrDefaultAsync(q => q.QuestionId == request.QuestionId);

        if (question == null)
            return NotFound(ApiResponse<ListeningAnswerResultDto>.Fail("Question not found"));

        // Normalize and validate answer based on question type
        var isCorrect = ValidateListeningAnswer(question, request.UserAnswer);

        // Record attempt
        var attempt = new UserPracticeAttempt
        {
            UserId = userId,
            SkillType = "listening",
            ListeningQuestionId = question.QuestionId,
            UserAnswer = request.UserAnswer,
            IsCorrect = isCorrect,
            TimeSpentSeconds = request.TimeSpentSeconds,
            AttemptDate = DateTime.UtcNow
        };

        _context.UserPracticeAttempts.Add(attempt);
        await _context.SaveChangesAsync();

        var result = new ListeningAnswerResultDto
        {
            QuestionId = question.QuestionId,
            IsCorrect = isCorrect,
            UserAnswer = request.UserAnswer,
            CorrectAnswer = question.CorrectAnswer,
            Explanation = question.Explanation,
            BandTarget = question.BandTarget
        };

        return Ok(ApiResponse<ListeningAnswerResultDto>.Ok(result));
    }

    /// <summary>
    /// Validate listening answer based on question type
    /// </summary>
    private bool ValidateListeningAnswer(ListeningQuestion question, string userAnswer)
    {
        if (string.IsNullOrWhiteSpace(userAnswer) || string.IsNullOrWhiteSpace(question.CorrectAnswer))
            return false;

        var userAns = userAnswer.Trim().ToLower();
        var correctAns = question.CorrectAnswer.Trim().ToLower();

        if (question.QuestionType.ToLower() == "multiple_choice")
        {
            // Exact match for multiple choice (A, B, C, D)
            return userAns == correctAns;
        }
        else // fill_in_blank
        {
            // Case-insensitive, whitespace-trimmed comparison
            // Accept alternative answers separated by comma
            var alternatives = correctAns.Split(',').Select(a => a.Trim());
            return alternatives.Any(alt => userAns == alt);
        }
    }

    /// <summary>
    /// Get user's listening practice history
    /// </summary>
    [HttpGet("history")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<ListeningAnswerResultDto>>>> GetHistory([FromQuery] int limit = 20)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<List<ListeningAnswerResultDto>>.Fail("Not authenticated"));

        var attempts = await _context.UserPracticeAttempts
            .Where(a => a.UserId == userId && a.SkillType == "listening")
            .Include(a => a.ListeningQuestion)
            .OrderByDescending(a => a.AttemptDate)
            .Take(limit)
            .Select(a => new ListeningAnswerResultDto
            {
                QuestionId = a.ListeningQuestion!.QuestionId,
                IsCorrect = a.IsCorrect,
                UserAnswer = a.UserAnswer ?? string.Empty,
                CorrectAnswer = a.ListeningQuestion.CorrectAnswer,
                Explanation = a.ListeningQuestion.Explanation,
                BandTarget = a.ListeningQuestion.BandTarget
            })
            .ToListAsync();

        return Ok(ApiResponse<List<ListeningAnswerResultDto>>.Ok(attempts));
    }

    /// <summary>
    /// Get listening practice statistics
    /// </summary>
    [HttpGet("stats")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PracticeSessionSummaryDto>>> GetStats()
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<PracticeSessionSummaryDto>.Fail("Not authenticated"));

        var attempts = await _context.UserPracticeAttempts
            .Where(a => a.UserId == userId && a.SkillType == "listening")
            .Include(a => a.ListeningQuestion)
            .ToListAsync();

        if (!attempts.Any())
            return Ok(ApiResponse<PracticeSessionSummaryDto>.Ok(new PracticeSessionSummaryDto
            {
                SkillType = "listening",
                TotalQuestions = 0,
                CorrectAnswers = 0,
                AccuracyPercent = 0,
                TotalTimeSeconds = 0
            }));

        var totalCorrect = attempts.Count(a => a.IsCorrect);
        var totalTime = attempts.Sum(a => a.TimeSpentSeconds ?? 0);
        var avgBand = attempts.Average(a => (float?)a.ListeningQuestion?.BandTarget) ?? 0;

        var summary = new PracticeSessionSummaryDto
        {
            SkillType = "listening",
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
    /// Teacher: Create a listening material
    /// </summary>
    [HttpPost("materials")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<ListeningMaterialDto>>> CreateMaterial([FromBody] CreateListeningMaterialRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<ListeningMaterialDto>.Fail("Invalid request"));

        var material = new ListeningMaterial
        {
            Title = request.Title,
            MaterialType = request.MaterialType,
            Transcript = request.Transcript,
            AudioUrl = request.AudioUrl,
            DurationSeconds = request.DurationSeconds,
            SpeakerCount = request.SpeakerCount,
            Topics = request.Topics,
            NotesForLearner = request.NotesForLearner,
            DifficultyLevel = request.DifficultyLevel,
            Source = request.Source,
            CreatedAt = DateTime.UtcNow
        };

        _context.ListeningMaterials.Add(material);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "create_listening_material", "ListeningMaterial", material.MaterialId);

        return Ok(ApiResponse<ListeningMaterialDto>.Ok(new ListeningMaterialDto
        {
            MaterialId = material.MaterialId,
            MaterialType = material.MaterialType,
            Title = material.Title,
            AudioUrl = material.AudioUrl,
            DurationSeconds = material.DurationSeconds,
            Topics = material.Topics,
            DifficultyLevel = material.DifficultyLevel,
            Source = material.Source
        }, "Listening material created"));
    }

    /// <summary>
    /// Teacher: Update a listening material
    /// </summary>
    [HttpPut("materials/{id}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateMaterial(int id, [FromBody] CreateListeningMaterialRequest request)
    {
        var material = await _context.ListeningMaterials.FindAsync(id);
        if (material == null)
            return NotFound(ApiResponse<bool>.Fail("Material not found"));

        material.Title = request.Title;
        material.MaterialType = request.MaterialType;
        material.Transcript = request.Transcript;
        material.AudioUrl = request.AudioUrl;
        material.DurationSeconds = request.DurationSeconds;
        material.SpeakerCount = request.SpeakerCount;
        material.Topics = request.Topics;
        material.NotesForLearner = request.NotesForLearner;
        material.DifficultyLevel = request.DifficultyLevel;
        material.Source = request.Source;

        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "update_listening_material", "ListeningMaterial", id);

        return Ok(ApiResponse<bool>.Ok(true, "Listening material updated"));
    }

    /// <summary>
    /// Teacher: Delete a listening material
    /// </summary>
    [HttpDelete("materials/{id}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMaterial(int id)
    {
        var material = await _context.ListeningMaterials.FindAsync(id);
        if (material == null)
            return NotFound(ApiResponse<bool>.Fail("Material not found"));

        _context.ListeningMaterials.Remove(material);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "delete_listening_material", "ListeningMaterial", id);

        return Ok(ApiResponse<bool>.Ok(true, "Listening material deleted"));
    }

    /// <summary>
    /// Teacher: Add a question to a listening material
    /// </summary>
    [HttpPost("materials/{materialId}/questions")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> CreateMaterialQuestion(int materialId, [FromBody] CreateListeningQuestionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<bool>.Fail("Invalid request"));

        var material = await _context.ListeningMaterials.FindAsync(materialId);
        if (material == null)
            return NotFound(ApiResponse<bool>.Fail("Material not found"));

        var question = new ListeningQuestion
        {
            MaterialId = materialId,
            QuestionType = request.QuestionType,
            QuestionText = request.QuestionText,
            CorrectAnswer = request.CorrectAnswer,
            Options = request.Options,
            Explanation = request.Explanation,
            TimeCodeStart = request.TimeCodeStart,
            TimeCodeEnd = request.TimeCodeEnd,
            BandTarget = request.BandTarget,
            QuestionOrder = request.QuestionOrder,
            CreatedAt = DateTime.UtcNow
        };

        _context.ListeningQuestions.Add(question);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "create_listening_question", "ListeningQuestion", question.QuestionId);

        return Ok(ApiResponse<bool>.Ok(true, "Listening question added"));
    }

    /// <summary>
    /// Teacher: Update a listening question
    /// </summary>
    [HttpPut("materials/{materialId}/questions/{questionId}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateMaterialQuestion(int materialId, int questionId, [FromBody] CreateListeningQuestionRequest request)
    {
        var question = await _context.ListeningQuestions
            .FirstOrDefaultAsync(q => q.QuestionId == questionId && q.MaterialId == materialId);
        if (question == null)
            return NotFound(ApiResponse<bool>.Fail("Question not found"));

        question.QuestionType = request.QuestionType;
        question.QuestionText = request.QuestionText;
        question.CorrectAnswer = request.CorrectAnswer;
        question.Options = request.Options;
        question.Explanation = request.Explanation;
        question.TimeCodeStart = request.TimeCodeStart;
        question.TimeCodeEnd = request.TimeCodeEnd;
        question.BandTarget = request.BandTarget;
        question.QuestionOrder = request.QuestionOrder;

        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "update_listening_question", "ListeningQuestion", questionId);

        return Ok(ApiResponse<bool>.Ok(true, "Listening question updated"));
    }

    /// <summary>
    /// Teacher: Delete a listening question
    /// </summary>
    [HttpDelete("materials/{materialId}/questions/{questionId}")]
    [Authorize(Roles = "teacher,admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMaterialQuestion(int materialId, int questionId)
    {
        var question = await _context.ListeningQuestions
            .FirstOrDefaultAsync(q => q.QuestionId == questionId && q.MaterialId == materialId);
        if (question == null)
            return NotFound(ApiResponse<bool>.Fail("Question not found"));

        _context.ListeningQuestions.Remove(question);
        await _context.SaveChangesAsync();

        var userId = GetCurrentUserId();
        await AuditHelper.LogAsync(_context, userId, "delete_listening_question", "ListeningQuestion", questionId);

        return Ok(ApiResponse<bool>.Ok(true, "Listening question deleted"));
    }

    #endregion
}
