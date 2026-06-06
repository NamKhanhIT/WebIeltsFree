# WebIeltsFree — Complete Role Analysis & Business Flows
## Teacher Expansion + All Three Roles

> Append this entire document to `WebIeltsFree_Dashboard_Build_Prompt.md` before any new coding session.
> Based on full codebase scan of the uploaded project + confirmed design decisions.

---

## PART A — DECISIONS LOCKED IN

| Question | Decision |
|---|---|
| Proactive submission review | ❌ Skip — disputes are the only entry point |
| Custom test creation | ✅ Full creation + assign to specific students |
| Test deadline | ✅ Both — teacher chooses deadline or self-paced per assignment |
| Roadmap suggestions | ✅ Teacher suggests → **blocking pending action** on student dashboard |
| Student approval flow | ✅ Student must resolve before studying (not optional, not timed) |
| Announcements | ✅ Platform-wide — visible to ALL students |
| Teacher public profile | ✅ Name, bio, specialties — students can browse |
| Test skill types | ✅ Any combination of all 4 skills per test |

---

## PART B — COMPLETE TEACHER FUNCTION MAP

### B1 — Functions Already Built (v2.2.0)

| # | Function | API Route | Area Route |
|---|---|---|---|
| 1 | View dashboard stats | `GET /api/teacher/dashboard` | `Teacher/Dashboard` |
| 2 | List all students | `GET /api/teacher/students` | `Teacher/Students/Index` |
| 3 | View student academic detail | `GET /api/teacher/students/{id}/detail` | `Teacher/Students/Details` |
| 4 | Claim grade dispute | `PATCH /api/disputes/{id}/claim` | `Teacher/GradeDisputes/Edit → Claim` |
| 5 | Resolve grade dispute + score override | `PATCH /api/disputes/{id}/resolve` | `Teacher/GradeDisputes/Edit → Resolve` |
| 6 | Reject grade dispute | `PATCH /api/disputes/{id}/reject` | `Teacher/GradeDisputes/Edit → Reject` |
| 7 | Message student (open pool) | `POST /api/messages/conversations/{id}/send` | `Teacher/Conversations/Edit` |
| 8 | Close conversation | `PATCH /api/messages/conversations/{id}/close` | `Teacher/Conversations/Edit` |
| 9 | Create writing prompt | `POST /api/writing/prompts` | `Teacher/WritingPrompts/Create` |
| 10 | Edit writing prompt | `PUT /api/writing/prompts/{id}` | `Teacher/WritingPrompts/Edit` |
| 11 | Soft-delete writing prompt | `DELETE /api/writing/prompts/{id}` | `Teacher/WritingPrompts/Delete` |
| 12 | Create speaking topic | `POST /api/speaking/topics` | `Teacher/SpeakingTopics/Create` |
| 13 | Edit speaking topic | `PUT /api/speaking/topics/{id}` | `Teacher/SpeakingTopics/Edit` |
| 14 | Soft-delete speaking topic | `DELETE /api/speaking/topics/{id}` | `Teacher/SpeakingTopics/Delete` |
| 15 | Create reading passage + questions | `POST /api/reading/passages` | `Teacher/ReadingPassages/Create` |
| 16 | Edit reading passage + questions | `PUT /api/reading/passages/{id}` | `Teacher/ReadingPassages/Edit` |
| 17 | Soft-delete reading passage | `DELETE /api/reading/passages/{id}` | `Teacher/ReadingPassages/Delete` |
| 18 | Create listening material + questions | `POST /api/listening/materials` | `Teacher/ListeningMaterials/Create` |
| 19 | Edit listening material + questions | `PUT /api/listening/materials/{id}` | `Teacher/ListeningMaterials/Edit` |
| 20 | Soft-delete listening material | `DELETE /api/listening/materials/{id}` | `Teacher/ListeningMaterials/Delete` |

### B2 — NEW Functions (from this session's decisions)

| # | Function | New Area Route | New DB Table |
|---|---|---|---|
| 21 | Create custom test (any skill mix) | `Teacher/TestAssignments/CreateTest` | `tb_tests` + `is_teacher_created` flag |
| 22 | Assign test to specific student | `Teacher/TestAssignments/Assign` | `tb_teacher_assignments` |
| 23 | Set/remove deadline on assignment | `Teacher/TestAssignments/Edit` | `tb_teacher_assignments.deadline` |
| 24 | View all assignments + status | `Teacher/TestAssignments/Index` | `tb_teacher_assignments` |
| 25 | Revoke an assignment | `Teacher/TestAssignments/Delete` | soft-delete `tb_teacher_assignments` |
| 26 | Suggest roadmap changes for a student | `Teacher/RoadmapSuggestions/Create` | `tb_roadmap_suggestions` |
| 27 | View own pending suggestions | `Teacher/RoadmapSuggestions/Index` | `tb_roadmap_suggestions` |
| 28 | Post platform-wide announcement | `Teacher/Announcements/Create` | `tb_announcements` |
| 29 | Edit announcement | `Teacher/Announcements/Edit` | `tb_announcements` |
| 30 | Deactivate announcement | `Teacher/Announcements/Delete` | soft-delete `tb_announcements.is_active` |
| 31 | Manage own public profile | `Teacher/TeacherProfile/Edit` | `tb_teacher_profiles` |
| 32 | Approve another teacher's content | `Teacher/WritingPrompts/Approve` | `tb_writing_prompts.status` |
| 33 | Reject another teacher's content | `Teacher/WritingPrompts/Reject` | `tb_writing_prompts.status` |

---

## PART C — NEW DATABASE TABLES (4 tables — total becomes 45)

Add all SQL to `DatabaseScripts/ieltsdb.sql`. Add all entities to `Models/Entities.cs`. Add all DbSets to `Models/AppDbContext.cs`.

