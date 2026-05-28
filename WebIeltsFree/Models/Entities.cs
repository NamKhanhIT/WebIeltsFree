using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebIeltsFree.Models;

#region User Domain

[Table("tb_users")]
public class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("email")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Column("username")]
    [MaxLength(50)]
    public string? Username { get; set; }

    [Column("password_hash")]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("role")]
    [MaxLength(20)]
    public string Role { get; set; } = "student"; 

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "active"; 

    [Column("has_completed_onboarding")]
    public bool HasCompletedOnboarding { get; set; } = false;

    [Column("reset_password_token")]
    [MaxLength(255)]
    public string? ResetPasswordToken { get; set; }

    [Column("reset_password_expires")]
    public DateTime? ResetPasswordExpires { get; set; }

    [Column("email_verified")]
    public bool EmailVerified { get; set; }

    [Column("email_verification_token")]
    [MaxLength(255)]
    public string? EmailVerificationToken { get; set; }

    [Column("login_count")]
    public int LoginCount { get; set; }

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public UserProfile? Profile { get; set; }
    public UserGoal? Goal { get; set; }
    public ICollection<UserLearningProgress> LearningProgress { get; set; } = new List<UserLearningProgress>();
    public ICollection<UserTestAttempt> TestAttempts { get; set; } = new List<UserTestAttempt>();
    public ICollection<SpeakingSession> SpeakingSessions { get; set; } = new List<SpeakingSession>();
    public ICollection<WritingSubmission> WritingSubmissions { get; set; } = new List<WritingSubmission>();
    public ICollection<UserPracticeAttempt> PracticeAttempts { get; set; } = new List<UserPracticeAttempt>();

    // Teacher role navigation properties
    public ICollection<GradeDispute> GradeDisputes { get; set; } = new List<GradeDispute>();
    public ICollection<GradeDispute> ReviewedDisputes { get; set; } = new List<GradeDispute>();
}

/// <summary>
/// Extended user profile information
/// </summary>
[Table("tb_user_profiles")]
public class UserProfile
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("full_name")]
    [MaxLength(255)]
    public string? FullName { get; set; }

    [Column("avatar_url")]
    public string? AvatarUrl { get; set; }

    [Column("country")]
    [MaxLength(100)]
    public string? Country { get; set; }

    [Column("timezone")]
    [MaxLength(100)]
    public string? Timezone { get; set; }

    [Column("preferred_language")]
    [MaxLength(50)]
    public string? PreferredLanguage { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public User? User { get; set; }
}

/// <summary>
/// User learning goals with configurable target band (0-9 IELTS scale)
/// </summary>
[Table("tb_user_goals")]
public class UserGoal
{
    [Key]
    [Column("goal_id")]
    public int GoalId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("current_band")]
    [Range(0, 9)]
    public float? CurrentBand { get; set; }

    [Column("target_band")]
    [Range(0, 9)]
    public float TargetBand { get; set; } = 6.5f;

    [Column("exam_date")]
    public DateTime? ExamDate { get; set; }

    [Column("study_hours_per_day")]
    public int? StudyHoursPerDay { get; set; }

    [Column("learning_reason")]
    [MaxLength(255)]
    public string? LearningReason { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}

/// <summary>
/// Tracks user progress per lesson
/// </summary>
[Table("tb_user_learning_progress")]
public class UserLearningProgress
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("lesson_id")]
    public int LessonId { get; set; }

    [Column("completion_percent")]
    public float CompletionPercent { get; set; } = 0;

    [Column("score")]
    public float? Score { get; set; }

    [Column("last_accessed")]
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("LessonId")]
    public Lesson? Lesson { get; set; }
}

#endregion

#region Learning Domain

/// <summary>
/// Course - top-level learning container
/// </summary>
[Table("tb_courses")]
public class Course
{
    [Key]
    [Column("course_id")]
    public int CourseId { get; set; }

    [Column("title")]
    [MaxLength(255)]
    public string? Title { get; set; }

    [Column("target_band")]
    [Range(0, 9)]
    public float? TargetBand { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    [Column("is_published")]
    public bool IsPublished { get; set; }

    [Column("estimated_hours")]
    public int? EstimatedHours { get; set; }

    [Column("difficulty_level")]
    public int? DifficultyLevel { get; set; }

    [Column("skill_type")]
    [MaxLength(50)]
    public string? SkillType { get; set; }

    [Column("order_index")]
    public int OrderIndex { get; set; }

    [Column("slug")]
    [MaxLength(255)]
    public string? Slug { get; set; }

    public ICollection<Module> Modules { get; set; } = new List<Module>();
}

/// <summary>
/// Module - section within a course
/// </summary>
[Table("tb_modules")]
public class Module
{
    [Key]
    [Column("module_id")]
    public int ModuleId { get; set; }

