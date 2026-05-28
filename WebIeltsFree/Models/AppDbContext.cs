using Microsoft.EntityFrameworkCore;

namespace WebIeltsFree.Models;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    #region User Domain
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }
    public DbSet<UserLearningProgress> UserLearningProgress { get; set; }
    #endregion

    #region Learning Domain
    public DbSet<Course> Courses { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<LessonContent> LessonContents { get; set; }
    public DbSet<Vocabulary> Vocabulary { get; set; }
    #endregion

    #region Test Domain
    public DbSet<Test> Tests { get; set; }
    public DbSet<TestSection> TestSections { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }
    public DbSet<UserTestAttempt> UserTestAttempts { get; set; }
    #endregion

    #region Skills Domain
    public DbSet<SpeakingSession> SpeakingSessions { get; set; }
    public DbSet<WritingSubmission> WritingSubmissions { get; set; }
    #endregion

    #region Practice Domain - Listening & Reading
    public DbSet<ListeningMaterial> ListeningMaterials { get; set; }
    public DbSet<ListeningQuestion> ListeningQuestions { get; set; }
    public DbSet<ReadingPassage> ReadingPassages { get; set; }
    public DbSet<ReadingQuestion> ReadingQuestions { get; set; }
    public DbSet<UserPracticeAttempt> UserPracticeAttempts { get; set; }
    #endregion

    #region AI Domain
    public DbSet<AIRoadmap> AIRoadmaps { get; set; }
    public DbSet<AIRoadmapStep> AIRoadmapSteps { get; set; }
    public DbSet<AISkillAnalysis> AISkillAnalyses { get; set; }
    public DbSet<AdminAction> AdminActions { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    #endregion

    #region 
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<UserPlacementResult> UserPlacementResults { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<WritingPrompt> WritingPrompts { get; set; }
    public DbSet<SpeakingTopic> SpeakingTopics { get; set; }
    public DbSet<SpeakingTopicPart> SpeakingTopicParts { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<UserDailyActivity> UserDailyActivities { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    #endregion

    #region Teacher Domain
    public DbSet<GradeDispute> GradeDisputes { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<TeacherAssignment> TeacherAssignments { get; set; }
    public DbSet<RoadmapSuggestion> RoadmapSuggestions { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<TeacherProfile> TeacherProfiles { get; set; }
    public DbSet<TeacherCertificate> TeacherCertificates { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User -> UserProfile (1:1)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId);

        // User -> UserGoal (1:1)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Goal)
            .WithOne(g => g.User)
            .HasForeignKey<UserGoal>(g => g.UserId);

        // User -> LearningProgress (1:N)
        modelBuilder.Entity<User>()
            .HasMany(u => u.LearningProgress)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);

        // User -> TestAttempts (1:N)
        modelBuilder.Entity<User>()
            .HasMany(u => u.TestAttempts)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId);

        // User -> SpeakingSessions (1:N)
        modelBuilder.Entity<User>()
            .HasMany(u => u.SpeakingSessions)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId);

        // User -> WritingSubmissions (1:N)
        modelBuilder.Entity<User>()
            .HasMany(u => u.WritingSubmissions)
            .WithOne(w => w.User)
            .HasForeignKey(w => w.UserId);

        // User -> PracticeAttempts (1:N)
        modelBuilder.Entity<User>()
            .HasMany(u => u.PracticeAttempts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .IsRequired(false);

        // Listening -> Questions (1:N)
        modelBuilder.Entity<ListeningMaterial>()
            .HasMany(m => m.Questions)
            .WithOne(q => q.Material)
            .HasForeignKey(q => q.MaterialId);

        // Reading -> Questions (1:N)
        modelBuilder.Entity<ReadingPassage>()
            .HasMany(p => p.Questions)
            .WithOne(q => q.Passage)
            .HasForeignKey(q => q.PassageId);

        // Course -> Modules (1:N)
        modelBuilder.Entity<Course>()
            .HasMany(c => c.Modules)
            .WithOne(m => m.Course)
            .HasForeignKey(m => m.CourseId);

        // Module -> Lessons (1:N)
        modelBuilder.Entity<Module>()
            .HasMany(m => m.Lessons)
            .WithOne(l => l.Module)
            .HasForeignKey(l => l.ModuleId);

        // Lesson -> Contents (1:N)
        modelBuilder.Entity<Lesson>()
            .HasMany(l => l.Contents)
            .WithOne(c => c.Lesson)
            .HasForeignKey(c => c.LessonId);

        // Test -> Sections (1:N)
        modelBuilder.Entity<Test>()
            .HasMany(t => t.Sections)
            .WithOne(s => s.Test)
            .HasForeignKey(s => s.TestId);

        // TestSection -> Questions (1:N)
        modelBuilder.Entity<TestSection>()
            .HasMany(s => s.Questions)
            .WithOne(q => q.Section)
            .HasForeignKey(q => q.SectionId);

        // Question -> Answer (1:1)
        modelBuilder.Entity<Question>()
            .HasOne(q => q.Answer)
            .WithOne(a => a.Question)
            .HasForeignKey<Answer>(a => a.QuestionId);

        // UserTestAttempt -> UserAnswers (1:N)
        modelBuilder.Entity<UserTestAttempt>()
            .HasMany(a => a.Answers)
            .WithOne(ua => ua.Attempt)
            .HasForeignKey(ua => ua.AttemptId);

        // AIRoadmap -> Steps (1:N)
        modelBuilder.Entity<AIRoadmap>()
            .HasMany(r => r.Steps)
            .WithOne(s => s.Roadmap)
            .HasForeignKey(s => s.RoadmapId);

        // Unique constraint on User.Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // SystemSetting uses SettingKey as primary key (string)
        modelBuilder.Entity<SystemSetting>()
            .HasKey(s => s.SettingKey);

        // GradeDispute -> User FK
        modelBuilder.Entity<GradeDispute>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(u => u.GradeDisputes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Reviewer)
                .WithMany(u => u.ReviewedDisputes)
                .HasForeignKey(d => d.ReviewedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Conversation -> Student FK
        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Student)
            .WithMany()
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Conversation -> Teacher FK
        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Teacher)
            .WithMany()
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Conversation -> Messages (1:N)
        modelBuilder.Entity<Conversation>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId);

        // Message -> Sender FK
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // SpeakingTopic configuration
        modelBuilder.Entity<SpeakingTopic>()
            .Property(s => s.PartNumber)
            .HasConversion(
                v => v.ToString(),
                v => int.Parse(v));

        modelBuilder.Entity<SpeakingTopic>()
            .Property(s => s.TargetBand)
            .HasConversion(
                v => (int)v,
                v => (decimal)v);

        // WritingPrompt configuration
        modelBuilder.Entity<WritingPrompt>()
            .Property(w => w.TargetBand)
            .HasConversion(
                v => (int)v,
                v => (decimal)v);

        // TeacherProfile -> User (1:1, shares PK)
        modelBuilder.Entity<TeacherProfile>()
            .HasOne(tp => tp.User)
            .WithOne()
            .HasForeignKey<TeacherProfile>(tp => tp.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // TeacherProfile -> Certificates (1:N)
        modelBuilder.Entity<TeacherProfile>()
            .HasMany(tp => tp.Certificates)
            .WithOne(c => c.TeacherProfile)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