### C1 — Alter existing tb_tests

```sql
-- Add teacher-created flags to existing tb_tests
ALTER TABLE tb_tests
  ADD COLUMN created_by INT NULL COMMENT 'FK to tb_users (teacher)',
  ADD COLUMN is_teacher_created TINYINT(1) NOT NULL DEFAULT 0,
  ADD COLUMN is_public TINYINT(1) NOT NULL DEFAULT 1;
  -- is_public = 0 means only assigned students can see it
```

### C2 — tb_teacher_assignments

```sql
CREATE TABLE tb_teacher_assignments (
  assignment_id  INT AUTO_INCREMENT PRIMARY KEY,
  test_id        INT NOT NULL,
  teacher_id     INT NOT NULL,
  student_id     INT NOT NULL,
  title          VARCHAR(255) NOT NULL,
  instructions   TEXT NULL,
  deadline       DATETIME NULL,       -- NULL = self-paced
  status         ENUM('pending','completed','overdue') NOT NULL DEFAULT 'pending',
  assigned_at    DATETIME DEFAULT CURRENT_TIMESTAMP,
  completed_at   DATETIME NULL,
  attempt_id     INT NULL,            -- FK tb_user_test_attempts (filled on completion)
  is_deleted     TINYINT(1) NOT NULL DEFAULT 0,
  CONSTRAINT fk_asgn_test    FOREIGN KEY (test_id)    REFERENCES tb_tests(test_id),
  CONSTRAINT fk_asgn_teacher FOREIGN KEY (teacher_id) REFERENCES tb_users(user_id),
  CONSTRAINT fk_asgn_student FOREIGN KEY (student_id) REFERENCES tb_users(user_id)
);
```

**Entity (add to Entities.cs):**
```csharp
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
    // status: "pending" | "completed" | "overdue"
    [Column("assigned_at")]         public DateTime AssignedAt { get; set; }
    [Column("completed_at")]        public DateTime? CompletedAt { get; set; }
    [Column("attempt_id")]          public int? AttemptId { get; set; }
    [Column("is_deleted")]          public bool IsDeleted { get; set; }
    public User? Teacher { get; set; }
    public User? Student { get; set; }
    public Test? Test { get; set; }
}
```

### C3 — tb_roadmap_suggestions

```sql
CREATE TABLE tb_roadmap_suggestions (
  suggestion_id     INT AUTO_INCREMENT PRIMARY KEY,
  teacher_id        INT NOT NULL,
  student_id        INT NOT NULL,
  roadmap_id        INT NOT NULL,
  suggestion_title  VARCHAR(255) NOT NULL,
  message           TEXT NOT NULL,       -- teacher's explanation shown to student
  suggested_changes TEXT NOT NULL,       -- JSON: [{action, week_number, description}]
  status            ENUM('pending','accepted','rejected') NOT NULL DEFAULT 'pending',
  created_at        DATETIME DEFAULT CURRENT_TIMESTAMP,
  resolved_at       DATETIME NULL,
  CONSTRAINT fk_rsugg_teacher  FOREIGN KEY (teacher_id) REFERENCES tb_users(user_id),
  CONSTRAINT fk_rsugg_student  FOREIGN KEY (student_id) REFERENCES tb_users(user_id),
  CONSTRAINT fk_rsugg_roadmap  FOREIGN KEY (roadmap_id) REFERENCES tb_ai_roadmaps(roadmap_id)
);
```

**Entity:**
```csharp
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
    // status: "pending" | "accepted" | "rejected"
    [Column("created_at")]            public DateTime CreatedAt { get; set; }
    [Column("resolved_at")]           public DateTime? ResolvedAt { get; set; }
    public User? Teacher { get; set; }
    public User? Student { get; set; }
    public AIRoadmap? Roadmap { get; set; }
}
```

### C4 — tb_announcements

```sql
CREATE TABLE tb_announcements (
  announcement_id INT AUTO_INCREMENT PRIMARY KEY,
  teacher_id      INT NOT NULL,
  title           VARCHAR(255) NOT NULL,
  content         TEXT NOT NULL,
  is_active       TINYINT(1) NOT NULL DEFAULT 1,
  created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
  expires_at      DATETIME NULL,         -- NULL = never expires
  CONSTRAINT fk_ann_teacher FOREIGN KEY (teacher_id) REFERENCES tb_users(user_id)
);
```

**Entity:**
```csharp
[Table("tb_announcements")]
public class Announcement
{
    [Key][Column("announcement_id")] public int AnnouncementId { get; set; }
    [Column("teacher_id")]           public int TeacherId { get; set; }
    [Column("title")]                public string Title { get; set; } = "";
    [Column("content")]              public string Content { get; set; } = "";
    [Column("is_active")]            public bool IsActive { get; set; } = true;
    [Column("created_at")]           public DateTime CreatedAt { get; set; }
    [Column("expires_at")]           public DateTime? ExpiresAt { get; set; }
    public User? Teacher { get; set; }
}
```

### C5 — tb_teacher_profiles

```sql
CREATE TABLE tb_teacher_profiles (
  teacher_id        INT PRIMARY KEY,
  bio               TEXT NULL,
  specialties       VARCHAR(500) NULL,   -- comma-separated: "Writing,Speaking,Grammar"
  years_experience  INT NULL,
  is_public         TINYINT(1) NOT NULL DEFAULT 1,
  created_at        DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at        DATETIME NULL,
  CONSTRAINT fk_tprofile_user FOREIGN KEY (teacher_id) REFERENCES tb_users(user_id)
);
```

