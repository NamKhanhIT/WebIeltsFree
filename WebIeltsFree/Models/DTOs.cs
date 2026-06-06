using System.ComponentModel.DataAnnotations;

namespace WebIeltsFree.Models;

#region DTOs - Notifications, Achievements, Sessions, Activity

public class NotificationDto
{
    public int NotificationId { get; set; }
    public string Type { get; set; } = "system";
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AchievementDto
{
    public int AchievementId { get; set; }
    public string AchievementCode { get; set; } = string.Empty;
    public string AchievementName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int Points { get; set; }
    public string? BadgeColor { get; set; }
}

public class AchievementStatsDto
{
    public int TotalEarned { get; set; }
    public int TotalAvailable { get; set; }
    public int TotalPoints { get; set; }
    public int ProgressPercent { get; set; }
}

public class UserAchievementDto
{
    public int UserAchievementId { get; set; }
    public AchievementDto Achievement { get; set; }
    public DateTime UnlockedAt { get; set; }
}

public class LogActivityRequest
{
    public int LessonsCompleted { get; set; }
    public int QuestionsAnswered { get; set; }
    public int WritingSubmissions { get; set; }
    public int SpeakingSessions { get; set; }
    public int MinutesSpent { get; set; }
    public int XpEarned { get; set; }
}

public class UserActivityDto
{
    public int ActivityId { get; set; }
    public DateTime ActivityDate { get; set; }
    public int LessonsCompleted { get; set; }
    public int QuestionsAnswered { get; set; }
    public int WritingSubmissions { get; set; }
    public int SpeakingSessions { get; set; }
    public int MinutesSpent { get; set; }
    public int XpEarned { get; set; }
    public int StreakDay { get; set; }
}

public class StreakInfoDto
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastActivityDate { get; set; }
}

public class WeeklySummaryDto
{
    public int TotalLessons { get; set; }
    public int TotalQuestions { get; set; }
    public int TotalWriting { get; set; }
    public int TotalSpeaking { get; set; }
    public int TotalMinutes { get; set; }
    public int TotalXp { get; set; }
    public int ActiveDays { get; set; }
    public int AverageMinutesPerDay { get; set; }
}

public class UserSessionDto
{
    public int SessionId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool Revoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateSessionRequest
{
    public int? ExpiryDays { get; set; } = 7;
    public string? DeviceInfo { get; set; }
}

#endregion

#region Authentication DTOs

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
    [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{8,}$", 
        ErrorMessage = "Password must contain at least one letter and one number")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string? FullName { get; set; }

    /// <summary>
    /// User's target IELTS band (0-9, configurable per user)
    /// </summary>
    [Range(0, 9, ErrorMessage = "Target band must be between 0 and 9")]
    public float TargetBand { get; set; } = 6.5f;
}

public class AuthResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public UserDto? User { get; set; }
    public string? RedirectUrl { get; set; }
}

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase, lowercase, number and special character")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase, lowercase, number and special character")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

#endregion

#region User DTOs

public class UserDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "student";
    public string Status { get; set; } = "active";
    public DateTime CreatedAt { get; set; }
    public UserProfileDto? Profile { get; set; }
    public UserGoalDto? Goal { get; set; }
}

public class UserProfileDto
{
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Country { get; set; }
    public string? Timezone { get; set; }
    public string? PreferredLanguage { get; set; }
}

public class UpdateProfileRequest
{
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Country { get; set; }
    public string? Timezone { get; set; }
    public string? PreferredLanguage { get; set; }
}

public class UserGoalDto
{
    public float? CurrentBand { get; set; }
    
    /// <summary>
    /// Target band score (0-9 IELTS scale, configurable per user)
    /// </summary>
    public float TargetBand { get; set; }
    
    public DateTime? ExamDate { get; set; }
    public int? StudyHoursPerDay { get; set; }
}

/// <summary>
/// DTO for user onboarding status
/// </summary>
public class OnboardingStatusDto
{
    public bool HasCompletedTutorial { get; set; }
    public bool HasPlacement { get; set; }
    public bool HasGoals { get; set; }
    public bool HasRoadmap { get; set; }
    public float? CurrentBand { get; set; }
    public float TargetBand { get; set; }
}

