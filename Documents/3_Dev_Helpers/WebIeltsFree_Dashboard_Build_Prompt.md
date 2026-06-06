# WebIeltsFree — Management Dashboard Build Prompt
## Full AI Coding Context + Task Specification

> **HOW TO USE THIS FILE:**
> Paste this entire document at the start of a new AI coding session.
> The AI must execute Section 0.5 Pre-Flight Steps BEFORE writing any code.
> Then read every remaining section in order — each one prevents a specific category of bugs.
> Build order is strict: complete ALL Admin area files before starting ANY Teacher area file.

---

## SECTION 0 — SYSTEM ROLE

You are a Senior ASP.NET Core 8 Full-Stack Developer.
Your job is to build a management dashboard for an existing, production-ready web application called **WebIeltsFree** — an AI-powered IELTS learning platform.

You must follow every rule in this document exactly. If something is ambiguous, apply the most defensive, safest interpretation. Do not invent patterns that are not in this document.

---

## SECTION 0.5 — PRE-FLIGHT STEPS (MANDATORY — COMPLETE BEFORE WRITING ANY CODE)

Execute these three steps in order. Do not write a single line of C# or Razor until all three are done and their findings are documented in Section 13.

---

### Step 1 — Install Design Skills

Run both commands. If a skill is already installed, skip it. Read each skill's full output before continuing.

```bash
npx skills add https://github.com/anthropics/skills --skill frontend-design
npx skills add https://github.com/nextlevelbuilder/ui-ux-pro-max-skill --skill ui-ux-pro-max
```

These skills define the typography system, spacing rules, animation patterns, color theory, and component aesthetics you must follow for every view. They override generic Bootstrap defaults wherever they conflict. Read them fully — they are authoritative.

---

### Step 2 — Read and Extract the Template

Scan this folder completely: `~/WebIeltsFree/Template_IELTS/Admin_Teacher/`

Read every file in that folder. Then extract and document the following before writing any view:

| What to extract | Why it matters |
|---|---|
| Layout structure (sidebar, topbar, content area) | Every view inherits this shell — get it wrong and the layout breaks across all pages |
| CSS class names for cards, tables, badges, buttons, forms | Use these exact classes, NOT generic Bootstrap equivalents |
| Color token names or CSS variable names (e.g. `--primary`, `--sidebar-bg`) | Hard-coding hex values breaks theme consistency |
| Sidebar navigation link pattern | Your area controllers must register links here |
| Table row action button pattern | Index views must use the same pattern the template uses |
| Alert / toast / notification pattern | TempData["Success"] and TempData["Warning"] messages use these |
| Form layout pattern (label placement, input sizing, validation styles) | Create/Edit views must match the template exactly |

**RULE:** Every Razor view you generate must use the classes and layout patterns extracted from this template. Do NOT fall back to default Bootstrap markup where the template has its own component pattern. If the template uses a custom card class like `card-dashboard` instead of `card`, use `card-dashboard`.

---

### Step 3 — Scan for Duplicate Action Names

Scan every `.cs` file in `~/WebIeltsFree/Controllers/` (root controllers only, not Areas).

For each file found, list every `public IActionResult` and `public async Task<IActionResult>` method signature.

Then compare against every action you are about to create across both Areas.

Record all findings in **Section 13** of this document using the table format shown there.

**What counts as a collision:**
- Same controller class name AND same action method name, existing in both `Controllers/` and `Areas/*/Controllers/`
- Example: `Controllers/WritingController.cs` has `public IActionResult Index()` AND `Areas/Teacher/Controllers/WritingPromptsController.cs` also has `public IActionResult Index()`
- Areas are separately namespaced so this does NOT cause a compile error, but it CAN cause routing confusion if `[Area]` attributes are missing. Every collision must be verified to have explicit `[Area]` decoration.

**What to do with collisions:**
- Document them in Section 13
- Verify the area controller has `[Area("Admin")]` or `[Area("Teacher")]` explicitly
- Do NOT rename area actions to avoid collisions — the area attribute is the correct fix

---

## SECTION 1 — PROJECT CONTEXT

**What the app does:**
WebIeltsFree is a free IELTS preparation platform. It provides adaptive learning, AI-graded Speaking and Writing practice, an AI chatbot, and human teacher support. Students target band scores 0–9.

**Current version:** 2.2.0 (production-ready)

**What you are building:**
A server-rendered MVC management dashboard using ASP.NET Core **Areas**. Two areas:
- `Admin` — system management (users, roles, bans, hard-deletes)
- `Teacher` — academic management (disputes, messaging, content)

This dashboard is separate from the existing API controllers. Do NOT modify any existing controllers in the root `Controllers/` folder.

---