**Entity:**
```csharp
[Table("tb_teacher_profiles")]
public class TeacherProfile
{
    [Key][Column("teacher_id")]       public int TeacherId { get; set; }
    [Column("bio")]                   public string? Bio { get; set; }
    [Column("specialties")]           public string? Specialties { get; set; }
    [Column("years_experience")]      public int? YearsExperience { get; set; }
    [Column("is_public")]             public bool IsPublic { get; set; } = true;
    [Column("created_at")]            public DateTime CreatedAt { get; set; }
    [Column("updated_at")]            public DateTime? UpdatedAt { get; set; }
    public User? User { get; set; }
}
```

### New DbSets (add to AppDbContext.cs)

```csharp
public DbSet<TeacherAssignment>  TeacherAssignments  { get; set; }
public DbSet<RoadmapSuggestion>  RoadmapSuggestions  { get; set; }
public DbSet<Announcement>       Announcements       { get; set; }
public DbSet<TeacherProfile>     TeacherProfiles     { get; set; }
```

---

## PART D — REAL AuditHelper SIGNATURE (from codebase scan)

```csharp
// ACTUAL SIGNATURE — use this exactly:
await AuditHelper.LogAsync(
    context:    _context,
    userId:     GetCurrentUserId(),           // int?
    action:     "create_writing_prompt",       // snake_case verb_noun
    entityType: "WritingPrompt",               // PascalCase class name
    entityId:   entity.PromptId,              // int?
    oldValues:  null,                          // JSON string of old state (null for creates)
    newValues:  System.Text.Json.JsonSerializer.Serialize(entity), // JSON of new state
    ipAddress:  HttpContext.Connection.RemoteIpAddress?.ToString(),
    userAgent:  HttpContext.Request.Headers.UserAgent.ToString()
);

// Action name convention (snake_case):
// "create_writing_prompt" | "edit_writing_prompt" | "soft_delete_writing_prompt"
// "create_speaking_topic" | "edit_speaking_topic" | "soft_delete_speaking_topic"
// "claim_dispute" | "resolve_dispute" | "reject_dispute"
// "reply_conversation" | "close_conversation"
// "create_assignment" | "revoke_assignment"
// "create_announcement" | "edit_announcement" | "deactivate_announcement"
// "suggest_roadmap_change"
// "approve_content" | "reject_content"
// "promote_user" | "ban_user" | "hard_delete_user"
```

---

## PART E — DUPLICATE FUNCTION MAP (codebase scan results)

The following action names exist in BOTH root `Controllers/` (API) and will exist in `Areas/` (MVC Dashboard). This is NOT a bug — areas use `[Area]` attribute for namespacing — but every area controller must be verified to have explicit `[Area("Admin")]` or `[Area("Teacher")]` attribute to prevent any routing ambiguity.

| Root Controller | Root Action | Area Controller | Area Action | Risk |
|---|---|---|---|---|
| `Controllers/UsersController.cs` | `GetProfile`, `UpdateProfile` | `Areas/Admin/Controllers/UsersController.cs` | `Index`, `Edit` | ⚠️ Same class name — MUST have `[Area("Admin")]` |
| `Controllers/DisputesController.cs` | `GetDisputes`, `GetDispute`, `ClaimDispute`, `ResolveDispute`, `RejectDispute` | `Areas/Teacher/Controllers/GradeDisputesController.cs` | `Index`, `Details`, `Edit` | ✅ Different class name — no conflict |
| `Controllers/MessagesController.cs` | `GetConversations`, `SendMessage` | `Areas/Teacher/Controllers/ConversationsController.cs` | `Index`, `Details`, `Edit` | ✅ Different class name — no conflict |
| `Controllers/WritingController.cs` | `CreatePrompt`, `UpdatePrompt`, `DeletePrompt` | `Areas/Teacher/Controllers/WritingPromptsController.cs` | `Create`, `Edit`, `Delete` | ✅ Different class name — no conflict |
| `Controllers/SpeakingController.cs` | `CreateTopic`, `UpdateTopic`, `DeleteTopic` | `Areas/Teacher/Controllers/SpeakingTopicsController.cs` | `Create`, `Edit`, `Delete` | ✅ Different class name — no conflict |

**Action required:** Verify `Areas/Admin/Controllers/UsersController.cs` has `[Area("Admin")]` — it shares the class name `UsersController` with root `Controllers/UsersController.cs`. The `[Area]` attribute is the ONLY thing preventing routing collision.

---

## PART F — TEMPLATE SPEC (InApp by CodesCandy)

**Template location:** `~/WebIeltsFree/Template_IELTS/Admin_Teacher/inapp-1.0.0/inapp-1.0.0/src/`
**CSS Framework:** Bootstrap 5
**Icon library:** Tabler Icons (`ti ti-*` classes)
**JS files:** `main.js` (module), `sidebar.js`, `chart.js`, `custom.js`

### Layout Shell (replicate in `_Layout.cshtml`)

```html
<div id="overlay" class="overlay"></div>

<!-- TOPBAR -->
<nav id="topbar" class="navbar bg-white border-bottom fixed-top topbar px-3">
  <button id="toggleBtn" class="d-none d-lg-inline-flex btn btn-light btn-icon btn-sm">
    <i class="ti ti-layout-sidebar-left-expand"></i>
  </button>
  <button id="mobileBtn" class="btn btn-light btn-icon btn-sm d-lg-none me-2">
    <i class="ti ti-layout-sidebar-left-expand"></i>
  </button>
  <!-- Bell notification dropdown + avatar dropdown -->
</nav>

<!-- SIDEBAR -->
<aside id="sidebar" class="sidebar">
  <div class="logo-area">
    <a href="/" class="d-inline-flex">
      <img src="/img/logo-icon.svg" width="24">
      <span class="logo-text ms-2"><img src="/img/logo.svg"></span>
    </a>
  </div>
  <ul class="nav flex-column">
    <li class="px-4 py-2"><small class="nav-text">SECTION LABEL</small></li>
    <li><a class="nav-link active" href="/area/controller"><i class="ti ti-home"></i><span class="nav-text">Page</span></a></li>
  </ul>
</aside>

<!-- MAIN CONTENT -->
<main id="content" class="content py-10">
  <div class="container-fluid">
    @RenderBody()
  </div>
</main>
```

