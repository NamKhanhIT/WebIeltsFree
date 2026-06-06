# 🎯 WebIeltsFree - Phân Tích Toàn Diện & Kế Hoạch Tối Ưu

## Tổng Quan Dự Án Hiện Tại

Dự án WebIeltsFree là một nền tảng học IELTS miễn phí sử dụng AI, được xây dựng trên ASP.NET Core 8 (MVC + Web API) với MySQL database. Backend đã có **10 controllers**, **~970 dòng entity models**, **~700 dòng DTOs**, tích hợp Gemini AI, JWT authentication, rate limiting, và Docker support.

---

## 🔴 PHẦN 1: PHÂN TÍCH ĐIỂM YẾU & THIẾU SÓT

### 1.1 Database - Điểm Yếu Nghiêm Trọng

| Vấn đề | Mức độ | Chi tiết |
|---------|--------|----------|
| **Thiếu Foreign Key constraints** | 🔴 Critical | Nhiều bảng thiếu FK: `tb_ai_roadmaps.user_id`, `tb_speaking_sessions.user_id`, `tb_writing_submissions.user_id`, `tb_user_goals.user_id`, `tb_user_learning_progress.user_id/lesson_id`, `tb_user_test_attempts.user_id/test_id`, `tb_user_answers.attempt_id/question_id` |
| **Thiếu Indexes** | 🔴 Critical | Các bảng `tb_user_goals`, `tb_user_learning_progress`, `tb_user_test_attempts`, `tb_writing_submissions`, `tb_speaking_sessions` thiếu index trên `user_id` — query sẽ rất chậm khi scale |
| **Dữ liệu trùng lặp** | 🟡 Medium | `tb_vocabulary` có cả `meaning` + `definition`, `example` + `example_sentence` — cần normalize |
| **Thiếu bảng `tb_user_placement_results`** | 🔴 Critical | Placement test đã có bảng `tb_placement_tests` nhưng thiếu bảng lưu kết quả user |
| **Thiếu bảng `tb_learning_paths`** | 🟡 Medium | `tb_ai_roadmaps` không đủ trường để lưu detailed learning path |
| **Thiếu `updated_at` timestamps** | 🟡 Medium | Hầu hết bảng chỉ có `created_at`, thiếu `updated_at` cho audit trail |
| **Thiếu soft delete** | 🟡 Medium | Không có `is_deleted`/`deleted_at` — xóa data sẽ mất vĩnh viễn |
| **Thiếu `tb_notifications`** | 🟡 Medium | Không có hệ thống thông báo cho user |
| **Thiếu `tb_user_sessions`** | 🟡 Medium | Không quản lý refresh token riêng |
| **Thiếu `tb_writing_prompts`** | 🟡 Medium | Writing prompts hardcode trong code, không có bảng riêng |
| **Thiếu `tb_speaking_topics`** | 🟡 Medium | Speaking topics không có bảng quản lý |

### 1.2 Backend - Điểm Yếu

| Vấn đề | Mức độ | Chi tiết |
|---------|--------|----------|
| **API Key lộ trong `appsettings.json`** | 🔴 Critical | Gemini API key `AIzaSy...` hardcode trong source code — **BẢO MẬT NGHIÊM TRỌNG** |
| **JWT Secret hardcode** | 🔴 Critical | `IeltsAIProjectSuperSecretKey2026!@#$` — phải dùng environment variables |
| **DB Connection string hardcode** | 🔴 Critical | `Server=localhost;Port=3307;Database=ieltsdb;User=root;Password=;` — root user, không password |
| **CORS AllowAll** | 🟡 Medium | Production không nên dùng `AllowAnyOrigin()` |
| **In-memory cache thay vì Redis** | 🟡 Medium | `CacheService` dùng `Dictionary` — mất khi restart, không scale được |
| **In-memory vector search** | 🟡 Medium | `VectorSearchService` dùng keyword matching — không phải real vector search |
| **Spaced Repetition in-memory** | 🟡 Medium | SM-2 algorithm data mất khi restart |
| **Thiếu MongoDB integration** | 🔴 Critical | Code mention MongoDB nhưng chưa implement, chưa có NuGet package |
| **Thiếu Pinecone integration** | 🔴 Critical | Vector search là mock, chưa connect Pinecone thật |
| **Thiếu SignalR hubs** | 🟡 Medium | PROJECT_SUMMARY mention SignalR nhưng chưa implement |
| **SecurityValidator dùng `Random` thay vì `RandomNumberGenerator`** | 🟡 Medium | Token generation không cryptographically secure |
| **Empty catch blocks** | 🟡 Medium | Nhiều `catch { }` nuốt lỗi — khó debug |
| **Thiếu Email service** | 🟡 Medium | Forgot password chỉ là stub, không gửi email thật |