/// <summary>
/// Request for saving onboarding goals
/// </summary>
public class SaveOnboardingGoalsRequest
{
    [Required]
    public string LearningReason { get; set; } = string.Empty;
    
    public DateTime? ExamDate { get; set; }
    
    [Range(1, 12)]
    public int StudyHoursPerDay { get; set; } = 2;
    
    [Range(0, 9)]
    public float TargetBand { get; set; } = 7.0f;
}

public class SetGoalRequest
{
    /// <summary>
    /// Target band score (0-9 IELTS scale)
    /// Common targets: 5.0, 5.5, 6.0, 6.5, 7.0, 7.5, 8.0
    /// </summary>
    [Required]
    [Range(0, 9, ErrorMessage = "Target band must be between 0 and 9")]
    public float TargetBand { get; set; }

    public DateTime? ExamDate { get; set; }

    [Range(1, 12)]
    public int? StudyHoursPerDay { get; set; }

    /// <summary>
    /// Why user is learning IELTS (academic, immigration, career, personal)
    /// </summary>
    public string? LearningReason { get; set; }
}

public class UserProgressSummary
{
    public float? CurrentBand { get; set; }
    public float TargetBand { get; set; }
    public float ProgressPercent { get; set; }
    public int TotalLessonsCompleted { get; set; }
    public int TotalTestsTaken { get; set; }
    public int CurrentStreak { get; set; }
    public int TotalXP { get; set; }
    public SkillBreakdown? Skills { get; set; }
}

public class SkillBreakdown
{
    public float? Reading { get; set; }
    public float? Listening { get; set; }
    public float? Writing { get; set; }
    public float? Speaking { get; set; }
}

#endregion

#region Learning DTOs

public class CourseDto
{
    public int CourseId { get; set; }
    public string? Title { get; set; }
    public float? TargetBand { get; set; }
    public string? Description { get; set; }
    public int ModuleCount { get; set; }
    public int LessonCount { get; set; }
}

public class ModuleDto
{
    public int ModuleId { get; set; }
    public string? Title { get; set; }
    public int OrderIndex { get; set; }
    public List<LessonSummaryDto> Lessons { get; set; } = new();
}

public class LessonSummaryDto
{
    public int LessonId { get; set; }
    public string? Title { get; set; }
    public string? SkillType { get; set; }
    public int? DifficultyLevel { get; set; }
    public int? EstimatedMinutes { get; set; }
    public float CompletionPercent { get; set; }
}

public class LessonDetailDto
{
    public int LessonId { get; set; }
    public string? Title { get; set; }
    public string? SkillType { get; set; }
    public int? DifficultyLevel { get; set; }
    public int? EstimatedMinutes { get; set; }
    public List<LessonContentDto> Contents { get; set; } = new();
    public float CompletionPercent { get; set; }
    public float? Score { get; set; }
    public ReadingPassageDto? ReadingPassage { get; set; }
}

public class LessonContentDto
{
    public int ContentId { get; set; }
    public string? ContentType { get; set; }
    public string? ContentBody { get; set; }
}

public class SubmitAnswerRequest
{
    [Required]
    public int QuestionId { get; set; }
    
    [Required]
    public string Answer { get; set; } = string.Empty;
}

public class SubmitAnswerResponse
{
    public bool IsCorrect { get; set; }
    public string? CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
    public int XPEarned { get; set; }
}

public class CompleteLessonResponse
{
    public bool Success { get; set; }
    public float Score { get; set; }
    public int XPEarned { get; set; }
    public string? Message { get; set; }
}

#endregion

#region Test DTOs

public class TestSummaryDto
{
    public int TestId { get; set; }
    public string? Title { get; set; }
    public int? Difficulty { get; set; }
    public int DurationMinutes { get; set; }
    public string? SkillType { get; set; }
    public bool HasAttempted { get; set; }
    public float? BestScore { get; set; }
}

public class TestDetailDto
{
    public int TestId { get; set; }
    public string? Title { get; set; }
    public int DurationMinutes { get; set; }
    public List<TestSectionDto> Sections { get; set; } = new();
}