### Key CSS Classes to Use Exactly

| Component | Classes |
|---|---|
| Stat card (primary) | `card p-4 bg-primary bg-opacity-10 border border-primary border-opacity-25 rounded-2` |
| Stat card (success) | `card p-4 bg-success bg-opacity-10 border border-success border-opacity-25 rounded-2` |
| Stat card (info) | `card p-4 bg-info bg-opacity-10 border border-info border-opacity-25 rounded-2` |
| Stat card (warning) | `card p-4 bg-warning bg-opacity-10 border border-warning border-opacity-25 rounded-2` |
| Icon badge | `icon-shape icon-md bg-{color} text-white rounded-2` |
| Card header | `card-header d-flex justify-content-between align-items-center bg-transparent px-4 py-3` |
| Avatar | `avatar avatar-sm rounded-circle` |
| Status badge (success) | `badge bg-success-subtle text-success` |
| Status badge (warning) | `badge bg-warning-subtle text-warning` |
| Status badge (danger) | `badge bg-danger-subtle text-danger` |
| Status badge (primary) | `badge bg-primary-subtle text-primary` |
| Status badge (info) | `badge bg-info-subtle text-info` |
| Sidebar nav label | `px-4 py-2` + `<small class="nav-text">LABEL</small>` |
| Sidebar nav link | `nav-link` (active: add `active` class) |
| List group items | `list-group list-group-flush` > `list-group-item d-flex align-items-center gap-3` |
| Table | Standard Bootstrap `table` inside `card` |
| Form select sm | `form-select form-select-sm` |
| Section heading | `<h1 class="fs-3 mb-1">Title</h1>` |
| Page header block | `<div class="mb-6"><h1 class="fs-3 mb-1">...</h1><p>...</p></div>` |

### Tabler Icons Reference (use these for area nav items)

| Page | Icon |
|---|---|
| Dashboard | `ti ti-home` |
| Users | `ti ti-users` |
| Grade Disputes | `ti ti-gavel` |
| Conversations | `ti ti-messages` |
| Writing Prompts | `ti ti-pencil` |
| Speaking Topics | `ti ti-microphone` |
| Reading Passages | `ti ti-book` |
| Listening Materials | `ti ti-headphones` |
| Test Assignments | `ti ti-clipboard-list` |
| Roadmap Suggestions | `ti ti-map` |
| Announcements | `ti ti-speakerphone` |
| Teacher Profile | `ti ti-user-circle` |
| Audit Logs | `ti ti-history` |
| Settings | `ti ti-settings` |
| Analytics | `ti ti-chart-bar` |

---

## PART G — COMPLETE BUSINESS FLOWS

### G1 — STUDENT COMPLETE FLOW