    [Column("course_id")]
    public int? CourseId { get; set; }

    [Column("title")]
    [MaxLength(255)]
    public string? Title { get; set; }

    [Column("order_index")]
    public int OrderIndex { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CourseId")]
    public Course? Course { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}

/// <summary>
/// Lesson - individual learning unit
/// </summary>
[Table("tb_lessons")]
public class Lesson
{
    [Key]
    [Column("lesson_id")]
    public int LessonId { get; set; }

    [Column("module_id")]
    public int? ModuleId { get; set; }

    [Column("title")]
    [MaxLength(255)]
    public string? Title { get; set; }

    [Column("skill_type")]
    [MaxLength(20)]
    public string? SkillType { get; set; } // reading, listening, writing, speaking

    [Column("difficulty_level")]
    public int? DifficultyLevel { get; set; }

    [Column("estimated_minutes")]
    public int? EstimatedMinutes { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ModuleId")]
    public Module? Module { get; set; }

    public ICollection<LessonContent> Contents { get; set; } = new List<LessonContent>();
}

/// <summary>
/// Lesson content - actual materials
/// </summary>
[Table("tb_lesson_contents")]
public class LessonContent
{
    [Key]
    [Column("content_id")]
    public int ContentId { get; set; }

    [Column("lesson_id")]
    public int? LessonId { get; set; }

    [Column("content_type")]
    [MaxLength(20)]
    public string? ContentType { get; set; } // video, article, audio, exercise

    [Column("content_body")]
    public string? ContentBody { get; set; }

    [ForeignKey("LessonId")]
    public Lesson? Lesson { get; set; }
}

/// <summary>
/// Vocabulary bank
/// </summary>
[Table("tb_vocabulary")]
public class Vocabulary
{
    [Key]
    [Column("vocab_id")]
    public int VocabId { get; set; }

    [Column("word")]
    [MaxLength(255)]
    public string? Word { get; set; }

    [Column("meaning")]
    public string? Meaning { get; set; }

    [Column("example")]
    public string? Example { get; set; }

    [Column("difficulty")]
    public int? Difficulty { get; set; }

    [Column("audio_url")]
    [MaxLength(500)]
    public string? AudioUrl { get; set; }

    [Column("ielts_topic")]
    [MaxLength(100)]
    public string? IeltsTopic { get; set; }

    [Column("part_of_speech")]
    [MaxLength(50)]
    public string? PartOfSpeech { get; set; }

    [Column("usage_frequency")]
    public int? UsageFrequency { get; set; }

    [Column("phonetic")]
    [MaxLength(100)]
    public string? Phonetic { get; set; }

    [Column("antonyms")]
    public string? Antonyms { get; set; }

    [Column("synonyms")]
    public string? Synonyms { get; set; }
}

#endregion

#region Test Domain

/// <summary>
/// Practice test definition
/// </summary>
[Table("tb_tests")]
public class Test
{
    [Key]
    [Column("test_id")]
    public int TestId { get; set; }

    [Column("title")]
    [MaxLength(255)]
    public string? Title { get; set; }

    [Column("difficulty")]
    public int? Difficulty { get; set; }

    [Column("duration_minutes")]
    public int DurationMinutes { get; set; } = 60;

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("is_teacher_created")]
    public bool IsTeacherCreated { get; set; }

    [Column("is_public")]
    public bool IsPublic { get; set; } = true;

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "draft";

    [Column("reviewed_by")]
    public int? ReviewedBy { get; set; }

    [Column("reviewed_at")]
    public DateTime? ReviewedAt { get; set; }

    [Column("reviewer_note")]
    public string? ReviewerNote { get; set; }

    [ForeignKey("CreatedBy")]
    public User? Creator { get; set; }

    [ForeignKey("ReviewedBy")]
    public User? Reviewer { get; set; }

    public ICollection<TestSection> Sections { get; set; } = new List<TestSection>();
}

/// <summary>
/// Test section (grouped by skill type)
/// </summary>
[Table("tb_test_sections")]
public class TestSection
{
    [Key]
    [Column("section_id")]
    public int SectionId { get; set; }

    [Column("test_id")]
    public int? TestId { get; set; }

    [Column("skill_type")]
    [MaxLength(20)]
    public string? SkillType { get; set; }

    /// <summary>
    /// Audio URL for listening sections (uploaded file path or external URL)
    /// </summary>
    [Column("audio_url")]
    public string? AudioUrl { get; set; }

    [ForeignKey("TestId")]
    public Test? Test { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}

/// <summary>
/// Individual test question
/// </summary>
[Table("tb_questions")]
public class Question
{
    [Key]
    [Column("question_id")]
    public int QuestionId { get; set; }

    [Column("section_id")]
    public int? SectionId { get; set; }

    [Column("question_text")]
    public string? QuestionText { get; set; }

    [Column("difficulty")]
    public int? Difficulty { get; set; }

    [ForeignKey("SectionId")]
    public TestSection? Section { get; set; }

    public Answer? Answer { get; set; }
}

/// <summary>
/// Correct answer for a question
/// </summary>
[Table("tb_answers")]
public class Answer
{
    [Key]
    [Column("answer_id")]
    public int AnswerId { get; set; }

    [Column("question_id")]
    public int? QuestionId { get; set; }

    [Column("correct_answer")]
    public string? CorrectAnswer { get; set; }

    [ForeignKey("QuestionId")]
    public Question? Question { get; set; }
}

/// <summary>
/// User's answer during a test attempt
/// </summary>
[Table("tb_user_answers")]
public class UserAnswer
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("attempt_id")]
    public int? AttemptId { get; set; }

    [Column("question_id")]
    public int? QuestionId { get; set; }

    [Column("user_answer")]
    public string? UserAnswerText { get; set; }

    [Column("is_correct")]
    public bool? IsCorrect { get; set; }

    [ForeignKey("AttemptId")]
    public UserTestAttempt? Attempt { get; set; }

    [ForeignKey("QuestionId")]
    public Question? Question { get; set; }
}

/// <summary>
/// Records a user's test attempt with band score
/// </summary>
[Table("tb_user_test_attempts")]
public class UserTestAttempt
{
    [Key]
    [Column("attempt_id")]
    public int AttemptId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("test_id")]
    public int? TestId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("started_at")]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    [Column("finished_at")]
    public DateTime? FinishedAt { get; set; }