### 1.3 Frontend - Điểm Yếu

| Vấn đề | Mức độ | Chi tiết |
|---------|--------|----------|
| **CHƯA CÓ FRONTEND NEXT.JS** | 🔴 Critical | Hiện tại chỉ có MVC Views (Razor), chưa có Next.js frontend riêng |
| **Thiếu trang Landing Page** | 🔴 Critical | Không có trang giới thiệu chuyên nghiệp |
| **Thiếu Dashboard** | 🔴 Critical | Không có trang tổng quan cho học viên |
| **Thiếu Interactive Tutorial** | 🟡 Medium | Onboarding flow chưa có UI |
| **Thiếu Skill Tree UI** | 🟡 Medium | Learning roadmap chưa có giao diện game-like |
| **Thiếu Offline Support** | 🟡 Medium | Chưa có Service Worker, IndexedDB |

### 1.4 Dữ Liệu - Điểm Yếu

| Vấn đề | Mức độ | Chi tiết |
|---------|--------|----------|
| **Quá ít dữ liệu thật** | 🔴 Critical | Chỉ có 3 listening materials, 2 reading passages, 3 vocabulary words, 5 writing submissions, 7 questions |
| **Không có courses** | 🔴 Critical | Bảng `tb_courses` trống |
| **Không có modules** | 🔴 Critical | Bảng `tb_modules` trống |
| **Không có lessons** | 🔴 Critical | Bảng `tb_lessons` trống |
| **Không có placement test questions** | 🔴 Critical | Bảng `tb_test_questions` trống |
| **Không có audio files** | 🟡 Medium | `audio_url` đều NULL cho listening materials |

---

## 🟢 PHẦN 2: KẾ HOẠCH TỐI ƯU DATABASE

### 2.1 MySQL Schema Optimization

#### Bảng cần thêm mới

```
tb_user_placement_results    — Kết quả placement test
tb_user_sessions             — Manage refresh tokens
tb_notifications             — Hệ thống thông báo
tb_writing_prompts           — Bank đề writing
tb_speaking_topics           — Bank đề speaking
tb_speaking_topic_parts      — Speaking cue cards (Part 1/2/3)
tb_gamification              — XP, levels, badges, achievements
tb_user_achievements         — User's earned achievements
tb_user_daily_activity       — Track daily login/practice
tb_reports                   — User report/feedback system
tb_audit_logs                — System audit trail
```

#### Bảng cần sửa đổi

**tb_users** — Thêm:
- `username VARCHAR(50) UNIQUE` 
- `email_verified BOOLEAN DEFAULT FALSE`
- `email_verification_token VARCHAR(255)`
- `last_login_at TIMESTAMP`
- `login_count INT DEFAULT 0`
- `is_deleted BOOLEAN DEFAULT FALSE`
- `deleted_at TIMESTAMP NULL`
- `updated_at TIMESTAMP`

**tb_courses** — Thêm:
- `skill_type ENUM('reading','listening','writing','speaking','vocabulary','grammar','mixed')`
- `difficulty_level INT`
- `thumbnail_url VARCHAR(500)`
- `estimated_hours INT`
- `is_published BOOLEAN DEFAULT FALSE`
- `created_at TIMESTAMP`
- `updated_at TIMESTAMP`
- `order_index INT`
- `slug VARCHAR(255) UNIQUE`

**tb_vocabulary** — Sửa:
- Xóa trùng lặp (`meaning`/`definition`, `example`/`example_sentence`)
- Thêm: `phonetic VARCHAR(100)`, `audio_url VARCHAR(500)`, `synonyms TEXT`, `antonyms TEXT`, `part_of_speech VARCHAR(50)`, `ielts_topic VARCHAR(100)`, `usage_frequency ENUM('high','medium','low')`

**tb_ai_roadmap_steps** — Thêm (nếu chưa có trong MySQL):
- `priority INT DEFAULT 0`
- `is_completed BOOLEAN DEFAULT FALSE`
- `completed_at TIMESTAMP NULL`
- `step_type VARCHAR(50)` — lesson/test/practice/review
- `description TEXT`

#### Index Optimization