## SECTION 2 — TECH STACK

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 MVC + Entity Framework Core |
| ORM | EF Core with Pomelo.EntityFrameworkCore.MySql 8.0.2 |
| Database | MySQL 8.0 (primary), MongoDB Atlas (logs/AI), Pinecone (vectors) |
| Auth | JWT Bearer + ASP.NET Core Identity-style role claims |
| Password hashing | BCrypt.Net-Next 4.0.3 |
| UI | Bootstrap 5 (CDN) + vanilla JS |
| Logging | Serilog (already configured) |
| Migrations | NOT USED — project uses Database.EnsureCreated() + raw SQL scripts only |

---

## SECTION 3 — CRITICAL RULES (NEVER VIOLATE)

### R1 — Role strings are LOWERCASE
The `tb_users.role` column stores: `'student'`, `'teacher'`, `'admin'`, `'moderator'`.
The JWT `role` claim carries exactly this value. ASP.NET Core role checks are case-sensitive.

```csharp
// ✅ CORRECT
[Authorize(Roles = "admin")]
[Authorize(Roles = "teacher,admin")]
[Authorize(Roles = "student")]

// ❌ WRONG — will silently fail, no 403, just always unauthorized
[Authorize(Roles = "Admin")]
[Authorize(Roles = "Teacher, Admin")]
```

### R2 — Never use EF migrations
Do NOT run `dotnet ef migrations add`. Do NOT call `Database.Migrate()`.
The project uses `Database.EnsureCreated()` and raw SQL scripts in `DatabaseScripts/`.
If you need a new column, note it as a SQL script comment alongside the code.

### R3 — Never use EF .Remove() in Teacher area
Teachers can only soft-delete. Never call `_context.SomeTable.Remove(entity)` in any Teacher area controller.
Admin area UsersController is the only place where `.Remove()` is permitted (hard-delete).

### R4 — Always wrap API responses in ApiResponse<T>
This only applies to controllers in the root `Controllers/` folder (existing API).
Area controllers (Admin/Teacher) return standard MVC `View()`, `RedirectToAction()`, `Json()` results — NOT ApiResponse<T>.

### R5 — Always call AuditHelper on mutations
Every Create, Edit, SoftDelete, HardDelete, Claim, Resolve, Reject action in any area controller
must call `AuditHelper.LogAsync()`. See Section 7 for the exact signature.

### R6 — Sanitize all user text input
Any text input from a form must be sanitized before saving:
```csharp
var clean = InputSanitizer.StripHtmlTags(model.SomeTextField);
```

### R7 — Teachers cannot approve their own content
When the content approval workflow is active (see Task 2.1), a teacher cannot approve
a piece of content they created themselves. Check: `content.CreatedBy != currentUserId`.

### R8 — Score override requires TeacherNotes
When a teacher resolves a dispute, `TeacherNotes` is **required** (not nullable in business logic).
Return a model error and re-render the form if it is empty or whitespace.

### R9 — Polymorphic dispute score sync
`tb_grade_disputes.submission_type` is either `'writing'` or `'speaking'`.
When resolving with a `RevisedScore`, you must branch:
- `'writing'` → update `band_score` in `tb_writing_submissions` where `submission_id` matches
- `'speaking'` → update `band_score` in `tb_speaking_sessions` where `session_id` matches

### R10 — Prevent admin self-lockout
In UsersController (Admin area), block any action that would edit or delete the currently
logged-in admin's own account. Check: `targetUserId == GetCurrentUserId()` → redirect with warning.

### R11 — Area boilerplate is mandatory
Each area requires `_ViewImports.cshtml` and `_ViewStart.cshtml` inside its `Views/` folder.
Without these, Tag Helpers won't resolve and layouts won't load.

### R12 — GetCurrentUserId() pattern
Use this helper in every area controller:
```csharp
private int GetCurrentUserId()
{
    var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return int.TryParse(claim, out int id) ? id : 0;
}
```

### R13 — All views must use the template from Admin_Teacher folder
Every `.cshtml` file you generate must be built on top of the extracted template (Step 2 of Section 0.5).
- Use the template's layout file as `Layout` in `_ViewStart.cshtml`
- Use the template's exact CSS class names for every component
- Apply the `frontend-design` and `ui-ux-pro-max` skill aesthetics ON TOP of the template's structure
- The skills define visual quality and creative direction; the template defines structural constraints
- When they conflict, the template structure wins; the skills govern typography, color refinement, and micro-interactions

### R14 — Build order is Admin-first, then Teacher
Complete every Admin area file (controllers + all views) before starting any Teacher area file.
Do not interleave. The output section lists the exact order — follow it.

### R15 — No hardcoded sub-scores in WritingPrompt or SpeakingTopic
- The prompt/topic YAML should only have `title`, `difficulty`, `type`, `estimated_time`
- Never include fields like `vocabulary_score`, `grammar_score`, `organization_score`, `topic_response_score` in the entity model
- Those fields only appear in `WritingSubmission` and `SpeakingSession` after analysis
- The template YAML is misleading — ignore the sub-score columns in `tb_writing_prompts` and `tb_speaking_topics` in `Entities.cs`
- Those columns should not exist — remove them from the C# model if present





