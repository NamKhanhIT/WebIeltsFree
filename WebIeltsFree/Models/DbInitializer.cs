using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace WebIeltsFree.Models;

/// <summary>
/// Database initializer that seeds courses, modules, and lessons on application startup
/// </summary>
public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        try
        {
            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_writing_submissions ADD COLUMN task_type INT DEFAULT 2;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_writing_submissions ADD COLUMN word_count INT DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_writing_submissions ADD COLUMN ta_score DECIMAL(3,1) DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_writing_submissions ADD COLUMN cc_score DECIMAL(3,1) DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_writing_submissions ADD COLUMN lr_score DECIMAL(3,1) DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_writing_submissions ADD COLUMN gra_score DECIMAL(3,1) DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_writing_submissions ADD COLUMN ai_feedback LONGTEXT DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_writing_submissions ADD COLUMN feedback_details LONGTEXT DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_reading_passages ADD COLUMN image_url VARCHAR(255) DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_reading_passages ADD COLUMN passage_translation LONGTEXT DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_reading_passages ADD COLUMN vocab_highlights LONGTEXT DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_test_sections ADD COLUMN audio_url VARCHAR(500) DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_teacher_assignments ADD COLUMN attempt_id INT DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_answers ADD PRIMARY KEY (answer_id);");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_answers MODIFY COLUMN answer_id INT AUTO_INCREMENT;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_tests ADD COLUMN status VARCHAR(20) NOT NULL DEFAULT 'published';");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_tests ADD COLUMN reviewed_by INT DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_tests ADD COLUMN reviewed_at DATETIME DEFAULT NULL;");
            }
            catch { }

            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE tb_tests ADD COLUMN reviewer_note TEXT DEFAULT NULL;");
            }
            catch { }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database migration helper error: {ex.Message}");
        }

        UpdateReadingPassageMetaData(context);

        if (context.Courses.Any())
            return; 

        SeedCourses(context);
        SeedModules(context);
        SeedLessons(context);
        SeedUsers(context);
    }


    private static void SeedCourses(AppDbContext context)
    {
        var courses = new Course[]
        {
            new Course
            {
                Title = "IELTS Listening Mastery",
                TargetBand = 6.5f,
                Description = "Complete listening preparation with real exam practices covering social and academic contexts"
            },
            new Course
            {
                Title = "IELTS Reading Excellence",
                TargetBand = 6.5f,
                Description = "Advanced reading strategies and comprehension techniques for all passage types"
            },
            new Course
            {
                Title = "IELTS Writing Perfection",
                TargetBand = 6.5f,
                Description = "Master Task 1 (Data Description) and Task 2 (Essay Writing)"
            },
            new Course
            {
                Title = "IELTS Speaking Confidence",
                TargetBand = 6.5f,
                Description = "Speaking Parts 1, 2, 3 with fluency and pronunciation training"
            }
        };

        foreach (var course in courses)
        {
            context.Courses.Add(course);
        }

        context.SaveChanges();
    }

    private static void SeedModules(AppDbContext context)
    {
        var listeningCourse = context.Courses.FirstOrDefault(c => c.Title == "IELTS Listening Mastery");
        var readingCourse = context.Courses.FirstOrDefault(c => c.Title == "IELTS Reading Excellence");
        var writingCourse = context.Courses.FirstOrDefault(c => c.Title == "IELTS Writing Perfection");
        var speakingCourse = context.Courses.FirstOrDefault(c => c.Title == "IELTS Speaking Confidence");

        var modules = new List<Module>();

        // Listening Modules
        if (listeningCourse != null)
        {
            modules.AddRange(new Module[]
            {
                new Module { CourseId = listeningCourse.CourseId, Title = "Part 1: Everyday Conversation (Form & Note Completion)", OrderIndex = 1 },
                new Module { CourseId = listeningCourse.CourseId, Title = "Part 2: Monologue in Social Context (Map & Labelling)", OrderIndex = 2 },
                new Module { CourseId = listeningCourse.CourseId, Title = "Part 3: Academic Discussion (Matching & MCQ)", OrderIndex = 3 },
                new Module { CourseId = listeningCourse.CourseId, Title = "Part 4: Academic Lecture (Summary & Sentence Completion)", OrderIndex = 4 }
            });
        }

        // Reading Modules
        if (readingCourse != null)
        {
            modules.AddRange(new Module[]
            {
                new Module { CourseId = readingCourse.CourseId, Title = "Passage Strategy: Skimming, Scanning, and Time Control", OrderIndex = 1 },
                new Module { CourseId = readingCourse.CourseId, Title = "True/False/Not Given + Writer Views/Claims", OrderIndex = 2 },
                new Module { CourseId = readingCourse.CourseId, Title = "Matching Headings / Information / Features", OrderIndex = 3 },
                new Module { CourseId = readingCourse.CourseId, Title = "Completion Tasks: Sentence, Summary, Table, Diagram", OrderIndex = 4 }
            });
        }

        // Writing Modules
        if (writingCourse != null)
        {
            modules.AddRange(new Module[]
            {
                new Module { CourseId = writingCourse.CourseId, Title = "Task 1 Academic: Line/Bar/Pie/Table Reports", OrderIndex = 1 },
                new Module { CourseId = writingCourse.CourseId, Title = "Task 1 Academic: Process and Map Reports", OrderIndex = 2 },
                new Module { CourseId = writingCourse.CourseId, Title = "Task 2: Opinion & Discussion Essays", OrderIndex = 3 },
                new Module { CourseId = writingCourse.CourseId, Title = "Task 2: Problem-Solution & Advantage-Disadvantage", OrderIndex = 4 }
            });
        }

        if (speakingCourse != null)
        {
            modules.AddRange(new Module[]
            {
                new Module { CourseId = speakingCourse.CourseId, Title = "Part 1: Personal Topics and Natural Interaction", OrderIndex = 1 },
                new Module { CourseId = speakingCourse.CourseId, Title = "Part 2: Cue Card Planning and 2-minute Delivery", OrderIndex = 2 },
                new Module { CourseId = speakingCourse.CourseId, Title = "Part 3: Abstract Discussion and Idea Development", OrderIndex = 3 },
                new Module { CourseId = speakingCourse.CourseId, Title = "Pronunciation, Fluency, and Lexical Range for Band 7+", OrderIndex = 4 }
            });
        }

        foreach (var module in modules)
        {
            context.Modules.Add(module);
        }

        context.SaveChanges();
    }

    private static void SeedLessons(AppDbContext context)
    {
        var modules = context.Modules.ToList();
        var lessons = new List<Lesson>();

        foreach (var module in modules)
        {
            var course = context.Courses.FirstOrDefault(c => c.CourseId == module.CourseId);
            var skillType = course?.Title switch
            {
                "IELTS Listening Mastery" => "listening",
                "IELTS Reading Excellence" => "reading",
                "IELTS Writing Perfection" => "writing",
                "IELTS Speaking Confidence" => "speaking",
                _ => "reading"
            };
            var lessonTitles = GetLessonTitles(module.Title ?? string.Empty, skillType);
            for (var i = 0; i < lessonTitles.Count; i++)
            {
                lessons.Add(new Lesson
                {
                    ModuleId = module.ModuleId,
                    Title = lessonTitles[i],
                    SkillType = skillType,
                    DifficultyLevel = i <= 1 ? 1 : (i <= 3 ? 2 : 3),
                    EstimatedMinutes = 20 + (i * 8)
                });
            }
        }

        foreach (var lesson in lessons)
        {
            context.Lessons.Add(lesson);
        }

        context.SaveChanges();
    }

    private static List<string> GetLessonTitles(string moduleTitle, string skillType)
    {
        if (skillType == "listening")
        {
            if (moduleTitle.Contains("Part 1"))
                return new() { "Booking Form Completion", "Daily Services Conversation", "Number & Date Dictation", "Distractor Awareness in Part 1", "Mini Mock: Part 1 (10 Questions)" };
            if (moduleTitle.Contains("Part 2"))
                return new() { "Campus Orientation Map", "City Tour Monologue", "Direction Language and Landmarks", "Diagram/Plan Labelling Practice", "Mini Mock: Part 2 (10 Questions)" };
            if (moduleTitle.Contains("Part 3"))
                return new() { "Tutor-Student Research Discussion", "Matching Opinions to Speakers", "Academic Multiple Choice Tactics", "Paraphrase Recognition in Seminars", "Mini Mock: Part 3 (10 Questions)" };
            return new() { "Lecture Signposting and Prediction", "Note Completion from Lecture", "Sentence Completion under Time Pressure", "Section 4 High-speed Listening Drill", "Mini Mock: Part 4 (10 Questions)" };
        }

        if (skillType == "reading")
        {
            if (moduleTitle.Contains("Passage Strategy"))
                return new() { "3-Passage Time Allocation", "Skimming for Main Ideas", "Scanning for Names, Numbers, Dates", "Keyword and Paraphrase Mapping", "60-minute Reading Simulation" };
            if (moduleTitle.Contains("True/False/Not Given"))
                return new() { "True vs False vs Not Given Logic", "Writer Views and Claims", "Evidence Line Hunting", "Trap Statement Patterns", "Mixed TFNG and Views Practice Set" };
            if (moduleTitle.Contains("Matching Headings"))
                return new() { "Heading Selection Framework", "Matching Information to Paragraphs", "Feature Matching with Classification", "Sentence Ending Matching", "Mixed Matching Practice Set" };
            return new() { "Sentence Completion with Word Limit", "Summary and Note Completion", "Table and Flow-chart Completion", "Diagram Labelling Strategy", "Completion-heavy Reading Simulation" };
        }

        if (skillType == "writing")
        {
            if (moduleTitle.Contains("Line/Bar/Pie/Table"))
                return new() { "Task 1 Structure: Intro + Overview + Details", "Line Graph Trend Language", "Bar/Pie Comparative Analysis", "Table Data Grouping", "Task 1 Timed Report (20 mins)" };
            if (moduleTitle.Contains("Process and Map"))
                return new() { "Process Report Sequencing", "Passive Voice for Process Writing", "Map Comparison (Past vs Present)", "Task 1 Coherence and Linking", "Task 1 Band 7 Checklist Practice" };
            if (moduleTitle.Contains("Opinion & Discussion"))
                return new() { "Opinion Essay Positioning", "Discussion Essay Balance", "Thesis Statements and Topic Sentences", "Idea Development with Examples", "Task 2 Timed Essay (40 mins)" };
            return new() { "Problem-Solution Essay Logic", "Advantages-Disadvantages Essay", "Counterargument and Refutation", "Grammar Range for Band 7+", "Task 2 Band Descriptor Self-review" };
        }

        if (moduleTitle.Contains("Part 1"))
            return new() { "Hometown and Daily Routine", "Work/Study Topic Expansion", "Likes, Dislikes, and Reasons", "Natural Fluency without Over-long Answers", "Part 1 Interview Drill" };
        if (moduleTitle.Contains("Part 2"))
            return new() { "Cue Card Note Planning in 1 Minute", "Story Structure for 2-minute Talk", "Using High-value Vocabulary Naturally", "Handling Follow-up Questions", "Part 2 Timed Recording Practice" };
        if (moduleTitle.Contains("Part 3"))
            return new() { "Abstract Question Handling", "Developing Multi-layered Ideas", "Compare, Cause, Predict Framework", "Agree/Disagree with Nuance", "Part 3 Discussion Simulation" };
        return new() { "Pronunciation Chunking and Stress", "Linking and Rhythm for Fluency", "Grammar Accuracy under Pressure", "Lexical Resource Upgrade", "Speaking Band 7 Performance Mock" };
    }

    private static void SeedUsers(AppDbContext context)
    {
        if (context.Users.Any(u => u.Role == "admin" || u.Role == "teacher"))
            return;

        var admin = new User
        {
            Email = "admin@webieltsfree.com",
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "admin",
            Status = "active",
            EmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };

        var teacher = new User
        {
            Email = "teacher@webieltsfree.com",
            Username = "teacher",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
            Role = "teacher",
            Status = "active",
            EmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(admin, teacher);
        context.SaveChanges();

        context.UserProfiles.AddRange(
            new UserProfile { UserId = admin.UserId, FullName = "System Administrator", CreatedAt = DateTime.UtcNow },
            new UserProfile { UserId = teacher.UserId, FullName = "IELTS Senior Teacher", CreatedAt = DateTime.UtcNow }
        );

        context.SaveChanges();
    }

    private static void UpdateReadingPassageMetaData(AppDbContext context)
    {
        try
        {
            var p1 = context.ReadingPassages.FirstOrDefault(p => p.PassageId == 2001 || p.PassageTitle.Contains("Urban Green"));
            if (p1 != null)
            {
                p1.LessonId = 2005;
                p1.ImageUrl = "https://images.unsplash.com/photo-1542601906990-b4d3fb778b09?q=80&w=1200";
            }

            var p2 = context.ReadingPassages.FirstOrDefault(p => p.PassageId == 2002 || p.PassageTitle.Contains("Sleep"));
            if (p2 != null)
            {
                p2.LessonId = 2006;
                p2.ImageUrl = "https://images.unsplash.com/photo-1511295742364-92767fc4a28f?q=80&w=1200";
            }

            var p3 = context.ReadingPassages.FirstOrDefault(p => p.PassageId == 2003 || p.PassageTitle.Contains("Artificial"));
            if (p3 != null)
            {
                p3.LessonId = 2007;
                p3.ImageUrl = "https://images.unsplash.com/photo-1576091160399-112ba8d25d1d?q=80&w=1200";
            }

            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating reading passage metadata: {ex.Message}");
        }
    }
}