```sql
-- Critical performance indexes
ALTER TABLE tb_user_goals ADD INDEX idx_user_goals_user (user_id);
ALTER TABLE tb_user_learning_progress ADD INDEX idx_progress_user (user_id);
ALTER TABLE tb_user_learning_progress ADD INDEX idx_progress_lesson (lesson_id);
ALTER TABLE tb_user_learning_progress ADD INDEX idx_progress_user_lesson (user_id, lesson_id);
ALTER TABLE tb_user_test_attempts ADD INDEX idx_test_user (user_id);
ALTER TABLE tb_user_test_attempts ADD INDEX idx_test_test (test_id);
ALTER TABLE tb_writing_submissions ADD INDEX idx_writing_user (user_id);
ALTER TABLE tb_writing_submissions ADD INDEX idx_writing_created (created_at);
ALTER TABLE tb_speaking_sessions ADD INDEX idx_speaking_user (user_id);
ALTER TABLE tb_speaking_sessions ADD INDEX idx_speaking_created (created_at);
ALTER TABLE tb_ai_roadmaps ADD INDEX idx_roadmap_user (user_id);
ALTER TABLE tb_ai_skill_analysis ADD INDEX idx_skill_user (user_id);
ALTER TABLE tb_vocabulary ADD INDEX idx_vocab_band (band_level);
ALTER TABLE tb_vocabulary ADD INDEX idx_vocab_category (category);
ALTER TABLE tb_lessons ADD INDEX idx_lesson_skill (skill_type);
ALTER TABLE tb_lessons ADD INDEX idx_lesson_module (module_id);
ALTER TABLE tb_courses ADD INDEX idx_course_skill (skill_type);
```

#### Foreign Key Constraints cần thêm

```sql
-- User relationships
ALTER TABLE tb_user_goals ADD CONSTRAINT fk_goal_user 
  FOREIGN KEY (user_id) REFERENCES tb_users(user_id) ON DELETE CASCADE;
ALTER TABLE tb_user_learning_progress ADD CONSTRAINT fk_progress_user 
  FOREIGN KEY (user_id) REFERENCES tb_users(user_id) ON DELETE CASCADE;
ALTER TABLE tb_user_learning_progress ADD CONSTRAINT fk_progress_lesson 
  FOREIGN KEY (lesson_id) REFERENCES tb_lessons(lesson_id) ON DELETE CASCADE;
ALTER TABLE tb_user_test_attempts ADD CONSTRAINT fk_attempt_user 
  FOREIGN KEY (user_id) REFERENCES tb_users(user_id) ON DELETE CASCADE;
ALTER TABLE tb_user_test_attempts ADD CONSTRAINT fk_attempt_test 
  FOREIGN KEY (test_id) REFERENCES tb_tests(test_id) ON DELETE CASCADE;
ALTER TABLE tb_user_answers ADD CONSTRAINT fk_answer_attempt 
  FOREIGN KEY (attempt_id) REFERENCES tb_user_test_attempts(attempt_id) ON DELETE CASCADE;
ALTER TABLE tb_user_answers ADD CONSTRAINT fk_answer_question 
  FOREIGN KEY (question_id) REFERENCES tb_questions(question_id) ON DELETE CASCADE;
ALTER TABLE tb_writing_submissions ADD CONSTRAINT fk_writing_user 
  FOREIGN KEY (user_id) REFERENCES tb_users(user_id) ON DELETE CASCADE;
ALTER TABLE tb_speaking_sessions ADD CONSTRAINT fk_speaking_user 
  FOREIGN KEY (user_id) REFERENCES tb_users(user_id) ON DELETE CASCADE;
ALTER TABLE tb_ai_roadmaps ADD CONSTRAINT fk_roadmap_user 
  FOREIGN KEY (user_id) REFERENCES tb_users(user_id) ON DELETE CASCADE;
ALTER TABLE tb_ai_skill_analysis ADD CONSTRAINT fk_analysis_user 
  FOREIGN KEY (user_id) REFERENCES tb_users(user_id) ON DELETE CASCADE;
ALTER TABLE tb_admin_actions ADD CONSTRAINT fk_admin_action_user 
  FOREIGN KEY (admin_id) REFERENCES tb_users(user_id) ON DELETE SET NULL;

-- Learning hierarchy
ALTER TABLE tb_modules ADD CONSTRAINT fk_module_course 
  FOREIGN KEY (course_id) REFERENCES tb_courses(course_id) ON DELETE CASCADE;
ALTER TABLE tb_lessons ADD CONSTRAINT fk_lesson_module 
  FOREIGN KEY (module_id) REFERENCES tb_modules(module_id) ON DELETE CASCADE;
ALTER TABLE tb_lesson_contents ADD CONSTRAINT fk_content_lesson 
  FOREIGN KEY (lesson_id) REFERENCES tb_lessons(lesson_id) ON DELETE CASCADE;
ALTER TABLE tb_questions ADD CONSTRAINT fk_question_section 
  FOREIGN KEY (section_id) REFERENCES tb_test_sections(section_id) ON DELETE CASCADE;
ALTER TABLE tb_answers ADD CONSTRAINT fk_answer_question_ref 
  FOREIGN KEY (question_id) REFERENCES tb_questions(question_id) ON DELETE CASCADE;
ALTER TABLE tb_test_sections ADD CONSTRAINT fk_section_test 
  FOREIGN KEY (test_id) REFERENCES tb_tests(test_id) ON DELETE CASCADE;
ALTER TABLE tb_ai_roadmap_steps ADD CONSTRAINT fk_step_roadmap 
  FOREIGN KEY (roadmap_id) REFERENCES tb_ai_roadmaps(roadmap_id) ON DELETE CASCADE;
ALTER TABLE tb_ai_roadmap_steps ADD CONSTRAINT fk_step_lesson 
  FOREIGN KEY (lesson_id) REFERENCES tb_lessons(lesson_id) ON DELETE SET NULL;
```