## SECTION 4 — ENTITY MODELS

These are the EF Core entity classes already defined in `WebIeltsFree/Models/Entities.cs`.
Use them exactly as shown. Do not rename properties or change attributes.

```csharp
// ── tb_users ────────────────────────────────────────────────────────────────
[Table("tb_users")]
public class User
{
    [Key][Column("user_id")]        public int UserId { get; set; }
    [Column("email")]               public string Email { get; set; } = "";
    [Column("password_hash")]       public string PasswordHash { get; set; } = "";
    [Column("role")]                public string Role { get; set; } = "student";
    [Column("status")]              public string Status { get; set; } = "active";
    // status values: "active" | "banned" | "inactive"
    [Column("created_at")]          public DateTime CreatedAt { get; set; }
    public UserProfile? Profile { get; set; }
    public UserGoal? Goals { get; set; }
}

// ── tb_user_profiles ────────────────────────────────────────────────────────
[Table("tb_user_profiles")]
public class UserProfile
{
    [Key][Column("user_id")]        public int UserId { get; set; }
    [Column("full_name")]           public string? FullName { get; set; }
    [Column("avatar_url")]          public string? AvatarUrl { get; set; }
    [Column("country")]             public string? Country { get; set; }
    [Column("timezone")]            public string? Timezone { get; set; }
    [Column("created_at")]          public DateTime CreatedAt { get; set; }
}

// ── tb_user_goals ────────────────────────────────────────────────────────────
[Table("tb_user_goals")]
public class UserGoal
{
    [Key][Column("goal_id")]        public int GoalId { get; set; }
    [Column("user_id")]             public int UserId { get; set; }
    [Column("current_band")]        public float CurrentBand { get; set; }
    [Column("target_band")]         public float TargetBand { get; set; }
    [Column("exam_date")]           public DateTime? ExamDate { get; set; }
    [Column("study_hours_per_day")] public float StudyHoursPerDay { get; set; }
}

// ── tb_grade_disputes ────────────────────────────────────────────────────────
[Table("tb_grade_disputes")]
public class GradeDispute
{
    [Key][Column("dispute_id")]     public int DisputeId { get; set; }
    [Column("user_id")]             public int UserId { get; set; }
    [Column("submission_type")]     public string SubmissionType { get; set; } = "";
    // submission_type values: "writing" | "speaking"
    [Column("submission_id")]       public int SubmissionId { get; set; }
    [Column("reason")]              public string Reason { get; set; } = "";
    [Column("status")]              public string Status { get; set; } = "pending";
    // status values: "pending" | "under_review" | "resolved" | "rejected"
    [Column("reviewed_by")]         public int? ReviewedBy { get; set; }
    [Column("original_score")]      public float OriginalScore { get; set; }
    [Column("revised_score")]       public float? RevisedScore { get; set; }
    [Column("teacher_notes")]       public string? TeacherNotes { get; set; }
    [Column("created_at")]          public DateTime CreatedAt { get; set; }
    [Column("resolved_at")]         public DateTime? ResolvedAt { get; set; }
    public User? User { get; set; }
    public User? Reviewer { get; set; }
}

// ── tb_writing_submissions ───────────────────────────────────────────────────
[Table("tb_writing_submissions")]
public class WritingSubmission
{
    [Key][Column("submission_id")]  public int SubmissionId { get; set; }
    [Column("user_id")]             public int UserId { get; set; }
    [Column("essay_text")]          public string EssayText { get; set; } = "";
    [Column("band_score")]          public float BandScore { get; set; }
    [Column("ta_score")]            public float TaScore { get; set; }
    [Column("cc_score")]            public float CcScore { get; set; }
    [Column("lr_score")]            public float LrScore { get; set; }
    [Column("ga_score")]            public float GaScore { get; set; }
    [Column("ai_feedback")]         public string? AiFeedback { get; set; }
    [Column("created_at")]          public DateTime CreatedAt { get; set; }
}

// ── tb_speaking_sessions ─────────────────────────────────────────────────────
[Table("tb_speaking_sessions")]
public class SpeakingSession
{
    [Key][Column("session_id")]     public int SessionId { get; set; }
    [Column("user_id")]             public int UserId { get; set; }
    [Column("topic")]               public string? Topic { get; set; }
    [Column("band_score")]          public float BandScore { get; set; }
    [Column("fluency_score")]       public float FluencyScore { get; set; }
    [Column("pronunciation_score")] public float PronunciationScore { get; set; }
    [Column("grammar_score")]       public float GrammarScore { get; set; }
    [Column("transcript")]          public string? Transcript { get; set; }
    [Column("ai_feedback")]         public string? AiFeedback { get; set; }
    [Column("created_at")]          public DateTime CreatedAt { get; set; }
}

// ── tb_writing_prompts ───────────────────────────────────────────────────────
// NOTE: requires these columns in MySQL (add via SQL script if missing):
//   `status` ENUM('draft','pending_review','published','rejected') NOT NULL DEFAULT 'draft'
//   `created_by` INT NULL (FK tb_users.user_id)
//   `reviewed_by` INT NULL (FK tb_users.user_id)
//   `reviewed_at` DATETIME NULL
[Table("tb_writing_prompts")]
public class WritingPrompt
{
    [Key][Column("prompt_id")]      public int PromptId { get; set; }
    [Column("task_type")]           public string TaskType { get; set; } = "";
    // task_type: "task1" | "task2"
    [Column("prompt_text")]         public string PromptText { get; set; } = "";
    [Column("sample_answer")]       public string? SampleAnswer { get; set; }
    [Column("difficulty")]          public string Difficulty { get; set; } = "";
    // difficulty: "band_4_5" | "band_5_6" | "band_6_7" | "band_7_8" | "band_8_9"
    [Column("status")]              public string Status { get; set; } = "draft";
    // status: "draft" | "pending_review" | "published" | "rejected"
    [Column("created_by")]          public int? CreatedBy { get; set; }
    [Column("reviewed_by")]         public int? ReviewedBy { get; set; }
    [Column("reviewed_at")]         public DateTime? ReviewedAt { get; set; }
    [Column("created_at")]          public DateTime CreatedAt { get; set; }
}

// ── tb_speaking_topics ───────────────────────────────────────────────────────
// NOTE: same SQL columns needed as WritingPrompt above
[Table("tb_speaking_topics")]
public class SpeakingTopic
{
    [Key][Column("topic_id")]       public int TopicId { get; set; }
    [Column("title")]               public string Title { get; set; } = "";
    [Column("part_number")]         public int PartNumber { get; set; }
    // part_number: 1 | 2 | 3
    [Column("difficulty")]          public string Difficulty { get; set; } = "";
    [Column("status")]              public string Status { get; set; } = "draft";
    // status: "draft" | "pending_review" | "published" | "rejected"
    [Column("created_by")]          public int? CreatedBy { get; set; }
    [Column("reviewed_by")]         public int? ReviewedBy { get; set; }
    [Column("reviewed_at")]         public DateTime? ReviewedAt { get; set; }
    [Column("created_at")]          public DateTime CreatedAt { get; set; }
}

// ── tb_conversations ─────────────────────────────────────────────────────────
[Table("tb_conversations")]
public class Conversation
{
    [Key][Column("conversation_id")] public int ConversationId { get; set; }
    [Column("student_id")]           public int StudentId { get; set; }
    [Column("teacher_id")]           public int? TeacherId { get; set; }
    [Column("subject")]              public string? Subject { get; set; }
    [Column("status")]               public string Status { get; set; } = "open";
    // status: "open" | "active" | "closed"
    [Column("created_at")]           public DateTime CreatedAt { get; set; }
    [Column("last_message_at")]      public DateTime? LastMessageAt { get; set; }
    public User? Student { get; set; }
    public User? Teacher { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

// ── tb_messages ──────────────────────────────────────────────────────────────
[Table("tb_messages")]
public class Message
{
    [Key][Column("message_id")]      public int MessageId { get; set; }
    [Column("conversation_id")]      public int ConversationId { get; set; }
    [Column("sender_id")]            public int SenderId { get; set; }
    [Column("content")]              public string Content { get; set; } = "";
    [Column("is_read")]              public bool IsRead { get; set; }
    [Column("created_at")]           public DateTime CreatedAt { get; set; }
    public User? Sender { get; set; }
}

// ── tb_notifications ─────────────────────────────────────────────────────────
[Table("tb_notifications")]
public class Notification
{
    [Key][Column("notification_id")] public int NotificationId { get; set; }
    [Column("user_id")]              public int UserId { get; set; }
    [Column("type")]                 public string Type { get; set; } = "";
    [Column("title")]                public string Title { get; set; } = "";
    [Column("message")]              public string Message { get; set; } = "";
    [Column("is_read")]              public bool IsRead { get; set; }
    [Column("created_at")]           public DateTime CreatedAt { get; set; }
    [Column("expires_at")]           public DateTime? ExpiresAt { get; set; }
}
```