    [Column("band_score")]
    [Range(0, 9)]
    public float? BandScore { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("TestId")]
    public Test? Test { get; set; }

    public ICollection<UserAnswer> Answers { get; set; } = new List<UserAnswer>();
}

#endregion

#region AI Domain

/// <summary>
/// AI-generated learning roadmap
/// </summary>
[Table("tb_ai_roadmaps")]
public class AIRoadmap
{
    [Key]
    [Column("roadmap_id")]
    public int RoadmapId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("generated_at")]
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    [Column("target_band")]
    [Range(0, 9)]
    public float? TargetBand { get; set; }

    [Column("estimated_weeks")]
    public int? EstimatedWeeks { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    public ICollection<AIRoadmapStep> Steps { get; set; } = new List<AIRoadmapStep>();
}

/// <summary>
/// Individual step in an AI roadmap
/// </summary>
[Table("tb_ai_roadmap_steps")]
public class AIRoadmapStep
{
    [Key]
    [Column("step_id")]
    public int StepId { get; set; }

    [Column("roadmap_id")]
    public int? RoadmapId { get; set; }

    [Column("lesson_id")]
    public int? LessonId { get; set; }

    [Column("week_number")]
    public int? WeekNumber { get; set; }

    [Column("priority")]
    public int Priority { get; set; } = 0;

    [Column("is_completed")]
    public bool IsCompleted { get; set; } = false;

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("step_type")]
    [MaxLength(50)]
    public string? StepType { get; set; }

    [ForeignKey("RoadmapId")]
    public AIRoadmap? Roadmap { get; set; }

    [ForeignKey("LessonId")]
    public Lesson? Lesson { get; set; }
}