```
┌─────────────────────────────────────────────────────────────────────────┐
│ ONBOARDING                                                               │
│ 1. POST /api/auth/register → role = "student", status = "active"        │
│ 2. GET  /api/auth/verify-email?token=... → email_verified = true         │
│ 3. POST /api/auth/login → receive JWT                                    │
│ 4. GET  /api/tests/placement → take adaptive test                        │
│ 5. POST /api/tests/placement/submit → band scores stored                 │
│ 6. POST /api/ai/generate-learning-path → AIRoadmap created              │
│ 7. POST /api/users/goals → set target band, exam date, study hours       │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ DAILY DASHBOARD CHECK (BLOCKING ITEMS MUST BE RESOLVED FIRST)           │
│                                                                          │
│ A. Check roadmap suggestions → GET /api/roadmap-suggestions/pending      │
│    IF pending suggestions exist:                                         │
│    → Show blocking banner: "Your teacher suggested roadmap changes"      │
│    → Student CANNOT proceed to studying until resolved                   │
│    → Accept: PATCH /api/roadmap-suggestions/{id}/accept                  │
│      → applies changes to AIRoadmapSteps                                │
│    → Reject: PATCH /api/roadmap-suggestions/{id}/reject                  │
│      → roadmap unchanged, suggestion marked rejected                     │
│                                                                          │
│ B. Check assigned tests → GET /api/assignments/my                        │
│    → See test title, assigned by, deadline (if set), status              │
│    → IF deadline approaching → warning notification                      │
│                                                                          │
│ C. Check notifications → GET /api/notifications                          │
│ D. Check new announcements → GET /api/announcements (active only)        │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ LEARNING LOOP                                                            │
│                                                                          │
│ READING PRACTICE:                                                        │
│ 1. GET /api/reading/practice/{difficulty} → passage + questions          │
│    (correct answers OMITTED from response)                               │
│ 2. POST /api/reading/submit → auto-graded                                │
│    Validation: MC=exact match, TFNG=exact, fill=word bank alias match    │
│ 3. Result stored in tb_user_practice_attempts                            │
│                                                                          │
│ LISTENING PRACTICE:                                                      │
│ 1. GET /api/listening/practice/{difficulty} → material + questions       │
│ 2. POST /api/listening/submit → auto-graded, same validation logic       │
│                                                                          │
│ WRITING PRACTICE:                                                        │
│ 1. GET /api/writing/prompts → published prompts (is_active=true,         │
│    WritingPrompt.status="published" only)                                │
│ 2. POST /api/writing/submit → Gemini AI evaluates:                       │
│    TaskAchievementScore, CoherenceCohesionScore,                        │
│    LexicalResourceScore, GrammarAccuracyScore, BandScore                │
│ 3. Result in tb_writing_submissions + MongoDB writing_analyses_detail    │
│ 4. IF band score feels wrong → POST /api/disputes/submit                 │
│    { submission_type: "writing", submission_id: X, reason: "..." }      │
│                                                                          │
│ SPEAKING PRACTICE:                                                       │
│ 1. GET /api/speaking/topics → published topics (is_active=true)          │
│ 2. POST /api/speaking/start → begin session                              │
│ 3. WebSocket /ws/speaking → real-time AI examiner conversation           │
│    Part 1 (3 turns) → Part 2 (cue card) → Part 3 (abstract)            │
│ 4. POST /api/speaking/submit-audio → AI evaluates:                       │
│    FluencyScore, PronunciationScore, GrammarScore, OverallBand          │
│ 5. Result in tb_speaking_sessions + MongoDB speaking_sessions_detail     │
│ 6. IF band score feels wrong → POST /api/disputes/submit                 │
│    { submission_type: "speaking", submission_id: X, reason: "..." }     │
│                                                                          │
│ LESSONS:                                                                 │
│ 1. GET /api/lessons/courses → all published courses                      │
│ 2. GET /api/lessons/{id} → lesson content                                │
│ 3. POST /api/lessons/{id}/complete → log progress                        │
│                                                                          │
│ AI TOOLS:                                                                │
│ → POST /api/ai/chat → RAG chatbot Q&A                                   │
│ → GET  /api/ai/predict-band → band prediction                            │
│ → GET  /api/ai/recommendations → personalized lesson suggestions         │
│ → GET  /api/ai/insights → skill analysis summary                         │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ TEACHER INTERACTION                                                      │
│                                                                          │
│ DISPUTES:                                                                │
│ 1. POST /api/disputes/submit (role: student only)                        │
│ 2. Student receives notification when teacher claims dispute              │
│ 3. Student receives notification when resolved/rejected (with notes)     │
│ 4. IF resolved with revised score → original submission.band_score       │
│    is updated automatically                                              │
│                                                                          │
│ MESSAGING:                                                               │
│ 1. POST /api/messages/conversations/start → create thread               │
│    → All teachers notified: "new_conversation"                           │
│ 2. GET  /api/messages/conversations/{id}/messages → view thread         │
│ 3. POST /api/messages/conversations/{id}/send → reply                   │
│ 4. PATCH /api/messages/conversations/{id}/mark-read                     │
│                                                                          │
│ VIEW TEACHER PROFILES:                                                   │
│ 1. GET /api/teacher-profiles → list all public teacher profiles          │
│ 2. GET /api/teacher-profiles/{id} → individual teacher bio + specialties │
│                                                                          │
│ VIEW ANNOUNCEMENTS:                                                      │
│ 1. GET /api/announcements → all active, non-expired announcements        │
│    ordered by created_at DESC                                            │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ GAMIFICATION & PROGRESS                                                  │
│ → POST /api/useractivity/log → log daily activity                        │
│ → GET  /api/useractivity/streak → check streak                           │
│ → GET  /api/achievements/earned → view unlocked achievements             │
│ → GET  /api/users/progress → overall progress summary                    │
└─────────────────────────────────────────────────────────────────────────┘
```

---

### G2 — TEACHER COMPLETE FLOW