---

## SECTION 5 — AppDbContext DbSets

These `DbSet` properties already exist in `AppDbContext.cs`. Reference them exactly:

```csharp
public DbSet<User>               Users              { get; set; }
public DbSet<UserProfile>        UserProfiles       { get; set; }
public DbSet<UserGoal>           UserGoals          { get; set; }
public DbSet<GradeDispute>       GradeDisputes      { get; set; }
public DbSet<WritingSubmission>  WritingSubmissions { get; set; }
public DbSet<SpeakingSession>    SpeakingSessions   { get; set; }
public DbSet<WritingPrompt>      WritingPrompts     { get; set; }
public DbSet<SpeakingTopic>      SpeakingTopics     { get; set; }
public DbSet<Conversation>       Conversations      { get; set; }
public DbSet<Message>            Messages           { get; set; }
public DbSet<Notification>       Notifications      { get; set; }
```

---

## SECTION 6 — EXISTING UTILITY CLASSES

### AuditHelper (already exists in Models/)

```csharp
// Signature — call this on every data mutation in area controllers:
await AuditHelper.LogAsync(
    context:    _context,
    userId:     int,          // GetCurrentUserId()
    action:     string,       // "Create" | "Edit" | "SoftDelete" | "HardDelete"
                              // "Claim" | "Resolve" | "Reject" | "Approve" | "Reply"
    entityType: string,       // "WritingPrompt" | "SpeakingTopic" | "GradeDispute"
                              // "User" | "Conversation"
    entityId:   string,       // primary key as string
    details:    string        // human-readable description of the change
);
```