/// <summary>
/// AI analysis of user skill levels
/// </summary>
[Table("tb_ai_skill_analysis")]
public class AISkillAnalysis
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("reading_score")]
    [Range(0, 9)]
    public float? ReadingScore { get; set; }

    [Column("listening_score")]
    [Range(0, 9)]
    public float? ListeningScore { get; set; }

    [Column("writing_score")]
    [Range(0, 9)]
    public float? WritingScore { get; set; }

    [Column("speaking_score")]
    [Range(0, 9)]
    public float? SpeakingScore { get; set; }

    [NotMapped]
    public float? OverallBand
    {
        get
        {
            var scores = new[] { ReadingScore, ListeningScore, WritingScore, SpeakingScore }
                .Where(s => s.HasValue).Select(s => s!.Value).ToList();
            return scores.Any() ? (float)(Math.Round(scores.Average() * 2, MidpointRounding.AwayFromZero) / 2) : null;
        }
    }

    [Column("analyzed_at")]
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

    [Column("strengths")]
    public string? Strengths { get; set; }

    [Column("weaknesses")]
    public string? Weaknesses { get; set; }

    [Column("recommendations")]
    public string? Recommendations { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}

/// <summary>
/// Admin action log
/// </summary>
[Table("tb_admin_actions")]
public class AdminAction
{
    [Key]
    [Column("action_id")]
    public int ActionId { get; set; }

    [Column("admin_id")]
    public int? AdminId { get; set; }

    [Column("action_type")]
    [MaxLength(255)]
    public string? ActionType { get; set; }

    [Column("action_time")]
    public DateTime ActionTime { get; set; } = DateTime.UtcNow;

    [Column("target_entity")]
    public string? TargetEntity { get; set; }

    [Column("target_id")]
    public int? TargetId { get; set; }

    [Column("details")]
    public string? Details { get; set; }
}

/// <summary>
/// System settings (key-value)
/// </summary>
[Table("tb_system_settings")]
public class SystemSetting
{
    [Key]
    [Column("setting_key")]
    [MaxLength(255)]
    public string SettingKey { get; set; } = string.Empty;

    [Column("setting_value")]
    public string? SettingValue { get; set; }
}

#endregion

#region Practice Domain - Listening & Reading

/// <summary>
/// Listening material (dialogue, monologue, lecture, etc.)
/// </summary>
[Table("tb_listening_materials")]
public class ListeningMaterial
{
    [Key]
    [Column("material_id")]
    public int MaterialId { get; set; }

    [Column("lesson_id")]
    public int? LessonId { get; set; }

    [Column("material_type")]
    [MaxLength(50)]
    public string MaterialType { get; set; } = "dialogue";

    [Column("title")]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Column("transcript")]
    public string Transcript { get; set; } = string.Empty;

    [Column("audio_url")]
    public string? AudioUrl { get; set; }

    [Column("duration_seconds")]
    public int? DurationSeconds { get; set; }

    [Column("speaker_count")]
    public int? SpeakerCount { get; set; }

    [Column("topics")]
    [MaxLength(500)]
    public string? Topics { get; set; } // comma-separated: "work,travel,education"

    [Column("notes_for_learner")]
    public string? NotesForLearner { get; set; }

    [Column("difficulty_level")]
    [MaxLength(50)]
    public string DifficultyLevel { get; set; } = "band_5_6"; // band_3_4, band_5_6, band_7_8, band_8_9

    [Column("source")]
    [MaxLength(255)]
    public string? Source { get; set; } // Cambridge IELTS 18, etc.

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



    public ICollection<ListeningQuestion> Questions { get; set; } = new List<ListeningQuestion>();
}

/// <summary>
/// Listening question (fill-in-blank or multiple choice)
/// </summary>
[Table("tb_listening_questions")]
public class ListeningQuestion
{
    [Key]
    [Column("question_id")]
    public int QuestionId { get; set; }

    [Column("material_id")]
    public int? MaterialId { get; set; }

    [Column("question_type")]
    [MaxLength(50)]
    public string QuestionType { get; set; } = "fill_in_blank"; // fill_in_blank, multiple_choice

    [Column("question_text")]
    public string QuestionText { get; set; } = string.Empty;

    [Column("time_code_start")]
    public int? TimeCodeStart { get; set; } // in seconds

    [Column("time_code_end")]
    public int? TimeCodeEnd { get; set; }

    [Column("correct_answer")]
    public string CorrectAnswer { get; set; } = string.Empty;

    [Column("options")]
    public string? Options { get; set; } // JSON: ["A", "B", "C", "D"] for MC

    [Column("explanation")]
    public string? Explanation { get; set; }

    [Column("band_target")]
    [Range(0, 9)]
    public float? BandTarget { get; set; }

    [Column("question_order")]
    public int QuestionOrder { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("MaterialId")]
    public ListeningMaterial? Material { get; set; }

    public ICollection<UserPracticeAttempt> UserAttempts { get; set; } = new List<UserPracticeAttempt>();
}