```
┌─────────────────────────────────────────────────────────────────────────┐
│ ACCESS                                                                   │
│ 1. Admin promotes user via PATCH /api/admin/users/{id}/role              │
│    { role: "teacher" }                                                   │
│ 2. Teacher receives notification: "You have been promoted to Teacher"    │
│ 3. Teacher logs in → JWT role claim = "teacher"                          │
│ 4. Dashboard unlocks: /Teacher/Dashboard/Index                           │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ TEACHER DASHBOARD (Overview)                                             │
│ GET /api/teacher/dashboard →                                             │
│ {                                                                        │
│   pendingDisputes: int,      ← disputes with status="pending"           │
│   openConversations: int,    ← conversations with status="open"         │
│   myActiveConversations: int,← conversations teacher_id=me, "active"   │
│   pendingContentReviews: int,← content with status="pending_review"    │
│   myPendingAssignments: int, ← assignments teacher made, pending        │
│   totalStudents: int,        ← all users with role="student"            │
│   resolvedDisputesThisWeek: int                                          │
│ }                                                                        │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ GRADE DISPUTE WORKFLOW                                                   │
│                                                                          │
│ 1. VIEW: GET /api/disputes?status=pending → open dispute queue           │
│    Teacher sees: student name, submission type, original score, reason   │
│                                                                          │
│ 2. CLAIM: PATCH /api/disputes/{id}/claim                                 │
│    → dispute.status = "under_review"                                     │
│    → dispute.reviewed_by = teacherId                                     │
│    → Notification to student: "dispute_claimed"                          │
│    → AuditHelper: "claim_dispute"                                        │
│                                                                          │
│ 3. REVIEW DETAILS: GET /api/disputes/{id}                                │
│    → Load original submission:                                           │
│      IF submission_type="writing" →                                      │
│        tb_writing_submissions: essay_text, band_score,                   │
│        task_achievement_score, coherence_cohesion_score,                │
│        lexical_resource_score, grammar_accuracy_score, ai_feedback      │
│      IF submission_type="speaking" →                                     │
│        tb_speaking_sessions: transcript, fluency_score,                  │
│        pronunciation_score, grammar_score, ai_feedback                   │
│                                                                          │
│ 4a. RESOLVE (uphold): PATCH /api/disputes/{id}/resolve                   │
│     { teacher_notes: "Explanation...", revised_score: null }             │
│     → dispute.status = "resolved", resolved_at = now                    │
│     → original band_score NOT changed                                    │
│     → Notification to student: "dispute_resolved"                        │
│     → AuditHelper: "resolve_dispute"                                     │
│                                                                          │
│ 4b. RESOLVE (override): PATCH /api/disputes/{id}/resolve                 │
│     { teacher_notes: "Explanation...", revised_score: 7.0 }              │
│     → dispute.status = "resolved", dispute.revised_score = 7.0          │
│     → IF writing: tb_writing_submissions.band_score = 7.0 (by submission_id)│
│     → IF speaking: tb_speaking_sessions: use OverallBand calc or update  │
│       individual scores proportionally                                   │
│     → Notification to student: "dispute_resolved" (with new score)       │
│     → AuditHelper: "resolve_dispute"                                     │
│                                                                          │
│ 4c. REJECT: PATCH /api/disputes/{id}/reject                              │
│     { teacher_notes: "Reason for rejection..." }  ← REQUIRED            │
│     → dispute.status = "rejected"                                        │
│     → Notification to student: "dispute_rejected"                        │
│     → AuditHelper: "reject_dispute"                                      │
│                                                                          │
│ BUSINESS RULE: teacher_notes is REQUIRED for resolve AND reject.         │
│ Return HTTP 400 if missing or whitespace.                                │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ MESSAGING WORKFLOW                                                       │
│                                                                          │
│ 1. VIEW OPEN POOL: GET /api/messages/conversations?status=open           │
│    → All unassigned conversations (teacher_id IS NULL)                   │
│                                                                          │
│ 2. VIEW MY ACTIVE: GET /api/messages/conversations?status=active         │
│    → Conversations where teacher_id = currentTeacherId                   │
│                                                                          │
│ 3. VIEW THREAD: GET /api/messages/conversations/{id}/messages            │
│    → All messages ordered by created_at ASC                              │
│    → Shows student's name + avatar, teacher's name + avatar              │
│                                                                          │
│ 4. FIRST REPLY (claiming the conversation):                              │
│    POST /api/messages/conversations/{id}/send                            │
│    { content: "Hello, I can help..." }                                   │
│    → IF conversation.teacher_id IS NULL:                                 │
│      → conversation.teacher_id = currentTeacherId                        │
│      → conversation.status = "active"                                    │
│    → New tb_messages row added                                           │
│    → conversation.last_message_at = now                                  │
│    → Notification to student: "new_message"                              │
│    → AuditHelper: "reply_conversation"                                   │
│                                                                          │
│ 5. CLOSE: PATCH /api/messages/conversations/{id}/close                   │
│    → conversation.status = "closed"                                      │
│    → AuditHelper: "close_conversation"                                   │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ CONTENT MANAGEMENT WORKFLOW                                              │
│                                                                          │
│ WRITING PROMPTS / SPEAKING TOPICS / READING PASSAGES / LISTENING MATS:  │
│                                                                          │
│ CREATE:                                                                  │
│ → Form submitted → status = "draft", created_by = teacherId             │
│ → Saved to DB                                                            │
│ → AuditHelper: "create_{entity}"                                         │
│ → Content is NOT visible to students yet                                 │
│                                                                          │
│ SUBMIT FOR REVIEW:                                                       │
│ → Teacher updates status to "pending_review"                             │
│ → Other teachers see it in "Content Awaiting Review" section             │
│                                                                          │
│ PEER APPROVE (any teacher EXCEPT the creator):                           │
│ → BUSINESS RULE: content.created_by != currentTeacherId                  │
│ → status = "published", reviewed_by = teacherId, reviewed_at = now      │
│ → Content now visible to students (is_active = true, status = "published")│
│ → Notification to content creator: "content_approved"                   │
│ → AuditHelper: "approve_content"                                         │
│                                                                          │
│ PEER REJECT:                                                             │
│ → status = "rejected"                                                    │
│ → Notification to creator: "content_rejected"                            │
│ → AuditHelper: "reject_content"                                          │
│                                                                          │
│ EDIT (any teacher, own or others):                                       │
│ → IF was "published" → reset to "draft" (needs re-review after change)  │
│ → AuditHelper: "edit_{entity}"                                           │
│                                                                          │
│ SOFT-DELETE:                                                             │
│ → Set is_active = false (WritingPrompt, SpeakingTopic)                  │
│ → OR set IsDeleted = true (Lessons, Modules)                            │
│ → NEVER call .Remove() in Teacher area                                   │
│ → AuditHelper: "soft_delete_{entity}"                                    │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ CUSTOM TEST ASSIGNMENT WORKFLOW (NEW)                                    │
│                                                                          │
│ 1. CREATE TEST:                                                          │
│    Teacher fills form: Title, Sections (skill types), Questions per section│
│    → tb_tests row (is_teacher_created=1, created_by=teacherId, is_public=0)│
│    → tb_test_sections rows per skill selected                            │
│    → tb_questions rows per section                                       │
│    → tb_answers rows per question                                        │
│    → AuditHelper: "create_teacher_test"                                  │
│                                                                          │
│ 2. ASSIGN TO STUDENT:                                                    │
│    Teacher selects: student (from student list), test, instructions,     │
│    deadline (optional — leave blank for self-paced)                      │
│    → tb_teacher_assignments row:                                         │
│      { test_id, teacher_id, student_id, title, instructions, deadline,   │
│        status="pending", assigned_at=now }                               │
│    → Notification to student: "test_assigned"                            │
│      Title: "New test assigned by {teacher name}"                        │
│      Message: "{title}. {deadline message or 'Complete at your own pace'}"|
│    → AuditHelper: "create_assignment"                                    │
│                                                                          │
│ 3. STUDENT COMPLETES:                                                    │
│    Student takes test → POST /api/tests/submit → tb_user_test_attempts   │
│    → tb_teacher_assignments.status = "completed"                         │
│    → tb_teacher_assignments.completed_at = now                           │
│    → tb_teacher_assignments.attempt_id = attempt.AttemptId               │
│    → Notification to teacher: "assignment_completed"                     │
│                                                                          │
│ 4. OVERDUE HANDLING:                                                     │
│    → Background check or on-demand: IF deadline < now AND status=pending │
│    → Update status = "overdue"                                           │
│    → Notification to student: "assignment_overdue"                       │
│                                                                          │
│ 5. REVOKE:                                                               │
│    → tb_teacher_assignments.is_deleted = true                            │
│    → Notification to student: "assignment_revoked"                       │
│    → AuditHelper: "revoke_assignment"                                    │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ ROADMAP SUGGESTION WORKFLOW (NEW)                                        │
│                                                                          │
│ 1. TEACHER VIEWS STUDENT ROADMAP:                                        │
│    GET /api/teacher/students/{id}/detail → includes AIRoadmap + Steps    │
│                                                                          │
│ 2. TEACHER CREATES SUGGESTION:                                           │
│    POST /api/teacher/roadmap-suggestions                                  │
│    {                                                                     │
│      student_id: X,                                                      │
│      roadmap_id: Y,                                                      │
│      suggestion_title: "Focus more on Writing Task 2",                   │
│      message: "I noticed you struggle with coherence...",                │
│      suggested_changes: JSON [{                                          │
│        action: "add",                                                    │
│        week_number: 3,                                                   │
│        description: "Add 2 Writing Task 2 sessions"                     │
│      }]                                                                  │
│    }                                                                     │
│    → tb_roadmap_suggestions row (status="pending")                       │
│    → Notification to student: "roadmap_suggestion"                       │
│      Type = BLOCKING — student cannot study until resolved               │
│    → AuditHelper: "suggest_roadmap_change"                               │
│                                                                          │
│ 3. STUDENT RESOLVES (blocking action on dashboard):                      │
│    Student sees full suggestion title + message + proposed changes       │
│    → ACCEPT: PATCH /api/roadmap-suggestions/{id}/accept                  │
│      → Apply changes to AIRoadmapSteps                                   │
│      → suggestion.status = "accepted"                                    │
│      → Notification to teacher: "roadmap_suggestion_accepted"            │
│    → REJECT: PATCH /api/roadmap-suggestions/{id}/reject                  │
│      → Roadmap unchanged                                                 │
│      → suggestion.status = "rejected"                                    │
│      → Notification to teacher: "roadmap_suggestion_rejected"            │
│    Either way: blocking state cleared, student can continue studying     │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ ANNOUNCEMENTS WORKFLOW (NEW)                                             │
│                                                                          │
│ 1. CREATE:                                                               │
│    POST /api/teacher/announcements                                        │
│    { title, content, expires_at (optional) }                             │
│    → tb_announcements (is_active=true)                                   │
│    → AuditHelper: "create_announcement"                                  │
│    → All students see it on GET /api/announcements (active, not expired) │
│                                                                          │
│ 2. EDIT:                                                                 │
│    PUT /api/teacher/announcements/{id}                                   │
│    → AuditHelper: "edit_announcement"                                    │
│                                                                          │
│ 3. DEACTIVATE (soft-delete):                                             │
│    PATCH /api/teacher/announcements/{id}/deactivate                       │
│    → is_active = false → students can no longer see it                   │
│    → AuditHelper: "deactivate_announcement"                              │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ TEACHER PROFILE WORKFLOW (NEW)                                           │
│                                                                          │
│ 1. First login as teacher → tb_teacher_profiles row auto-created (empty) │
│                                                                          │
│ 2. EDIT OWN PROFILE:                                                     │
│    PUT /api/teacher/profile                                              │
│    { bio, specialties: "Writing,Speaking", years_experience }            │
│    → tb_teacher_profiles updated                                         │
│    → AuditHelper: "edit_teacher_profile"                                 │
│                                                                          │
│ 3. Students browse: GET /api/teacher-profiles                            │
│    → Only profiles with is_public=true                                   │
│    → Shows: full_name (from tb_user_profiles), bio, specialties          │
└─────────────────────────────────────────────────────────────────────────┘
```