### 2.2 MongoDB Collections Design

```javascript
// Collection: user_behavior_logs
{
  userId: ObjectId,
  sessionId: String,
  action: String, // "page_view", "lesson_start", "lesson_complete", "test_submit"
  metadata: {
    pageUrl: String,
    lessonId: Number,
    skillType: String,
    timeSpentSeconds: Number,
    device: String,
    browser: String
  },
  timestamp: ISODate,
  // TTL: 90 days
}

// Collection: learning_patterns
{
  userId: ObjectId,
  analyzedAt: ISODate,
  patterns: {
    preferredStudyTime: String, // "morning", "afternoon", "evening"
    averageSessionDuration: Number,
    strongestSkill: String,
    weakestSkill: String,
    learningPace: String, // "fast", "moderate", "slow"
    consistencyScore: Number // 0-100
  },
  weeklyActivity: [{ day: String, minutes: Number }],
  skillProgression: [{ skill: String, scores: [Number], dates: [ISODate] }]
}

// Collection: ai_predictions
{
  userId: ObjectId,
  predictionType: String, // "band_score", "skill_improvement", "exam_readiness"
  input: Object,
  output: Object,
  model: String,
  confidence: Number,
  createdAt: ISODate,
  // TTL: 30 days
}

// Collection: speaking_sessions_detail
{
  userId: ObjectId,
  mysqlSessionId: Number,
  audioBlob: BinData, // or S3 URL
  transcript: String,
  aiAnalysis: {
    fluencyDetails: Object,
    pronunciationDetails: Object,
    grammarDetails: [{error: String, correction: String, position: Number}],
    vocabularyUsage: [String],
    fillerWords: [{word: String, count: Number}]
  },
  duration: Number,
  createdAt: ISODate
}

// Collection: ai_conversations
{
  userId: ObjectId,
  conversationId: String,
  messages: [{
    role: String, // "user", "assistant", "system"
    content: String,
    timestamp: ISODate,
    sources: [{ id: String, score: Number }] // RAG sources
  }],
  topic: String,
  createdAt: ISODate,
  updatedAt: ISODate
}

// Collection: writing_analyses_detail
{
  userId: ObjectId,
  mysqlSubmissionId: Number,
  detailedAnalysis: {
    paragraphBreakdown: [{
      paragraph: Number,
      coherenceScore: Number,
      topicSentence: Boolean,
      linkingWords: [String]
    }],
    grammarErrors: [{
      sentence: String,
      error: String,
      correction: String,
      errorType: String,
      severity: String
    }],
    vocabularyAnalysis: {
      uniqueWords: Number,
      academicWords: [String],
      bandSpecificWords: [String],
      suggestedAlternatives: [{original: String, alternatives: [String]}]
    },
    structureAnalysis: {
      hasIntro: Boolean,
      hasConclusion: Boolean,
      bodyParagraphs: Number,
      thesisStatement: String
    }
  },
  modelUsed: String,
  processingTime: Number,
  createdAt: ISODate
}
```