/// <summary>
/// Reading passage
/// </summary>
[Table("tb_reading_passages")]
public class ReadingPassage
{
    [Key]
    [Column("passage_id")]
    public int PassageId { get; set; }

    [Column("lesson_id")]
    public int? LessonId { get; set; }

    [Column("passage_title")]
    [MaxLength(255)]
    public string PassageTitle { get; set; } = string.Empty;

    [Column("passage_text")]
    public string PassageText { get; set; } = string.Empty;

    [Column("word_count")]
    public int? WordCount { get; set; }

    [Column("difficulty_level")]
    [MaxLength(50)]
    public string DifficultyLevel { get; set; } = "band_5_6";

    [Column("topic_category")]
    [MaxLength(100)]
    public string? TopicCategory { get; set; } // academic, general, environment, etc.

    [Column("source")]
    [MaxLength(255)]
    public string? Source { get; set; }

    [Column("image_url")]
    [MaxLength(255)]
    public string? ImageUrl { get; set; }

    [Column("passage_translation")]
    public string? PassageTranslation { get; set; }

    [Column("vocab_highlights")]
    public string? VocabHighlights { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("LessonId")]
    public Lesson? Lesson { get; set; }

    public ICollection<ReadingQuestion> Questions { get; set; } = new List<ReadingQuestion>();
}

/// <summary>
/// Reading question (5 types)
/// </summary>
[Table("tb_reading_questions")]
public class ReadingQuestion
{
    [Key]
    [Column("question_id")]
    public int QuestionId { get; set; }

    [Column("passage_id")]
    public int? PassageId { get; set; }

    [Column("question_type")]
    [MaxLength(50)]
    public string QuestionType { get; set; } = "multiple_choice"; 
    // true_false_not_given, multiple_choice, matching_heading, sentence_completion, summary_completion

    [Column("question_number")]
    public int QuestionNumber { get; set; }

    [Column("question_text")]
    public string QuestionText { get; set; } = string.Empty;

    [Column("correct_answer")]
    public string CorrectAnswer { get; set; } = string.Empty;

    [Column("options")]
    public string? Options { get; set; } // JSON: ["A", "B", "C", "D"] or ["True", "False", "Not Given"]

    [Column("explanation")]
    public string? Explanation { get; set; }

    [Column("band_target")]
    [Range(0, 9)]
    public float? BandTarget { get; set; }

    [Column("paragraph_reference")]
    [MaxLength(10)]
    public string? ParagraphReference { get; set; } // "A", "B", "C", etc.

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("PassageId")]
    public ReadingPassage? Passage { get; set; }

    public ICollection<UserPracticeAttempt> UserAttempts { get; set; } = new List<UserPracticeAttempt>();
}

/// <summary>
/// User practice attempt (for listening/reading)
/// </summary>
[Table("tb_user_practice_attempts")]
public class UserPracticeAttempt
{
    [Key]
    [Column("attempt_id")]
    public int AttemptId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("skill_type")]
    [MaxLength(20)]
    public string SkillType { get; set; } = string.Empty; // listening, reading

    [Column("listening_question_id")]
    public int? ListeningQuestionId { get; set; }

    [Column("reading_question_id")]
    public int? ReadingQuestionId { get; set; }

    [Column("user_answer")]
    public string? UserAnswer { get; set; }

    [Column("is_correct")]
    public bool IsCorrect { get; set; } = false;

    [Column("time_spent_seconds")]
    public int? TimeSpentSeconds { get; set; }

    [Column("attempt_date")]
    public DateTime AttemptDate { get; set; } = DateTime.UtcNow;

    [Column("attempt_number")]
    public int AttemptNumber { get; set; } = 1;

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("ListeningQuestionId")]
    public ListeningQuestion? ListeningQuestion { get; set; }

    [ForeignKey("ReadingQuestionId")]
    public ReadingQuestion? ReadingQuestion { get; set; }
}

#endregion

#region Session Management & Notifications

/// <summary>
/// User session tracking (refresh tokens)
/// </summary>
[Table("tb_user_sessions")]
public class UserSession
{
    [Key]
    [Column("session_id")]
    public int SessionId { get; set; }

    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    [Column("refresh_token")]
    [MaxLength(500)]
    [Required]
    public string RefreshToken { get; set; } = string.Empty;

    [Column("token_family")]
    [MaxLength(255)]
    public string? TokenFamily { get; set; }

    [Column("expires_at")]
    [Required]
    public DateTime ExpiresAt { get; set; }