---

### G3 — ADMIN COMPLETE FLOW

```
┌─────────────────────────────────────────────────────────────────────────┐
│ ACCESS                                                                   │
│ 1. Admin account created directly in DB with role="admin"                │
│ 2. Login → JWT role = "admin"                                            │
│ 3. Admin dashboard: /Admin/Dashboard/Index                               │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ USER MANAGEMENT (Admin/Users)                                            │
│                                                                          │
│ INDEX:                                                                   │
│ → Search: email OR full_name (case-insensitive LIKE)                     │
│ → Filter: role (student/teacher/admin/moderator), status (active/banned) │
│ → Paginate: 10 per page, order by created_at DESC                        │
│ → Columns: initials avatar, full name, email, role badge, status badge,  │
│   login count, last login, created at, actions                           │
│ → Actions: Details | Edit | Ban/Unban (toggle) | Delete                  │
│                                                                          │
│ CREATE:                                                                  │
│ → Fields: Email, Password, Role (select), Status (select)                │
│ → Validate: email unique, password not empty                             │
│ → Hash: BCrypt.Net.BCrypt.HashPassword(password)                         │
│ → Also create: tb_user_profiles row (full_name = email prefix)           │
│ → AuditHelper: "create_user"                                             │
│                                                                          │
│ EDIT (role + status only):                                               │
│ → Role: student | teacher | admin | moderator                            │
│ → Status: active | banned | inactive                                     │
│ → BLOCK: cannot edit own account                                         │
│ → AuditHelper: "promote_user" or "ban_user" depending on change          │
│                                                                          │
│ TOGGLE BAN (quick action from Index):                                    │
│ → status: "active" ↔ "banned"                                            │
│ → BLOCK: cannot ban own account                                          │
│ → AuditHelper: "ban_user" or "unban_user"                                │
│                                                                          │
│ HARD DELETE:                                                             │
│ → ONLY place .Remove() is permitted                                      │
│ → BLOCK: cannot delete own account                                       │
│ → Log audit BEFORE deletion (entity won't exist after)                   │
│ → AuditHelper: "hard_delete_user"                                        │
│ → May need to delete child records first due to FK constraints           │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ CONTENT OVERSIGHT (Admin can see + restore soft-deleted content)          │
│                                                                          │
│ → View ALL content including is_active=false (soft-deleted by teachers)  │
│ → Restore: set is_active = true, status = "published"                   │
│ → Hard-delete: only Admin can call .Remove() on content                  │
│ → AuditHelper: "restore_content" or "hard_delete_content"               │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ AUDIT LOGS (Admin/AuditLogs)                                             │
│                                                                          │
│ → View tb_audit_logs with filters:                                       │
│   Filter by: action, entity_type, user_id (email search), date range     │
│ → Read-only — no edit or delete                                          │
│ → Columns: timestamp, user (email), action, entity type, entity ID,      │
│   old values (collapsible JSON), new values (collapsible JSON), IP       │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ ANNOUNCEMENTS OVERSIGHT (Admin)                                          │
│                                                                          │
│ → View ALL announcements (all teachers' posts)                           │
│ → Can deactivate any announcement (inappropriate content)                │
│ → Cannot create announcements (that's a Teacher function)               │
└─────────────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ SYSTEM SETTINGS (Admin/Settings)                                         │
│                                                                          │
│ → Read/update tb_system_settings key-value pairs                         │
│ → Settings: site name, maintenance mode, max login attempts, etc.        │
│ → AuditHelper: "update_system_setting"                                   │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## PART H — COMPLETE AREA CONTROLLER MAP (all files to build)

### Admin Area Controllers

| Controller | Actions | Views |
|---|---|---|
| `UsersController` | Index (search+paginate), Create, Edit (role+status), Details, Delete (hard), ToggleBan | Index, Create, Edit, Details, Delete |
| `AuditLogsController` | Index (filter+paginate), Details | Index, Details |
| `AnnouncementsController` | Index (all teachers'), Deactivate (POST) | Index |
| `SettingsController` | Index, Edit | Index, Edit |

### Teacher Area Controllers

| Controller | Actions | Views |
|---|---|---|
| `GradeDisputesController` | Index (tabbed), Details, Claim (POST), Edit (resolve form), Resolve (POST), Reject (POST) | Index, Details, Edit |
| `ConversationsController` | Index (open pool + my active), Details (thread), Edit/Reply (POST), Close (POST) | Index, Details, Edit |
| `WritingPromptsController` | Index, Create, Edit, Details, Delete (soft), Approve (POST), Reject (POST) | Index, Create, Edit, Details, Delete |
| `SpeakingTopicsController` | Index, Create, Edit, Details, Delete (soft), Approve (POST), Reject (POST) | Index, Create, Edit, Details, Delete |
| `ReadingPassagesController` | Index, Create, Edit, Details, Delete (soft) | Index, Create, Edit, Details, Delete |
| `ListeningMaterialsController` | Index, Create, Edit, Details, Delete (soft) | Index, Create, Edit, Details, Delete |
| `TestAssignmentsController` | Index, CreateTest, Assign, Edit (deadline), Details, Delete (soft/revoke) | Index, CreateTest, Assign, Details, Delete |
| `RoadmapSuggestionsController` | Index, Create (select student + roadmap), Details | Index, Create, Details |
| `AnnouncementsController` | Index, Create, Edit, Delete (soft/deactivate) | Index, Create, Edit, Delete |
| `TeacherProfileController` | Edit (own profile only) | Edit |

---

## PART I — NOTIFICATION TYPE REFERENCE (complete)

| Type | Triggered by | Recipient |
|---|---|---|
| `dispute_claimed` | Teacher claims dispute | Student |
| `dispute_resolved` | Teacher resolves dispute | Student |
| `dispute_rejected` | Teacher rejects dispute | Student |
| `new_message` | Teacher replies to conversation | Student |
| `new_conversation` | Student starts conversation | All teachers (broadcast) |
| `content_approved` | Peer teacher approves content | Content creator (teacher) |
| `content_rejected` | Peer teacher rejects content | Content creator (teacher) |
| `test_assigned` | Teacher assigns test to student | Student |
| `assignment_completed` | Student completes assigned test | Teacher who assigned |
| `assignment_overdue` | Deadline passed, not completed | Student |
| `assignment_revoked` | Teacher revokes assignment | Student |
| `roadmap_suggestion` | Teacher suggests roadmap change | Student (BLOCKING) |
| `roadmap_suggestion_accepted` | Student accepts suggestion | Teacher |
| `roadmap_suggestion_rejected` | Student rejects suggestion | Teacher |
| `user_promoted` | Admin promotes to teacher | New teacher |

---

## PART J — SIDEBAR NAV SPEC PER AREA

### Admin Sidebar
```
MAIN
  Dashboard           ti ti-home
  Users               ti ti-users
CONTENT
  Announcements       ti ti-speakerphone  (view all, deactivate)
SYSTEM
  Audit Logs          ti ti-history
  Settings            ti ti-settings
```

### Teacher Sidebar
```
MAIN
  Dashboard           ti ti-home
  My Students         ti ti-users
ACADEMIC SUPPORT
  Grade Disputes      ti ti-gavel        (badge: pending count)
  Conversations       ti ti-messages     (badge: open pool count)
  Roadmap Suggestions ti ti-map
CONTENT
  Writing Prompts     ti ti-pencil
  Speaking Topics     ti ti-microphone
  Reading Passages    ti ti-book
  Listening Materials ti ti-headphones
  Test Assignments    ti ti-clipboard-list
PLATFORM
  Announcements       ti ti-speakerphone
  My Profile          ti ti-user-circle
```