### 2.3 Pinecone Vector Database Design

```python
# Index: ielts-knowledge
# Dimension: 768 (text-embedding-3-small) hoặc 1536 (text-embedding-3-large)
# Metric: cosine

# Namespace: lessons
{
    "id": "lesson_102",
    "values": [0.1, 0.2, ...],  # embedding vector
    "metadata": {
        "skill": "reading",
        "difficulty": 6,
        "lessonId": 102,
        "title": "Climate Change and Urban Planning",
        "topicCategory": "academic",
        "bandTarget": 7.0,
        "contentPreview": "First 200 chars of content..."
    }
}

# Namespace: vocabulary
{
    "id": "vocab_mitigate",
    "values": [0.1, 0.2, ...],
    "metadata": {
        "word": "mitigate",
        "meaning": "to reduce the severity",
        "bandLevel": 7.0,
        "category": "academic",
        "partOfSpeech": "verb",
        "ieltsTopics": ["environment", "health"]
    }
}

# Namespace: reading_passages
{
    "id": "passage_1",
    "values": [0.1, 0.2, ...],
    "metadata": {
        "title": "Climate Change and Urban Planning",
        "difficulty": "band_7_8",
        "topicCategory": "academic",
        "wordCount": 512
    }
}

# Namespace: writing_samples
{
    "id": "writing_task2_education",
    "values": [0.1, 0.2, ...],
    "metadata": {
        "taskType": 2,
        "topic": "education",
        "bandLevel": 7.5,
        "sampleType": "model_answer"
    }
}

# Namespace: speaking_topics
{
    "id": "speaking_part2_book",
    "values": [0.1, 0.2, ...],
    "metadata": {
        "part": 2,
        "topic": "Describe a book you recently read",
        "category": "personal_experience",
        "difficulty": 6.0
    }
}

# Namespace: knowledge_base
{
    "id": "kb_grammar_conditionals",
    "values": [0.1, 0.2, ...],
    "metadata": {
        "type": "grammar",
        "topic": "conditionals",
        "bandRelevance": "6.0-7.5"
    }
}
```

---

## 🔵 PHẦN 3: KẾ HOẠCH TRIỂN KHAI CHI TIẾT

### Phase 1: Database & Security Foundation (Ưu tiên cao nhất)

> [!CAUTION]
> **BẢO MẬT:** API keys và secrets đang lộ trong source code. Phải fix NGAY trước khi deploy.

#### [MODIFY] [appsettings.json](file:///d:/University/STKN/WebIeltsFree/WebIeltsFree/appsettings.json)
- Xóa tất cả hardcoded secrets (Gemini API key, JWT key)
- Thay bằng environment variable references
- Thêm MongoDB và Pinecone connection config
- Thêm Email/SMTP config section

#### [NEW] appsettings.Production.json
- Production-specific overrides
- Tất cả secrets từ environment variables

#### [MODIFY] [.env.template](file:///d:/University/STKN/WebIeltsFree/.env.template)
- Thêm MongoDB connection string
- Thêm Pinecone API key + index URL
- Thêm SMTP settings
- Thêm Redis connection string

#### [NEW] database/ieltsdb_v2_migration.sql
- Migration script toàn bộ schema changes ở Phần 2.1
- Thêm tất cả bảng mới
- Thêm tất cả foreign keys
- Thêm tất cả indexes
- Seed data thật cho courses, modules, lessons, vocabulary, speaking topics, writing prompts

#### [NEW] database/ieltsdb_v2_seed_data.sql
- 6+ courses (Listening, Reading, Writing, Speaking, Vocabulary, Grammar)
- 24+ modules
- 100+ lessons
- 500+ vocabulary words với phonetic, synonyms, etc.
- 50+ speaking topics (Part 1, 2, 3)
- 30+ writing prompts (Task 1 + Task 2)
- 10+ reading passages với questions
- 10+ listening materials
- 20+ placement test questions

---

### Phase 2: Multi-Database Backend Integration

#### [MODIFY] [WebIeltsFree.csproj](file:///d:/University/STKN/WebIeltsFree/WebIeltsFree/WebIeltsFree.csproj)
- Thêm NuGet packages:
  - `MongoDB.Driver` (2.28+)
  - `Pinecone.NET` hoặc HTTP client
  - `StackExchange.Redis`
  - `MailKit` (for email)
  - `Serilog.AspNetCore` (structured logging)