    [Column("revoked")]
    public bool Revoked { get; set; } = false;

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }

    [Column("device_info")]
    [MaxLength(255)]
    public string? DeviceInfo { get; set; }

    [Column("ip_address")]
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}

/// <summary>
/// Placement test results
/// </summary>
[Table("tb_user_placement_results")]
public class UserPlacementResult
{
    [Key]
    [Column("result_id")]
    public int ResultId { get; set; }

    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    [Column("test_id")]
    public int? TestId { get; set; }

    [Column("reading_score")]
    public int? ReadingScore { get; set; }

    [Column("listening_score")]
    public int? ListeningScore { get; set; }

    [Column("writing_score")]
    public int? WritingScore { get; set; }

    [Column("speaking_score")]
    public int? SpeakingScore { get; set; }

    [Column("overall_band")]
    public decimal? OverallBand { get; set; }

    [Column("completed_at")]
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    [Column("recommendations")]
    public string? Recommendations { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("TestId")]
    public Test? Test { get; set; }
}

/// <summary>
/// User notifications
/// </summary>
[Table("tb_notifications")]
public class Notification
{
    [Key]
    [Column("notification_id")]
    public int NotificationId { get; set; }

    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    [Column("type")]
    [MaxLength(50)]
    public string Type { get; set; } = "system";

    [Column("title")]
    [MaxLength(255)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [Column("message")]
    [Required]
    public string Message { get; set; } = string.Empty;

    [Column("data")]
    public string? Data { get; set; }

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}

/// <summary>
/// Writing prompts bank
/// </summary>
[Table("tb_writing_prompts")]
public class WritingPrompt
{
    [Key]
    [Column("prompt_id")]
    public int PromptId { get; set; }

    [Column("task_type")]
    [MaxLength(10)]
    [Required]
    public string TaskType { get; set; } = "task1"; // task1 or task2

    [Column("prompt_text")]
    [Required]
    public string PromptText { get; set; } = string.Empty;

    [Column("prompt_image_url")]
    [MaxLength(500)]
    public string? PromptImageUrl { get; set; }

    [Column("chart_type")]
    [MaxLength(50)]
    public string? ChartType { get; set; }

    [Column("difficulty_level")]
    public int DifficultyLevel { get; set; } = 1;

    [Column("category")]
    [MaxLength(100)]
    public string? Category { get; set; }

    [Column("band_target")]
    public decimal TargetBand { get; set; } = 6;

    [Column("sample_answer")]
    public string? SampleAnswer { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("notes_for_teacher")]
    public string? NotesForTeacher { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "draft";

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("reviewed_by")]
    public int? ReviewedBy { get; set; }

    [Column("reviewed_at")]
    public DateTime? ReviewedAt { get; set; }

    [Column("reviewer_note")]
    public string? ReviewerNote { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [ForeignKey("CreatedBy")]
    public User? Creator { get; set; }

    [ForeignKey("ReviewedBy")]
    public User? Reviewer { get; set; }
}

/// <summary>
/// Speaking topics
/// </summary>
[Table("tb_speaking_topics")]
public class SpeakingTopic
{
    [Key]
    [Column("topic_id")]
    public int TopicId { get; set; }

    [Column("part")]
    [Required]
    public int PartNumber { get; set; } = 1; // 1, 2, or 3

    [Column("topic_title")]
    [MaxLength(255)]
    [Required]
    public string TopicName { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("difficulty_level")]
    public int DifficultyLevel { get; set; } = 1;

    [Column("band_target")]
    public decimal TargetBand { get; set; } = 6;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "draft";

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("reviewed_by")]
    public int? ReviewedBy { get; set; }

    [Column("reviewed_at")]
    public DateTime? ReviewedAt { get; set; }

    [Column("reviewer_note")]
    public string? ReviewerNote { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [ForeignKey("CreatedBy")]
    public User? Creator { get; set; }

    [ForeignKey("ReviewedBy")]
    public User? Reviewer { get; set; }

    public ICollection<SpeakingTopicPart> Parts { get; set; } = new List<SpeakingTopicPart>();
}

/// <summary>
/// Speaking topic parts (cue cards, questions)
/// </summary>
[Table("tb_speaking_topic_parts")]
public class SpeakingTopicPart
{
    [Key]
    [Column("part_id")]
    public int PartId { get; set; }

    [Column("topic_id")]
    [Required]
    public int TopicId { get; set; }

    [Column("part_number")]
    public int? PartNumber { get; set; }

    [Column("content_text")]
    public string? ContentText { get; set; }