### InputSanitizer (already exists in Models/Middleware.cs)

```csharp
// Call on any user-supplied text before saving to DB:
var clean = InputSanitizer.StripHtmlTags(rawInput);
```

### Notification helper pattern (inline — no separate service)

```csharp
// Create a notification for a user directly via AppDbContext:
_context.Notifications.Add(new Notification
{
    UserId    = targetUserId,
    Type      = "dispute_resolved",   // snake_case type string
    Title     = "Your dispute was resolved",
    Message   = $"Teacher reviewed your submission. Notes: {teacherNotes}",
    IsRead    = false,
    CreatedAt = DateTime.UtcNow,
    ExpiresAt = DateTime.UtcNow.AddDays(30)
});
// Always save as part of the same SaveChangesAsync() call.
```

---

## SECTION 7 — FOLDER STRUCTURE TO CREATE

```
WebIeltsFree/
├── Areas/
│   ├── Admin/
│   │   ├── Controllers/
│   │   │   └── UsersController.cs
│   │   └── Views/
│   │       ├── _ViewImports.cshtml        ← REQUIRED
│   │       ├── _ViewStart.cshtml          ← REQUIRED
│   │       └── Users/
│   │           ├── Index.cshtml
│   │           ├── Create.cshtml
│   │           ├── Edit.cshtml
│   │           ├── Details.cshtml
│   │           └── Delete.cshtml
│   └── Teacher/
│       ├── Controllers/
│       │   ├── GradeDisputesController.cs
│       │   ├── WritingPromptsController.cs
│       │   ├── SpeakingTopicsController.cs
│       │   └── ConversationsController.cs
│       └── Views/
│           ├── _ViewImports.cshtml        ← REQUIRED
│           ├── _ViewStart.cshtml          ← REQUIRED
│           ├── GradeDisputes/
│           │   ├── Index.cshtml
│           │   ├── Details.cshtml
│           │   └── Edit.cshtml            ← Full Bootstrap 5 UI required
│           ├── WritingPrompts/
│           │   ├── Index.cshtml
│           │   ├── Create.cshtml
│           │   ├── Edit.cshtml
│           │   ├── Details.cshtml
│           │   └── Delete.cshtml
│           ├── SpeakingTopics/
│           │   ├── Index.cshtml
│           │   ├── Create.cshtml
│           │   ├── Edit.cshtml
│           │   ├── Details.cshtml
│           │   └── Delete.cshtml
│           └── Conversations/
│               ├── Index.cshtml
│               ├── Details.cshtml
│               └── Edit.cshtml
```

### Required boilerplate — generate these first

**Both areas use identical content:**