#### [MODIFY] [Program.cs](file:///d:/University/STKN/WebIeltsFree/WebIeltsFree/Program.cs)
- Register MongoDB client as singleton
- Register Redis connection
- Register Pinecone client
- Register Email service
- Add Serilog logging
- Configure proper CORS policy for production
- Add SignalR

#### [NEW] Models/MongoDbContext.cs
- MongoDB connection management
- Collection accessors cho 6 collections
- TTL index setup

#### [NEW] Services/MongoDbService.cs
- CRUD operations cho behavior logs
- Learning patterns analysis storage
- AI predictions logging
- Speaking session detail storage
- Writing analysis detail storage
- AI conversation history

#### [NEW] Services/PineconeService.cs
- Real vector search implementation (thay mock)
- Upsert embeddings
- Query by namespace
- Metadata filtering
- Batch operations

#### [NEW] Services/RedisService.cs
- Replace in-memory CacheService
- Distributed caching
- Rate limiting counters
- Session management

#### [NEW] Services/EmailService.cs
- SMTP integration via MailKit
- Password reset emails
- Welcome emails
- Study reminder notifications

#### [MODIFY] [Middleware.cs](file:///d:/University/STKN/WebIeltsFree/WebIeltsFree/Models/Middleware.cs)
- `SecurityValidator.GenerateSecureToken` → dùng `RandomNumberGenerator`
- Rate limiting dùng Redis thay Dictionary
- Thêm request correlation ID
- Structured logging

#### [MODIFY] [GeminiService.cs](file:///d:/University/STKN/WebIeltsFree/WebIeltsFree/Models/GeminiService.cs)
- API key từ environment variable
- Log AI requests/responses to MongoDB
- Retry logic với exponential backoff
- Token usage tracking

#### [MODIFY] [AiServices.cs](file:///d:/University/STKN/WebIeltsFree/WebIeltsFree/Models/AiServices.cs)
- `CacheService` → delegate to `RedisService`
- `VectorSearchService` → delegate to `PineconeService`
- `SpacedRepetitionService` → persist to MySQL

---

### Phase 3: Entity & API Enhancement

#### [MODIFY] [Entities.cs](file:///d:/University/STKN/WebIeltsFree/WebIeltsFree/Models/Entities.cs)
- Thêm entities cho bảng mới
- Thêm `UpdatedAt`, `IsDeleted`, `DeletedAt` cho entities cần thiết
- Thêm `UserSession` entity (refresh token management)
- Thêm `WritingPrompt`, `SpeakingTopic` entities
- Thêm `Notification` entity
- Thêm `UserPlacementResult` entity
- Thêm `Gamification`, `UserAchievement` entities
- Fix `Vocabulary` entity (remove duplicate fields)

#### [MODIFY] [AppDbContext.cs](file:///d:/University/STKN/WebIeltsFree/WebIeltsFree/Models/AppDbContext.cs)
- Add DbSets cho tất cả entities mới
- Add Fluent API configs for new relationships
- Add global query filter for soft delete
- Add `SaveChangesAsync` override for auto `UpdatedAt`

#### [MODIFY] [DTOs.cs](file:///d:/University/STKN/WebIeltsFree/WebIeltsFree/Models/DTOs.cs)
- Thêm DTOs cho entities mới
- Thêm `VocabularyDetailDto` (with phonetic, synonyms)
- Thêm `SpeakingTopicDetailDto` (with parts)
- Thêm `WritingPromptDetailDto`
- Thêm `NotificationDto`
- Thêm `GamificationDto`, `AchievementDto`
- Thêm `PlacementResultDto`

#### [MODIFY] Controllers — Enhancement cho tất cả controllers:

**AuthController** — Thêm:
- Email verification flow
- Proper refresh token storage (tb_user_sessions)
- Login attempt tracking + account lockout
- OAuth integration ready (Google, Facebook)

**UsersController** — Thêm:
- Onboarding completion tracking
- Delete account (GDPR compliance)
- Export user data

**TestsController** — Thêm:
- Placement test with proper question bank
- Mock test (full IELTS simulation)
- Test analytics per skill

**SpeakingController** — Thêm:
- Topic categories endpoint
- Part 1/2/3 topic management
- WebRTC signaling (for real speaking)
- Save detailed analysis to MongoDB

**WritingController** — Thêm:
- Writing prompts bank
- Task 1 + Task 2 specific prompts
- Revision history
- AI detailed feedback saved to MongoDB