    [Column("time_limit_seconds")]
    public int? TimeLimitSeconds { get; set; }

    [Column("sequence_order")]
    public int? SequenceOrder { get; set; }

    [Column("is_follow_up")]
    public bool IsFollowUp { get; set; } = false;

    [ForeignKey("TopicId")]
    public SpeakingTopic? Topic { get; set; }
}

/// <summary>
/// Achievements/Badges
/// </summary>
[Table("tb_gamification")]
public class Achievement
{
    [Key]
    [Column("achievement_id")]
    public int AchievementId { get; set; }

    [Column("achievement_code")]
    [MaxLength(100)]
    [Required]
    public string AchievementCode { get; set; } = string.Empty;

    [Column("achievement_name")]
    [MaxLength(255)]
    [Required]
    public string AchievementName { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("icon_url")]
    [MaxLength(500)]
    public string? IconUrl { get; set; }

    [Column("points")]
    public int Points { get; set; } = 0;

    [Column("badge_color")]
    [MaxLength(20)]
    public string? BadgeColor { get; set; }

    [Column("criteria_json")]
    public string? CriteriaJson { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}

/// <summary>
/// User earned achievements
/// </summary>
[Table("tb_user_achievements")]
public class UserAchievement
{
    [Key]
    [Column("user_achievement_id")]
    public int UserAchievementId { get; set; }

    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    [Column("achievement_id")]
    [Required]
    public int AchievementId { get; set; }

    [Column("unlocked_at")]
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("AchievementId")]
    public Achievement? Achievement { get; set; }
}

/// <summary>
/// User daily activity tracking
/// </summary>
[Table("tb_user_daily_activity")]
public class UserDailyActivity
{
    [Key]
    [Column("activity_id")]
    public int ActivityId { get; set; }

    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    [Column("activity_date")]
    [Required]
    public DateTime ActivityDate { get; set; }

    [Column("lessons_completed")]
    public int LessonsCompleted { get; set; } = 0;

    [Column("questions_answered")]
    public int QuestionsAnswered { get; set; } = 0;

    [Column("writing_submissions")]
    public int WritingSubmissions { get; set; } = 0;

    [Column("speaking_sessions")]
    public int SpeakingSessions { get; set; } = 0;

    [Column("minutes_spent")]
    public int MinutesSpent { get; set; } = 0;

    [Column("xp_earned")]
    public int XpEarned { get; set; } = 0;

    [Column("streak_day")]
    public int StreakDay { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}

/// <summary>
/// System reports/feedback
/// </summary>
[Table("tb_reports")]
public class Report
{
    [Key]
    [Column("report_id")]
    public int ReportId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("report_type")]
    [MaxLength(50)]
    public string ReportType { get; set; } = "other";

    [Column("title")]
    [MaxLength(255)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    [Required]
    public string Description { get; set; } = string.Empty;

    [Column("priority")]
    [MaxLength(20)]
    public string Priority { get; set; } = "medium";

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "open";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("attachments_json")]
    public string? AttachmentsJson { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}

/// <summary>
/// Audit logs - System audit trail
/// </summary>
[Table("tb_audit_logs")]
public class AuditLog
{
    [Key]
    [Column("log_id")]
    public long LogId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("admin_id")]
    public int? AdminId { get; set; }

    [Column("action")]
    [MaxLength(255)]
    [Required]
    public string Action { get; set; } = string.Empty;

    [Column("entity_type")]
    [MaxLength(100)]
    public string? EntityType { get; set; }

    [Column("entity_id")]
    public int? EntityId { get; set; }

    [Column("old_values_json")]
    public string? OldValuesJson { get; set; }

    [Column("new_values_json")]
    public string? NewValuesJson { get; set; }

    [Column("ip_address")]
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [Column("user_agent")]
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "success";

    [Column("error_message")]
    public string? ErrorMessage { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public User? TargetUser { get; set; }

    [ForeignKey("AdminId")]
    public User? Admin { get; set; }
}

#endregion

#region Teacher Domain

/// <summary>
/// Conversation threads between students and teachers
/// </summary>
[Table("tb_conversations")]
public class Conversation
{
    [Key]
    [Column("conversation_id")]
    public int ConversationId { get; set; }

    [Column("student_id")]
    [Required]
    public int StudentId { get; set; }

    [Column("teacher_id")]
    public int? TeacherId { get; set; }

    [Column("subject")]
    [MaxLength(255)]
    public string? Subject { get; set; }