public class TestSectionDto
{
    public int SectionId { get; set; }
    public string? SkillType { get; set; }
    /// <summary>URL of audio file for listening sections</summary>
    public string? AudioUrl { get; set; }
    public string? PassageText { get; set; }
    public string? PassageTitle { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}

public class QuestionDto
{
    public int QuestionId { get; set; }
    public string? QuestionText { get; set; }
    public int? Difficulty { get; set; }
}

public class SubmitTestRequest
{
    [Required]
    public int TestId { get; set; }
    
    [Required]
    public List<TestAnswerItem> Answers { get; set; } = new();
}

public class TestAnswerItem
{
    public int QuestionId { get; set; }
    public string Answer { get; set; } = string.Empty;
}

public class TestResultDto
{
    public int AttemptId { get; set; }
    public int TestId { get; set; }
    public string? TestTitle { get; set; }
    
    /// <summary>
    /// Band score (0-9 IELTS scale)
    /// </summary>
    public float? BandScore { get; set; }
    
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int XPEarned { get; set; }
    public List<AnswerResultDto> AnswerResults { get; set; } = new();
}

public class AnswerResultDto
{
    public int QuestionId { get; set; }
    public string? QuestionText { get; set; }
    public string? UserAnswer { get; set; }
    public string? CorrectAnswer { get; set; }
    public bool IsCorrect { get; set; }
}

public class TestHistoryDto
{
    public int AttemptId { get; set; }
    public string? TestTitle { get; set; }
    public float? BandScore { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}

public class TeacherAssignmentDto
{
    public int AssignmentId { get; set; }
    public int TestId { get; set; }
    public string Title { get; set; } = "";
    public string? Instructions { get; set; }
    public DateTime? Deadline { get; set; }
    public string Status { get; set; } = "pending"; // pending, completed, overdue
    public DateTime AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? TeacherName { get; set; }
    public int DurationMinutes { get; set; }
    public int? AttemptId { get; set; }
}

#endregion

#region Speaking & Writing DTOs

public class SpeakingTopicDto
{
    public int TopicId { get; set; }
    public string? Topic { get; set; }
    public string? Category { get; set; }
    public int? DifficultyLevel { get; set; }
}

public class StartSpeakingSessionRequest
{
    public string? Topic { get; set; }
}

public class StartSpeakingSessionResponse
{
    public int SessionId { get; set; }
    public string? Topic { get; set; }
    public string? Instructions { get; set; }
}

public class SubmitSpeakingRequest
{
    [Required]
    public int SessionId { get; set; }
    