`_ViewImports.cshtml`:
```cshtml
@using WebIeltsFree
@using WebIeltsFree.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

`_ViewStart.cshtml`:
```cshtml
@{
    Layout = "_Layout";
}
```

### Program.cs — area route (add BEFORE default route)

```csharp
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
```

---

## SECTION 8 — TASK 1: ADMIN AREA

**Path:** `~/Areas/Admin/...`
**Attribute on every controller:** `[Area("Admin")]` + `[Authorize(Roles = "admin")]`

### UsersController.cs

**Actions required:**

#### Index
- Query `tb_users` with `.Include(u => u.Profile)`
- Filters (all optional, passed as query params):
  - `search` — case-insensitive match on `Email` OR `Profile.FullName`
  - `role` — exact match (`student`, `teacher`, `admin`, `moderator`)
  - `status` — exact match (`active`, `banned`, `inactive`)
- **Pagination:** 10 per page. Pass to view: `CurrentPage`, `TotalPages`, `TotalCount`
- Order by `CreatedAt` descending

#### Create
- Form fields: `Email`, `Role` (select), `Status` (select), `Password` (not bound to entity)
- Validate: password not empty, email not already taken (`AnyAsync`)
- Hash password: `BCrypt.Net.BCrypt.HashPassword(password)`
- After save: also create a `UserProfile` row with `UserId` and `FullName` defaulting to the email prefix
- Call `AuditHelper.LogAsync` with action `"Create"`

#### Details
- Include `Profile` and `Goals`
- Read-only view

#### Edit
- Only editable fields: `Role` (select) and `Status` (select)
- Validate role against whitelist: `["student","teacher","admin","moderator"]`
- Block editing own account (R10)
- Call `AuditHelper.LogAsync` with action `"Edit"`, include old→new values in details

#### ToggleBan (POST only, quick-action from Index)
- Toggles `Status` between `"active"` and `"banned"`
- Block banning own account
- Call `AuditHelper.LogAsync`

#### Delete (GET confirmation + POST confirm)
- **Hard-delete** using `_context.Users.Remove(user)` — this is the ONLY place Remove() is allowed
- Block deleting own account
- Call `AuditHelper.LogAsync` with action `"HardDelete"` BEFORE removing
- Note in a comment that FK cascade constraints may require deleting child records first

**Index.cshtml requirements:**
- Bootstrap 5 table with columns: Avatar (initials fallback), Full Name, Email, Role (colored badge), Status (colored badge), Created At, Actions
- Search bar + Role filter dropdown + Status filter dropdown in a filter card above the table
- Pagination controls at the bottom
- Action buttons per row: Details, Edit, Ban/Unban (toggle label), Delete
- `TempData["Success"]` and `TempData["Warning"]` alert banners at top

---

## SECTION 9 — TASK 2: TEACHER AREA

**Path:** `~/Areas/Teacher/...`
**Attribute on every controller:** `[Area("Teacher")]` + `[Authorize(Roles = "teacher,admin")]`

---

### Task 2.1 — WritingPromptsController.cs & SpeakingTopicsController.cs

Both controllers follow the same pattern. Describe both below.

**Actions: Index, Create, Edit, Details, Delete (soft)**

#### Index
- Show only content where `Status != "rejected"` by default
- Filter tabs: All | Draft | Pending Review | Published
- Show who created it (`CreatedBy` user's name via join or separate query)
- Teachers see all statuses; students (outside this area) only see `"published"` content

#### Create
- Set `CreatedBy = GetCurrentUserId()` and `CreatedAt = DateTime.UtcNow`
- Set initial `Status = "draft"`
- Sanitize all text fields with `InputSanitizer.StripHtmlTags()`
- Call `AuditHelper.LogAsync` with action `"Create"`
- After save: do NOT auto-publish. Status stays `"draft"` until another teacher approves.

#### Edit
- Allow editing `PromptText`/`Title`, `TaskType`/`PartNumber`, `Difficulty`, `SampleAnswer`
- **Editing resets status to `"draft"`** if it was `"published"` or `"rejected"` (content changed = needs re-review)
- Sanitize all text fields
- Call `AuditHelper.LogAsync` with action `"Edit"`

#### Approve (POST action — peer review)
- A teacher can approve another teacher's content (sets `Status = "published"`)
- **Block self-approval:** if `content.CreatedBy == GetCurrentUserId()` → return error "You cannot approve your own content"
- Set `ReviewedBy = GetCurrentUserId()`, `ReviewedAt = DateTime.UtcNow`
- Call `AuditHelper.LogAsync` with action `"Approve"`

#### Reject (POST action)
- Sets `Status = "rejected"`
- Same self-approval block applies
- Call `AuditHelper.LogAsync` with action `"Reject"`

#### Delete (soft only — NEVER use .Remove())
- Set `Status = "rejected"` (effectively archives it, hides from students)
- Do NOT call `_context.WritingPrompts.Remove()` or `_context.SpeakingTopics.Remove()`
- The Delete confirmation view should warn: "This will archive the content. Admins can restore it."
- Call `AuditHelper.LogAsync` with action `"SoftDelete"`

---

### Task 2.2 — GradeDisputesController.cs

**No Create or Delete actions.** Scaffold only: Index, Details, Edit.

#### Index
- Load from `tb_grade_disputes`, include `User` + `User.Profile`
- **Tabbed display** by status. Four tabs with badge counts:
  - `Pending` — all disputes with `status = "pending"`
  - `Under Review` — `status = "under_review"`. Non-admin teachers see only disputes they claimed (`reviewed_by == currentUserId`). Admins see all.
  - `Resolved` — `status = "resolved"`
  - `Rejected` — `status = "rejected"`
- Pass `ActiveTab` to view via `ViewBag.ActiveTab`
- Pass counts: `ViewBag.PendingCount`, `ViewBag.UnderReviewCount`, `ViewBag.ResolvedCount`, `ViewBag.RejectedCount`

#### Details
- Load dispute, include `User.Profile`
- Load and display the original submission:
  - If `SubmissionType == "writing"`: load `WritingSubmission` by `SubmissionId`, show `EssayText`, `BandScore`, sub-scores (`TA`, `CC`, `LR`, `GA`), `AiFeedback`
  - If `SubmissionType == "speaking"`: load `SpeakingSession` by `SubmissionId`, show `Transcript`, `BandScore`, `FluencyScore`, `PronunciationScore`, `GrammarScore`, `AiFeedback`
- Show a "Claim this dispute" button if `Status == "pending"`
- Show "Resolve" / "Reject" buttons if `Status == "under_review"` and `ReviewedBy == currentUserId`

#### Claim (POST action — triggered from Details or Index)
- Validate: dispute must have `Status == "pending"`
- Set `Status = "under_review"`, `ReviewedBy = GetCurrentUserId()`
- Save changes
- Create notification for the student:
  - `Type = "dispute_claimed"`, `Title = "Your dispute is being reviewed"`, `Message = "A teacher has picked up your grade dispute and will respond shortly."`
- Call `AuditHelper.LogAsync` with action `"Claim"`
- Redirect to `Details` of the same dispute

#### Edit (Resolve form)
- **GET:** Load dispute + original submission (same as Details logic). The form shows:
  - Read-only: Student name, submission type, original AI score, student's reason
  - Editable: `TeacherNotes` (textarea, required), `RevisedScore` (optional float 0–9, 0.5 step)
  - Two submit buttons: "Resolve (Uphold)" and "Resolve (Override Score)"
- **POST — Resolve:**
  - Validate `TeacherNotes` is not empty (return form with model error if missing)
  - Set `Status = "resolved"`, `ResolvedAt = DateTime.UtcNow`
  - If `RevisedScore` is provided:
    - If `SubmissionType == "writing"`: update `tb_writing_submissions.band_score` where `submission_id == dispute.SubmissionId`
    - If `SubmissionType == "speaking"`: update `tb_speaking_sessions.band_score` where `session_id == dispute.SubmissionId`
  - Create notification for student:
    - `Type = "dispute_resolved"`, `Title = "Grade dispute resolved"`
    - `Message` = include TeacherNotes and revised score if applicable
  - Call `AuditHelper.LogAsync` with action `"Resolve"`
  - Redirect to Index (Resolved tab)
- **POST — Reject:**
  - Validate `TeacherNotes` is not empty
  - Set `Status = "rejected"`, `ResolvedAt = DateTime.UtcNow`
  - Create notification for student: `Type = "dispute_rejected"`, include TeacherNotes in message
  - Call `AuditHelper.LogAsync` with action `"Reject"`
  - Redirect to Index (Rejected tab)

#### Edit.cshtml — UI Requirements (Bootstrap 5)
- Two-column card layout
- **Left card:** "Submission Being Disputed"
  - Student name + avatar initials
  - Submission type badge (Writing / Speaking)
  - Large display of original AI band score (styled prominently, e.g. `display-4`)
  - For Writing: TA / CC / LR / GA sub-scores in a small grid
  - For Speaking: Fluency / Pronunciation / Grammar sub-scores
  - Full essay text or transcript in a scrollable `<pre>` block
  - Original AI feedback
- **Right card:** "Review & Resolution"
  - Student's dispute reason in a highlighted blockquote
  - `TeacherNotes` textarea (required, rows=5, placeholder)
  - `RevisedScore` number input (min=0, max=9, step=0.5, optional — leave blank to uphold)
  - Helper text: "Leave blank to uphold the original score"
  - Two buttons: `btn-success` "Resolve" and `btn-danger` "Reject"
  - Both submit to same form, use different `name="action"` value attributes

---

### Task 2.3 — ConversationsController.cs

**Actions: Index, Details, Edit (Reply)**

#### Index
- Load `tb_conversations`, include `Student.Profile`
- Two sections on the same page:
  - **Open (Unassigned):** `status = "open"` — all teachers see these (open pool)
  - **My Active:** `status = "active"` AND `teacher_id = currentUserId`
- Show: Student name, subject, created date, last message date, status badge, action buttons

#### Details
- Load conversation + all messages ordered by `created_at` ASC
- Include `Sender.Profile` for each message
- Display as a chat-style thread (alternating left/right based on sender role)
- Show a reply form at the bottom (textarea + Send button)
- If `Status == "open"`: show banner "You are the first to reply. Replying will assign this conversation to you."

#### Edit / Reply (POST action — triggered from Details form)
- Sanitize message content with `InputSanitizer.StripHtmlTags()`
- Add a `Message` row to `tb_messages`: `ConversationId`, `SenderId = currentUserId`, `Content`, `IsRead = false`, `CreatedAt = UtcNow`
- Update `tb_conversations.LastMessageAt = UtcNow`
- **Auto-assign:** If `conversation.TeacherId == null` (first reply):
  - Set `conversation.TeacherId = currentUserId`
  - Set `conversation.Status = "active"`
  - Create notification for student: `Type = "new_message"`, `Title = "A teacher has replied"`, `Message = "A teacher has joined your conversation and sent a reply."`
- If already active: create notification for student: `Type = "new_message"`, `Title = "New message from your teacher"`
- Call `AuditHelper.LogAsync` with action `"Reply"`
- Redirect back to `Details` of the same conversation

#### Close (POST action)
- Set `Status = "closed"`
- Call `AuditHelper.LogAsync` with action `"Close"`
- Redirect to Index

---

## SECTION 10 — NOTIFICATION TYPE REFERENCE

Use these exact `type` string values for `tb_notifications`:

| Type | Triggered by |
|---|---|
| `dispute_claimed` | Teacher claims a pending dispute |
| `dispute_resolved` | Teacher resolves a dispute |
| `dispute_rejected` | Teacher rejects a dispute |
| `new_message` | Teacher replies to a conversation |
| `new_conversation` | Student opens a new conversation (notifies teachers) |
| `content_approved` | Teacher's content approved by a peer |
| `content_rejected` | Teacher's content rejected by a peer |

---

## SECTION 11 — DOUBLE-CHECK BEFORE GENERATING CODE

Before writing any controller or view, verify these:

**Pre-flight:**
- [ ] Both skills installed and fully read (frontend-design + ui-ux-pro-max)
- [ ] Template folder `~/WebIeltsFree/Template_IELTS/Admin_Teacher/` fully scanned and all CSS classes extracted
- [ ] All root controllers in `~/WebIeltsFree/Controllers/` scanned and duplicate action names documented in Section 13
- [ ] Section 13 filled in before any code is written

**Auth & routing:**
- [ ] All `[Authorize(Roles = "...")]` use **lowercase** role strings
- [ ] `[Area("Admin")]` on every Admin controller, `[Area("Teacher")]` on every Teacher controller
- [ ] `_ViewImports.cshtml` and `_ViewStart.cshtml` generated for both areas
- [ ] `app.MapControllerRoute("areas", ...)` included in `Program.cs` notes
- [ ] No `[Authorize(Roles = "Admin")]` (capital A) anywhere

**Data safety:**
- [ ] No `_context.WritingPrompts.Remove()` or `_context.SpeakingTopics.Remove()` anywhere
- [ ] Dispute resolve: branches on `SubmissionType == "writing"` vs `"speaking"` for score sync
- [ ] `TeacherNotes` validated as non-empty on resolve AND reject
- [ ] `AuditHelper.LogAsync()` called in EVERY action that mutates data
- [ ] `InputSanitizer.StripHtmlTags()` called on every user text input before saving
- [ ] Self-approval block in WritingPrompts and SpeakingTopics Approve action
- [ ] Self-edit/delete block in UsersController
- [ ] Pagination in UsersController Index (10 per page)
- [ ] `GetCurrentUserId()` helper defined in each controller
- [ ] Soft-delete in Teacher area uses `Status = "rejected"` NOT `.Remove()`
- [ ] `DeleteBehavior.Restrict` already configured in AppDbContext — do not add cascade deletes

**UI & template:**
- [ ] Every view uses the template's layout and CSS class names (not default Bootstrap fallbacks)
- [ ] frontend-design skill aesthetic applied to typography, color refinement, micro-interactions
- [ ] ui-ux-pro-max skill applied for component quality and visual polish
- [ ] TempData["Success"] and TempData["Warning"] use the template's alert/toast pattern
- [ ] All Index views use the template's table row and action button pattern

---

## SECTION 12 — OUTPUT FORMAT

Generate files in this exact order:

1. `Areas/Admin/Views/_ViewImports.cshtml`
2. `Areas/Admin/Views/_ViewStart.cshtml`
3. `Areas/Teacher/Views/_ViewImports.cshtml`
4. `Areas/Teacher/Views/_ViewStart.cshtml`
5. `Areas/Admin/Controllers/UsersController.cs`
6. `Areas/Teacher/Controllers/GradeDisputesController.cs`
7. `Areas/Teacher/Controllers/WritingPromptsController.cs`
8. `Areas/Teacher/Controllers/SpeakingTopicsController.cs`
9. `Areas/Teacher/Controllers/ConversationsController.cs`
10. `Areas/Admin/Views/Users/Index.cshtml`
11. `Areas/Teacher/Views/GradeDisputes/Index.cshtml`
12. `Areas/Teacher/Views/GradeDisputes/Details.cshtml`
13. `Areas/Teacher/Views/GradeDisputes/Edit.cshtml` ← Full Bootstrap 5 two-column card layout
14. `Areas/Teacher/Views/WritingPrompts/Index.cshtml`
15. `Areas/Teacher/Views/SpeakingTopics/Index.cshtml`
16. `Areas/Teacher/Views/Conversations/Index.cshtml`
17. `Areas/Teacher/Views/Conversations/Details.cshtml`

For each file, begin with a comment line: `// Path: ~/Areas/...` or `@* Path: ~/Areas/... *@`

Each file must be complete and compilable. Do not use placeholder comments like `// TODO` or `// rest of implementation here`.