    [Column("status")]
    [MaxLength(20)]
    [Required]
    public string Status { get; set; } = "open"; // open, active, closed

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_message_at")]
    public DateTime? LastMessageAt { get; set; }

    [ForeignKey("StudentId")]
    public User? Student { get; set; }

    [ForeignKey("TeacherId")]
    public User? Teacher { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

/// <summary>
/// Individual messages within a conversation
/// </summary>
[Table("tb_messages")]
public class Message
{
    [Key]
    [Column("message_id")]
    public int MessageId { get; set; }

    [Column("conversation_id")]
    [Required]
    public int ConversationId { get; set; }

    [Column("sender_id")]
    [Required]
    public int SenderId { get; set; }

    [Column("content")]
    [Required]
    public string Content { get; set; } = "";

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ConversationId")]
    public Conversation? Conversation { get; set; }

    [ForeignKey("SenderId")]
    public User? Sender { get; set; }
}

#endregion

#region Teacher Expansion

[Table("tb_teacher_assignments")]
public class TeacherAssignment
{
    [Key][Column("assignment_id")]  public int AssignmentId { get; set; }
    [Column("test_id")]             public int TestId { get; set; }
    [Column("teacher_id")]          public int TeacherId { get; set; }
    [Column("student_id")]          public int StudentId { get; set; }
    [Column("title")]               public string Title { get; set; } = "";
    [Column("instructions")]        public string? Instructions { get; set; }
    [Column("deadline")]            public DateTime? Deadline { get; set; }
    [Column("status")]              public string Status { get; set; } = "pending";
    [Column("assigned_at")]         public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    [Column("completed_at")]        public DateTime? CompletedAt { get; set; }
    [Column("attempt_id")]          public int? AttemptId { get; set; }
    [Column("is_deleted")]          public bool IsDeleted { get; set; }
    public User? Teacher { get; set; }
    public User? Student { get; set; }
    public Test? Test { get; set; }

    [ForeignKey("AttemptId")]
    public UserTestAttempt? Attempt { get; set; }
}

[Table("tb_roadmap_suggestions")]
public class RoadmapSuggestion
{
    [Key][Column("suggestion_id")]    public int SuggestionId { get; set; }
    [Column("teacher_id")]            public int TeacherId { get; set; }
    [Column("student_id")]            public int StudentId { get; set; }
    [Column("roadmap_id")]            public int RoadmapId { get; set; }
    [Column("suggestion_title")]      public string SuggestionTitle { get; set; } = "";
    [Column("message")]               public string Message { get; set; } = "";
    [Column("suggested_changes")]     public string SuggestedChanges { get; set; } = "[]";
    [Column("status")]                public string Status { get; set; } = "pending";
    [Column("created_at")]            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("resolved_at")]           public DateTime? ResolvedAt { get; set; }
    public User? Teacher { get; set; }
    public User? Student { get; set; }
    public AIRoadmap? Roadmap { get; set; }
}

[Table("tb_announcements")]
public class Announcement
{
    [Key][Column("announcement_id")] public int AnnouncementId { get; set; }
    [Column("teacher_id")]           public int TeacherId { get; set; }
    [Column("title")]                public string Title { get; set; } = "";
    [Column("content")]              public string Content { get; set; } = "";
    [Column("is_active")]            public bool IsActive { get; set; } = true;
    [Column("created_at")]           public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("expires_at")]           public DateTime? ExpiresAt { get; set; }
    public User? Teacher { get; set; }
}

[Table("tb_teacher_profiles")]
public class TeacherProfile
{
    [Key][Column("teacher_id")]       public int TeacherId { get; set; }
    [Column("bio")]                   public string? Bio { get; set; }
    [Column("specialties")]           public string? Specialties { get; set; }
    [Column("years_experience")]      public int? YearsExperience { get; set; }
    [Column("is_public")]             public bool IsPublic { get; set; } = true;
    [Column("created_at")]            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")]            public DateTime? UpdatedAt { get; set; }
    public User? User { get; set; }
    public ICollection<TeacherCertificate> Certificates { get; set; } = new List<TeacherCertificate>();
}

[Table("tb_teacher_certificates")]
public class TeacherCertificate
{
    [Key][Column("certificate_id")]   public int CertificateId { get; set; }
    [Column("teacher_id")]            public int TeacherId { get; set; }
    [Column("title")]                 public string Title { get; set; } = "";
    [Column("image_url")]             public string ImageUrl { get; set; } = "";
    [Column("issue_date")]            public DateTime? IssueDate { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }
}

#endregion