    /// <summary>
    /// Base64 encoded audio data or audio file URL
    /// </summary>
    [Required]
    public string AudioData { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional transcript if speech-to-text was done client-side
    /// </summary>
    public string? Transcript { get; set; }
}

public class SpeakingResultDto
{
    public int SessionId { get; set; }
    public string? Topic { get; set; }
    public float? FluencyScore { get; set; }
    public float? PronunciationScore { get; set; }
    public float? GrammarScore { get; set; }
    public float? OverallBand { get; set; }
    public string? Transcript { get; set; }
    public string? AiFeedback { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WritingPromptDto
{
    public int PromptId { get; set; }
    public string? Prompt { get; set; }
    public int TaskType { get; set; } // 1 or 2
    public int? DifficultyLevel { get; set; }
    public string? PromptImageUrl { get; set; }
}

public class SubmitWritingRequest
{
    [MaxLength(500, ErrorMessage = "Prompt cannot exceed 500 characters")]
    public string? Prompt { get; set; }
    
    [Required]
    [MinLength(50, ErrorMessage = "Essay must be at least 50 characters")]
    [MaxLength(10000, ErrorMessage = "Essay cannot exceed 10000 characters")]
    public string EssayText { get; set; } = string.Empty;
    
    [Range(1, 2, ErrorMessage = "Task type must be 1 or 2")]
    public int TaskType { get; set; } = 2;
}

public class WritingResultDto
{
    public int SubmissionId { get; set; }
    public string? Prompt { get; set; }
    public float? BandScore { get; set; }
    public float? TaskAchievementScore { get; set; }
    public float? CoherenceCohesionScore { get; set; }
    public float? LexicalResourceScore { get; set; }
    public float? GrammarAccuracyScore { get; set; }
    public string? AiFeedback { get; set; }
    public int? WordCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

#endregion

#region AI DTOs

public class AiChatRequest
{
    [Required]
    [MaxLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
    public string Message { get; set; } = string.Empty;
    
    [MaxLength(5000, ErrorMessage = "Context cannot exceed 5000 characters")]
    public string? Context { get; set; }
}

public class AiChatResponse
{
    public string Response { get; set; } = string.Empty;
    public List<string>? SuggestedFollowUps { get; set; }
}

public class GenerateLearningPathRequest
{
    /// <summary>
    /// Target band score (0-9 IELTS scale)
    /// </summary>
    [Required]
    [Range(0, 9)]
    public float TargetBand { get; set; }
    
    public DateTime? ExamDate { get; set; }
    public int? StudyHoursPerDay { get; set; }
    public List<string>? FocusAreas { get; set; }
}

public class LearningPathDto
{
    public int RoadmapId { get; set; }
    public float TargetBand { get; set; }
    public int EstimatedWeeks { get; set; }
    public int CurrentWeek { get; set; } = 1;
    public List<WeekPlanDto> WeeklyPlan { get; set; } = new();
}

public class WeekPlanDto
{
    public int Week { get; set; }
    public int WeekNumber { get; set; }
    public string? Focus { get; set; }
    public List<LessonSummaryDto> Lessons { get; set; } = new();
}

public class AiRecommendationDto
{
    public string? RecommendationType { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? LessonId { get; set; }
    public int Priority { get; set; }
}

public class BandPredictionDto
{
    public float PredictedBand { get; set; }
    public float ConfidencePercent { get; set; }
    public SkillBreakdown? PredictedSkills { get; set; }
    public string? Insights { get; set; }
    public string? Message { get; set; }
}

public class AiInsightsDto
{
    public string? OverallAssessment { get; set; }
    public List<string>? Strengths { get; set; }
    public List<string>? AreasForImprovement { get; set; }
    public List<string>? StudyTips { get; set; }
}

#endregion

#region Practice DTOs - Listening & Reading

public class ListeningMaterialDto
{
    public int MaterialId { get; set; }
    public int? LessonId { get; set; }
    public string MaterialType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Topics { get; set; }
    public string DifficultyLevel { get; set; } = string.Empty;
    public string? Source { get; set; }
}

public class ListeningQuestionDto
{
    public int QuestionId { get; set; }
    public string QuestionType { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public int? TimeCodeStart { get; set; }
    public int? TimeCodeEnd { get; set; }
    public List<string>? Options { get; set; }
    public int QuestionOrder { get; set; }
}

public class ListeningPracticeSetDto
{
    public ListeningMaterialDto Material { get; set; } = new();
    public List<ListeningQuestionDto> Questions { get; set; } = new();
    public string? Transcript { get; set; }
}

public class SubmitListeningAnswerRequest
{
    [Required]
    public int QuestionId { get; set; }

    [Required]
    [MaxLength(500)]
    public string UserAnswer { get; set; } = string.Empty;

    public int? TimeSpentSeconds { get; set; }
}

public class ListeningAnswerResultDto
{
    public int QuestionId { get; set; }
    public bool IsCorrect { get; set; }
    public string UserAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public float? BandTarget { get; set; }
}

public class ReadingPassageDto
{
    public int PassageId { get; set; }
    public string PassageTitle { get; set; } = string.Empty;
    public string PassageText { get; set; } = string.Empty;
    public int? WordCount { get; set; }
    public string DifficultyLevel { get; set; } = string.Empty;
    public string? TopicCategory { get; set; }
    public string? Source { get; set; }
    public string? ImageUrl { get; set; }
    public string? PassageTranslation { get; set; }
    public string? VocabHighlights { get; set; }
}

public class ReadingQuestionDto
{
    public int QuestionId { get; set; }
    public int QuestionNumber { get; set; }
    public string QuestionType { get; set; } = string.Empty; // true_false_not_given, multiple_choice, matching_heading, sentence_completion, summary_completion
    public string QuestionText { get; set; } = string.Empty;
    public List<string>? Options { get; set; }
    public string? ParagraphReference { get; set; }
}

public class ReadingPracticeSetDto
{
    public ReadingPassageDto Passage { get; set; } = new();
    public List<ReadingQuestionDto> Questions { get; set; } = new();
}

public class SubmitReadingAnswerRequest
{
    [Required]
    public int QuestionId { get; set; }

    [Required]
    [MaxLength(500)]
    public string UserAnswer { get; set; } = string.Empty;

    public int? TimeSpentSeconds { get; set; }
}

public class ReadingAnswerResultDto
{
    public int QuestionId { get; set; }
    public int QuestionNumber { get; set; }
    public bool IsCorrect { get; set; }
    public string UserAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public float? BandTarget { get; set; }
    public string QuestionType { get; set; } = string.Empty;
}

public class PracticeSessionSummaryDto
{
    public string SkillType { get; set; } = string.Empty; // listening or reading
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public float AccuracyPercent { get; set; }
    public float? AverageBandTarget { get; set; }
    public int TotalTimeSeconds { get; set; }
    public List<ListeningAnswerResultDto>? ListeningResults { get; set; }
    public List<ReadingAnswerResultDto>? ReadingResults { get; set; }
}

#endregion

#region Common DTOs

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int? TotalCount { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T> { Success = true, Data = data, Message = message };
    }

    public static ApiResponse<T> Fail(string message, List<string>? errors = null)
    {
        return new ApiResponse<T> { Success = false, Message = message, Errors = errors };
    }

    public static ApiResponse<T> Error(string message)
    {
        return new ApiResponse<T> { Success = false, Message = message };
    }

    public static ApiResponse<T> OkWithPaging(T data, int totalCount, int pageNumber, int pageSize, string? message = null)
    {
        return new ApiResponse<T> 
        { 
            Success = true, 
            Data = data, 
            Message = message,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}

public class PaginationParams
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}

#endregion

#region Text-to-Speech DTOs

public class TextToSpeechRequest
{
    [Required]
    [MaxLength(5000, ErrorMessage = "Text cannot exceed 5000 characters")]
    public string Text { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string? Language { get; set; } = "en-US";
    
    [MaxLength(50)]
    public string? Voice { get; set; } = "en-US-Neural2-C"; // Female voice
    
    [Range(-20, 20)]
    public double? Pitch { get; set; } = 0.0;
    
    [Range(0.25, 4.0)]
    public double? SpeakingRate { get; set; } = 1.0;
}

public class TextToSpeechResponse
{
    public string? AudioUrl { get; set; }
    public int Duration { get; set; }
    public string? ContentType { get; set; } = "audio/mp3";
}

#endregion

#region Teacher Domain DTOs

// === Admin ===
public class ChangeRoleRequest
{
    [Required]
    public string Role { get; set; } = "";
}

// === Disputes ===
public class SubmitDisputeRequest
{
    [Required]
    public string SubmissionType { get; set; } = ""; // writing, speaking
    [Required]
    public int SubmissionId { get; set; }
    [Required]
    [MinLength(10)]
    public string Reason { get; set; } = "";
}

public class ResolveDisputeRequest
{
    [Required]
    [MinLength(5)]
    public string TeacherNotes { get; set; } = "";
    public float? RevisedScore { get; set; }
}

public class RejectDisputeRequest
{
    [Required]
    [MinLength(5)]
    public string TeacherNotes { get; set; } = "";
}

public class DisputeDto
{
    public int DisputeId { get; set; }
    public int UserId { get; set; }
    public string? StudentName { get; set; }
    public string SubmissionType { get; set; } = "";
    public int SubmissionId { get; set; }
    public string Reason { get; set; } = "";
    public string Status { get; set; } = "pending";
    public int? ReviewedBy { get; set; }
    public string? ReviewerName { get; set; }
    public float OriginalScore { get; set; }
    public float? RevisedScore { get; set; }
    public string? TeacherNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

// === Messaging ===
public class StartConversationRequest
{
    [MaxLength(255)]
    public string? Subject { get; set; }
    [Required]
    [MinLength(1)]
    public string Message { get; set; } = "";
}

public class SendMessageRequest
{
    [Required]
    [MinLength(1)]
    public string Content { get; set; } = "";
}

public class ConversationDto
{
    public int ConversationId { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public int? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public string? Subject { get; set; }
    public string Status { get; set; } = "open";
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
}

public class MessageDto
{
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public string? SenderName { get; set; }
    public string? SenderRole { get; set; }
    public string Content { get; set; } = "";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

// === Teacher Dashboard ===
public class TeacherDashboardDto
{
    public int PendingDisputes { get; set; }
    public int OpenConversations { get; set; }
    public int TotalStudents { get; set; }
    public int ResolvedDisputesThisWeek { get; set; }
}

public class TeacherStudentListDto
{
    public int UserId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public float? CurrentBand { get; set; }
    public float? TargetBand { get; set; }
    public DateTime? LastActiveAt { get; set; }
    public int TotalPracticeAttempts { get; set; }
}

public class TeacherStudentDetailDto
{
    public UserDto? Profile { get; set; }
    public UserGoalDto? Goals { get; set; }
    public List<WritingResultDto> LatestWritingSubmissions { get; set; } = new();
    public List<SpeakingResultDto> LatestSpeakingSessions { get; set; } = new();
    public int TotalTestAttempts { get; set; }
    public int TotalPracticeAttempts { get; set; }
    public List<DisputeDto> ActiveDisputes { get; set; } = new();
}


// === Teacher Content CRUD ===
public class CreateWritingPromptRequest
{
    [Required]
    public string TaskType { get; set; } = "task1"; // task1 or task2
    [Required]
    public string PromptText { get; set; } = "";
    public string? PromptImageUrl { get; set; }
    public string? ChartType { get; set; }
    public int DifficultyLevel { get; set; } = 1;
    public string? Category { get; set; }
    public int BandTarget { get; set; } = 6;
    public string? SampleAnswer { get; set; }
    public string? NotesForTeacher { get; set; }
}

public class CreateSpeakingTopicRequest
{
    [Required]
    public string Part { get; set; } = "1";
    [Required]
    public string TopicTitle { get; set; } = "";
    public string? Description { get; set; }
    public int DifficultyLevel { get; set; } = 1;
    public int BandTarget { get; set; } = 6;
}

public class CreateSpeakingTopicPartRequest
{
    public int? PartNumber { get; set; }
    [Required]
    public string ContentText { get; set; } = "";
    public int? TimeLimitSeconds { get; set; }
    public int? SequenceOrder { get; set; }
    public bool IsFollowUp { get; set; }
}

public class CreateReadingPassageRequest
{
    [Required]
    public string PassageTitle { get; set; } = "";
    [Required]
    public string PassageText { get; set; } = "";
    public string DifficultyLevel { get; set; } = "band_5_6";
    public string? TopicCategory { get; set; }
    public string? Source { get; set; }
}

public class CreateReadingQuestionRequest
{
    [Required]
    public string QuestionType { get; set; } = "multiple_choice";
    public int QuestionNumber { get; set; }
    [Required]
    public string QuestionText { get; set; } = "";
    [Required]
    public string CorrectAnswer { get; set; } = "";
    public string? Options { get; set; }
    public string? Explanation { get; set; }
    public float? BandTarget { get; set; }
    public string? ParagraphReference { get; set; }
}

public class CreateListeningMaterialRequest
{
    [Required]
    public string Title { get; set; } = "";
    public string MaterialType { get; set; } = "dialogue";
    [Required]
    public string Transcript { get; set; } = "";
    public string? AudioUrl { get; set; }
    public int? DurationSeconds { get; set; }
    public int? SpeakerCount { get; set; }
    public string? Topics { get; set; }
    public string? NotesForLearner { get; set; }
    public string DifficultyLevel { get; set; } = "band_5_6";
    public string? Source { get; set; }
}

public class CreateListeningQuestionRequest
{
    [Required]
    public string QuestionType { get; set; } = "fill_in_blank";
    [Required]
    public string QuestionText { get; set; } = "";
    [Required]
    public string CorrectAnswer { get; set; } = "";
    public string? Options { get; set; }
    public string? Explanation { get; set; }
    public int? TimeCodeStart { get; set; }
    public int? TimeCodeEnd { get; set; }
    public float? BandTarget { get; set; }
    public int QuestionOrder { get; set; }
}

#endregion