**ListeningController** — Thêm:
- Audio streaming support
- Section-based practice (IELTS has 4 sections)
- Difficulty filtering

**ReadingController** — Thêm:
- Passage set management
- Timer endpoints
- Detailed answer review

#### [NEW] Controllers/GamificationController.cs
- XP system
- Streak tracking
- Achievement badges
- Leaderboard

#### [NEW] Controllers/NotificationController.cs
- Push notification management
- Study reminders
- Achievement notifications

#### [NEW] Controllers/AdminController.cs
- User management
- Content management (CRUD courses/lessons)
- Analytics dashboard data
- System health monitoring

#### [NEW] Hubs/
- `SpeakingHub.cs` — Real-time speaking practice
- `NotificationHub.cs` — Real-time notifications
- `AiTutorHub.cs` — Stream AI responses

---

### Phase 4: Dữ Liệu Thật (Content Seeding)

> [!IMPORTANT]
> Website cần dữ liệu thật, chất lượng cao để có tính học thuật. Tất cả nội dung phải tiệm cận IELTS chính thức.

#### Courses (6 courses):
1. **IELTS Listening Mastery** (Band 5.0-7.0) — 4 modules, 16 lessons
2. **IELTS Reading Academic** (Band 5.0-7.0) — 4 modules, 16 lessons
3. **IELTS Writing Task 1 & 2** (Band 5.0-7.0) — 4 modules, 12 lessons
4. **IELTS Speaking Confidence** (Band 5.0-7.0) — 4 modules, 12 lessons
5. **Academic Vocabulary Builder** (Band 5.0-8.0) — 6 modules, 24 lessons
6. **Grammar for IELTS** (Band 5.0-7.0) — 4 modules, 16 lessons

#### Vocabulary (500+ words):
- AWL (Academic Word List) — 570 words
- Topic-based: Environment, Education, Technology, Health, Society, Cities
- Band level tagged (5.0 → 8.0+)
- Vietnamese meaning, phonetic, example, synonyms

#### Reading Passages (20+):
- Academic topics following Cambridge IELTS style
- 700-900 words each
- True/False/Not Given, Multiple Choice, Matching Headings, Sentence Completion
- 13 questions per passage (giống IELTS thật)

#### Listening Materials (20+):
- Section 1: Social/everyday conversation
- Section 2: Monologue on everyday topic
- Section 3: Academic discussion
- Section 4: Academic lecture
- 10 questions per section

#### Writing Prompts (30+):
- Task 1: Bar charts, line graphs, pie charts, tables, maps, processes
- Task 2: Opinion, Discussion, Problem/Solution, Two-part, Advantages/Disadvantages

#### Speaking Topics (50+):
- Part 1: 20 common topics (Hometown, Work, Study, Food, Weather...)
- Part 2: 20 cue cards with sub-questions
- Part 3: Discussion questions linked to Part 2

---

### Phase 5: Docker & Deployment

#### [MODIFY] [docker-compose.yml](file:///d:/University/STKN/WebIeltsFree/docker-compose.yml)
- Thêm MongoDB container
- Thêm Redis container
- Network configuration
- Volume persistence
- Health checks

#### [MODIFY] [docker-compose.prod.yml](file:///d:/University/STKN/WebIeltsFree/docker-compose.prod.yml)
- Production optimized
- External database connections (cloud)
- SSL termination

#### [MODIFY] [Dockerfile](file:///d:/University/STKN/WebIeltsFree/Dockerfile)
- Multi-stage build optimization
- Security hardening
- Non-root user (already done ✅)

#### Deployment Architecture:

```
┌─────────────────────────────────────────────────────┐
│                    PUBLIC INTERNET                     │
│         (Users access via browser/mobile)              │
└─────────────┬──────────────────────┬──────────────────┘
              │                      │
    ┌─────────▼─────────┐  ┌────────▼────────┐
    │   Vercel (FREE)   │  │  Render (FREE)  │
    │   Next.js Frontend│  │  ASP.NET Backend│
    │   - SSR/SSG       │  │  - REST API     │
    │   - Edge Caching  │  │  - SignalR      │
    │   - CDN           │  │  - Background   │
    └───────────────────┘  └────────┬────────┘
                                    │
              ┌─────────────────────┼─────────────────┐
              │                     │                  │
    ┌─────────▼─────┐  ┌──────────▼──────┐  ┌───────▼───────┐
    │  Aiven MySQL  │  │  MongoDB Atlas  │  │   Pinecone    │
    │   (FREE 5GB)  │  │  (FREE 512MB)   │  │ (FREE 100K)   │
    │   - Core data │  │  - AI logs      │  │ - Embeddings  │
    │   - Users     │  │  - Behaviors    │  │ - RAG search  │
    │   - Progress  │  │  - Conversations│  │ - Recommend   │
    └───────────────┘  └─────────────────┘  └───────────────┘
```

---

## 🟡 PHẦN 4: SECURITY CHECKLIST

| # | Action | Status |
|---|--------|--------|
| 1 | Move ALL secrets to environment variables | ⬜ TODO |
| 2 | Add `.gitignore` cho `appsettings.json` secrets | ⬜ TODO |
| 3 | Implement proper refresh token storage (DB) | ⬜ TODO |
| 4 | Replace `Random` with `RandomNumberGenerator` | ⬜ TODO |
| 5 | Production CORS policy (specific origins only) | ⬜ TODO |
| 6 | HTTPS enforcement | ✅ Done |
| 7 | Security headers (CSP, X-Frame, etc.) | ✅ Done |
| 8 | Rate limiting | ✅ Done (needs Redis upgrade) |
| 9 | Input sanitization | ✅ Done |
| 10 | Password validation | ✅ Done |
| 11 | BCrypt password hashing | ✅ Done |
| 12 | SQL injection prevention (EF Core parameterized) | ✅ Done |
| 13 | XSS prevention | ✅ Done |
| 14 | Email enumeration prevention (forgot-password) | ✅ Done |
| 15 | Account lockout after failed attempts | ⬜ TODO |
| 16 | Audit logging | ⬜ TODO |
| 17 | GDPR compliance (data export/delete) | ⬜ TODO |
| 18 | API versioning | ⬜ TODO |

---

## 🟣 PHẦN 5: PERFORMANCE OPTIMIZATION

| Optimization | Implementation |
|-------------|----------------|
| **API Response Caching** | Redis with configurable TTL per endpoint |
| **Database Indexing** | Composite indexes on frequent query patterns |
| **Lazy Loading** | EF Core lazy loading for navigation properties |
| **Pagination** | Already implemented ✅ — ensure all list endpoints use it |
| **Connection Pooling** | MySQL: `MaxPoolSize=100;MinPoolSize=10` |
| **Compression** | Enable Brotli/Gzip response compression |
| **CDN** | Static assets via Vercel Edge Network |
| **Database Read Replicas** | Future: MySQL read replica for analytics queries |
| **Vector Search Optimization** | Pinecone metadata filters to narrow search scope |
| **Background Jobs** | AI analysis tasks run async (avoid blocking API) |

---

## User Review Required

> [!IMPORTANT]
> **Quyết định cần từ bạn:**
> 1. **Frontend**: Bạn muốn tạo Next.js frontend riêng (decoupled) hay tiếp tục dùng MVC Razor Views? → Tôi khuyến nghị **Next.js riêng** theo design ban đầu.
> 2. **MongoDB Atlas**: Bạn đã có tài khoản MongoDB Atlas chưa? Cần connection string.
> 3. **Pinecone**: Bạn đã có Pinecone account + API key chưa?
> 4. **Email Service**: Bạn muốn dùng SMTP nào? (Gmail SMTP miễn phí / SendGrid free tier / Resend free tier)
> 5. **Bắt đầu từ Phase nào?** Tôi khuyến nghị Phase 1 (Database + Security) → Phase 4 (Seed Data) → Phase 2 (Multi-DB) → Phase 3 (API Enhancement) → Phase 5 (Deploy).
> 6. **Gemini Model**: Hiện dùng `gemini-2.5-flash`. Bạn muốn giữ hay đổi?

## Open Questions

> [!WARNING]
> **Về scope:** Toàn bộ kế hoạch này rất lớn (~50+ files cần tạo/sửa). Tôi đề xuất chia thành **từng phase** và implement tuần tự. Mỗi phase sẽ mất khoảng 1-2 sessions. Bạn muốn bắt đầu với phase nào?

## Verification Plan

### Automated Tests
- Build project: `dotnet build`
- Database migration test: import SQL vào MySQL local
- API endpoint testing via Swagger UI
- Health check: `GET /health`

### Manual Verification  
- Test full user journey: Register → Placement Test → Roadmap → Learn → Practice → Review
- Verify all CRUD operations
- Test AI integration (Gemini API)
- Test MongoDB write/read
- Test Pinecone search
- Load test with multiple concurrent users
