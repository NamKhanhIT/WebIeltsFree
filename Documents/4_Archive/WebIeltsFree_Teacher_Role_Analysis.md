# WebIeltsFree — Teacher / Instructor Role Analysis

> **Append this document to WebIeltsFree_AI_Context.md before starting any teacher-related implementation.**

---

## DECISIONS SUMMARY

| Question | Answer |
|---|---|
| Role name | Teacher / Instructor |
| Primary job | All equally — grade review + user support + content management |
| Content access | Full — create & edit lessons, tests, prompts, topics |
| Communication | Both — ticket system (disputes) + direct messaging (support) |
| Assignment | Open pool — any teacher can handle any user |

---

## 1. ROLE DEFINITION

The **Teacher / Instructor** is a trusted academic role between students and admins. They own the pedagogical layer of the platform.

**Three core responsibilities:**

**Grade Reviewer** — Review Writing & Speaking submissions when students dispute AI scores. Override AI band scores with human judgment and written rationale. Resolve or reject dispute tickets with feedback to the student.

**Learning Coach** — Directly message any student to offer guidance. View any student's progress, scores, and practice history. Proactively reach out to struggling learners.

**Content Manager** — Create and edit Courses, Modules, Lessons, Lesson Contents. Create and manage Writing Prompts, Speaking Topics, Reading Passages, Listening Materials with questions.

> Teachers are NOT system admins. They cannot ban users, change roles, access audit logs, or modify system settings. Their authority is purely academic.

---

## 2. PERMISSION MATRIX

| Feature | Student | Teacher | Admin |
|---|---|---|---|
| **Auth & Profile** | | | |
| Register / Login | ✓ | ✓ | ✓ |
| Edit own profile | ✓ | ✓ | ✓ |
| View any student's profile & scores | — | ✓ | ✓ |
| Ban / unban users | — | — | ✓ |
| Change user roles | — | — | ✓ |
| **Learning & Practice** | | | |
| Take courses, lessons, tests | ✓ | ✓ | ✓ |
| Practice Reading & Listening | ✓ | ✓ | ✓ |
| Submit Writing essays | ✓ | ✓ | ✓ |
| Do Speaking sessions | ✓ | ✓ | ✓ |
| **Content Management** | | | |
| Create / edit Courses, Modules, Lessons | — | ✓ | ✓ |
| Create / edit Writing Prompts | — | ✓ | ✓ |
| Create / edit Speaking Topics & Parts | — | ✓ | ✓ |
| Create / edit Reading Passages & Questions | — | ✓ | ✓ |
| Create / edit Listening Materials & Questions | — | ✓ | ✓ |
| Delete content permanently | — | SOFT only | ✓ |
| **Grade Disputes** | | | |
| Submit a grade dispute ticket | ✓ | — | — |
| View all open dispute tickets | — | ✓ | ✓ |
| Pick up & review a dispute | — | ✓ | ✓ |
| Override AI band score | — | ✓ | ✓ |
| Resolve / reject a dispute | — | ✓ | ✓ |
| **Communication** | | | |
| Direct message a teacher | ✓ | — | — |
| Direct message any student | — | ✓ | — |
| **Analytics & System** | | | |
| View own activity & achievements | ✓ | ✓ | ✓ |
| View student progress dashboard | — | ✓ | ✓ |
| View platform-wide analytics | — | — | ✓ |
| Access audit logs | — | — | ✓ |
| Modify system settings | — | — | ✓ |

---

## 3. BUSINESS FLOWS — BEFORE vs AFTER

### Flow 1 — Student Disputes AI Grade

**BEFORE (broken):**
1. Student receives AI Writing/Speaking score
2. Student feels score is unfair — has NO recourse
3. Submits via `tb_reports` (bug reports table — wrong tool)
4. Admin has no pedagogical expertise to evaluate it
5. Report sits unresolved. Student trust in platform drops.

**AFTER (with Teacher):**
1. Student receives AI score, sees "Dispute this score" button
2. Student submits dispute `POST api/disputes/submit` with reason
3. Ticket created in `tb_grade_disputes` — status: **pending**
4. Any teacher sees ticket in dashboard, claims it → status: **under_review**
5. Teacher reviews full original submission + AI feedback + all scores
6. Teacher resolves: **uphold** (explain why) OR **override** (update score + write rationale)
7. Student notified via `tb_notifications` with teacher's full feedback

### Flow 2 — Student Needs Study Support

**BEFORE:** Only the AI chatbot. No human available. Student feels isolated. May abandon platform.

**AFTER:**
1. Student starts conversation `POST api/messages/conversations/start`
2. Thread appears in open pool — any teacher can respond
3. Teacher sees student's full profile + scores + history before replying
4. Async back-and-forth via `tb_messages`
5. Teachers can also proactively message struggling students

### Flow 3 — Content Creation

**BEFORE:** Admin is sole content creator AND system operator — two completely different jobs. Content updates are slow. Admin is the bottleneck.

**AFTER:**
1. Teachers own content — they create, edit, and improve lessons independently
2. New speaking topics via `POST api/speaking/topics` by any teacher
3. New writing prompts via `POST api/writing/prompts` by any teacher
4. Admin only reviews flagged/deleted content — no longer the creator

### Flow 4 — Teacher Account Creation

1. Teacher registers via normal `POST api/auth/register` (role defaults to student)
2. Admin promotes user to teacher via `PATCH api/admin/users/{id}/role`
3. Teacher receives notification — dashboard changes to show teacher features
4. Teacher can now access: disputes queue, student list, content editor

---

## 4. ADMIN WORKLOAD RELIEF

### Admin Loses (~60–70% reduction in daily tasks)
- Reviewing AI grade disputes from students
- Answering academic / pedagogical questions
- Creating courses, modules, and lesson content
- Creating and updating writing prompts, speaking topics
- Creating reading passages and listening materials
- Monitoring individual student progress academically
- Being first point of contact for study support

### Admin Keeps (purely technical/managerial)
- User account management (ban, unban, delete)
- Promoting users to Teacher or other roles
- Platform-wide analytics and health monitoring
- System settings and configuration
- Audit logs and security monitoring
- Final escalation for unresolved disputes
- Restoring soft-deleted content

---

## 5. DATABASE CHANGES

### Modified Tables (1)

```sql
-- Add 'teacher' to tb_users role ENUM
ALTER TABLE tb_users
  MODIFY COLUMN role ENUM('student', 'teacher', 'admin', 'moderator')
  NOT NULL DEFAULT 'student';
```

### New Tables (3) — Total tables: 41

```sql
-- Grade dispute tickets
CREATE TABLE tb_grade_disputes (
  dispute_id       INT AUTO_INCREMENT PRIMARY KEY,
  user_id          INT NOT NULL,                    -- student who submitted
  submission_type  ENUM('writing', 'speaking') NOT NULL,
  submission_id    INT NOT NULL,                    -- FK to tb_writing_submissions or tb_speaking_sessions
  reason           TEXT NOT NULL,                   -- student's explanation
  status           ENUM('pending','under_review','resolved','rejected') NOT NULL DEFAULT 'pending',
  reviewed_by      INT NULL,                        -- teacher_id, NULL until claimed
  original_score   FLOAT NOT NULL,                  -- AI score at time of dispute
  revised_score    FLOAT NULL,                      -- teacher override, NULL if upheld
  teacher_notes    TEXT NULL,                       -- resolution feedback to student (REQUIRED on resolve)
  created_at       DATETIME DEFAULT CURRENT_TIMESTAMP,
  resolved_at      DATETIME NULL,
  CONSTRAINT fk_dispute_user    FOREIGN KEY (user_id)    REFERENCES tb_users(user_id),
  CONSTRAINT fk_dispute_teacher FOREIGN KEY (reviewed_by) REFERENCES tb_users(user_id)
);

-- Message threads
CREATE TABLE tb_conversations (
  conversation_id  INT AUTO_INCREMENT PRIMARY KEY,
  student_id       INT NOT NULL,
  teacher_id       INT NULL,                        -- NULL until a teacher picks up
  subject          VARCHAR(255) NULL,
  status           ENUM('open','active','closed') NOT NULL DEFAULT 'open',
  created_at       DATETIME DEFAULT CURRENT_TIMESTAMP,
  last_message_at  DATETIME NULL,
  CONSTRAINT fk_conv_student FOREIGN KEY (student_id) REFERENCES tb_users(user_id),
  CONSTRAINT fk_conv_teacher FOREIGN KEY (teacher_id) REFERENCES tb_users(user_id)
);

-- Individual messages
CREATE TABLE tb_messages (
  message_id       INT AUTO_INCREMENT PRIMARY KEY,
  conversation_id  INT NOT NULL,
  sender_id        INT NOT NULL,
  content          TEXT NOT NULL,
  is_read          TINYINT(1) NOT NULL DEFAULT 0,
  created_at       DATETIME DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_msg_conv   FOREIGN KEY (conversation_id) REFERENCES tb_conversations(conversation_id),
  CONSTRAINT fk_msg_sender FOREIGN KEY (sender_id)       REFERENCES tb_users(user_id)
);
```

### New EF Entities (add to Entities.cs)

```csharp
[Table("tb_grade_disputes")]
public class GradeDispute
{
    [Key][Column("dispute_id")]     public int DisputeId { get; set; }
    [Column("user_id")]             public int UserId { get; set; }
    [Column("submission_type")]     public string SubmissionType { get; set; } = "";
    [Column("submission_id")]       public int SubmissionId { get; set; }
    [Column("reason")]              public string Reason { get; set; } = "";
    [Column("status")]              public string Status { get; set; } = "pending";
    [Column("reviewed_by")]         public int? ReviewedBy { get; set; }
    [Column("original_score")]      public float OriginalScore { get; set; }
    [Column("revised_score")]       public float? RevisedScore { get; set; }
    [Column("teacher_notes")]       public string? TeacherNotes { get; set; }
    [Column("created_at")]          public DateTime CreatedAt { get; set; }
    [Column("resolved_at")]         public DateTime? ResolvedAt { get; set; }
}

[Table("tb_conversations")]
public class Conversation
{
    [Key][Column("conversation_id")] public int ConversationId { get; set; }
    [Column("student_id")]           public int StudentId { get; set; }
    [Column("teacher_id")]           public int? TeacherId { get; set; }
    [Column("subject")]              public string? Subject { get; set; }
    [Column("status")]               public string Status { get; set; } = "open";
    [Column("created_at")]           public DateTime CreatedAt { get; set; }
    [Column("last_message_at")]      public DateTime? LastMessageAt { get; set; }
}

[Table("tb_messages")]
public class Message
{
    [Key][Column("message_id")]      public int MessageId { get; set; }
    [Column("conversation_id")]      public int ConversationId { get; set; }
    [Column("sender_id")]            public int SenderId { get; set; }
    [Column("content")]              public string Content { get; set; } = "";
    [Column("is_read")]              public bool IsRead { get; set; }
    [Column("created_at")]           public DateTime CreatedAt { get; set; }
}
```

---

## 6. CODEBASE CHANGES

### New Controllers

| Controller | Route | Auth | Key Endpoints |
|---|---|---|---|
| `DisputesController` | `api/disputes` | Student (submit) / Teacher+Admin (review) | POST submit, GET list, PATCH {id}/claim, PATCH {id}/resolve, PATCH {id}/reject |
| `MessagesController` | `api/messages` | Student + Teacher | POST conversations/start, GET conversations, GET conversations/{id}/messages, POST conversations/{id}/send, PATCH conversations/{id}/close |
| `TeacherController` | `api/teacher` | Teacher + Admin only | GET dashboard, GET students, GET students/{id}/detail |

### Updated Controllers

| Controller | What Changes |
|---|---|
| `WritingController` | Add `POST api/writing/submissions/{id}/dispute` (student). Add `POST/PUT/DELETE api/writing/prompts` (teacher). Score override on dispute resolve. |
| `SpeakingController` | Add `POST api/speaking/sessions/{id}/dispute`. Add `POST/PUT/DELETE api/speaking/topics` and `/parts` (teacher). |
| `LessonsController` | Add `[Authorize(Roles="teacher,admin")]` to all POST/PUT/DELETE endpoints |
| `ReadingController` | Add teacher-authorized `POST/PUT/DELETE api/reading/passages` and `/questions` |
| `ListeningController` | Add teacher-authorized `POST/PUT/DELETE api/listening/materials` and `/questions` |
| `UsersController` | Add `GET api/users/{id}/detail` (teacher scope). Add `PATCH api/admin/users/{id}/role` (admin only). |

### Program.cs Addition

```csharp
// Add role-based authorization policy
builder.Services.AddAuthorization(options => {
    options.AddPolicy("TeacherOrAdmin", policy =>
        policy.RequireRole("teacher", "admin"));
});
```

### Files Impacted Summary
- **New**: `DisputesController.cs`, `MessagesController.cs`, `TeacherController.cs`
- **Modified**: `Entities.cs`, `AppDbContext.cs`, `Program.cs`, `WritingController.cs`, `SpeakingController.cs`, `LessonsController.cs`, `ReadingController.cs`, `ListeningController.cs`, `UsersController.cs`
- **SQL**: Add 3 new CREATE TABLE scripts + 1 ALTER to `DatabaseScripts/`

---

## 7. NEW HARD RULES (Append to Context Doc)

10. NEVER let a student access teacher endpoints — always enforce role check
11. NEVER let a teacher resolve a dispute without `TeacherNotes` — it's required, return 400 if missing
12. NEVER let a teacher access admin-only routes (audit logs, system settings, user bans)
13. NEVER delete `tb_grade_disputes` rows — status changes only (pending → resolved/rejected)
14. NEVER expose one student's data to another student — `GET /users/{id}/detail` is teacher-only
15. NEVER hard-delete content as teacher — soft-delete only (set status='archived' or is_deleted=true)

---

## 8. NEW AUTH PATTERNS

```csharp
// Teacher-or-admin endpoint
[Authorize(Roles = "teacher,admin")]

// Student-only endpoint (e.g. submitting a dispute)
[Authorize(Roles = "student")]

// Check role in controller body
var role = User.FindFirst(ClaimTypes.Role)?.Value;
bool isTeacher = role == "teacher" || role == "admin";

// Teacher viewing student detail — guard in controller
if (role != "teacher" && role != "admin")
    return Forbid();
```

---

## 9. IMPLEMENTATION TASK LIST

Paste context doc + this section for each task. Do them in order.

### TASK T-1 — Add Teacher role to DB + Auth (LOW RISK)
```
Using the WebIeltsFree project context provided:

Add the Teacher role to the authentication and authorization system.

1. Add SQL to DatabaseScripts/: ALTER TABLE tb_users MODIFY COLUMN role ENUM('student','teacher','admin','moderator') NOT NULL DEFAULT 'student';
2. Add authorization policy in Program.cs: AddPolicy("TeacherOrAdmin", r => r.RequireRole("teacher","admin"))
3. Add PATCH api/admin/users/{id}/role endpoint in UsersController (admin only) to update a user's role
4. Verify JWT token generation in AuthController includes the 'role' claim — if not, add it
5. Do NOT use EF migrations. Add SQL to DatabaseScripts/ only.
6. Keep ApiResponse<T> wrapper on all responses.
```

### TASK T-2 — Grade Dispute System (MEDIUM RISK)
```
Using the WebIeltsFree project context + Teacher Role Analysis provided:

Implement the Grade Dispute system so students can dispute AI scores and teachers can resolve them.

Database:
- Add CREATE TABLE tb_grade_disputes SQL to DatabaseScripts/ (see Teacher Role Analysis doc for exact schema)
- Add GradeDispute entity to Entities.cs with all [Table][Column] attributes
- Add DbSet<GradeDispute> to AppDbContext.cs + configure FK relationships in OnModelCreating

API — Create DisputesController (api/disputes):
- POST api/disputes/submit [Authorize(Roles="student")] — create dispute with reason + submission_type + submission_id
- GET api/disputes [Authorize(Roles="teacher,admin")] — list all pending/under_review disputes
- GET api/disputes/{id} [Authorize(Roles="teacher,admin")] — dispute detail
- PATCH api/disputes/{id}/claim [Authorize(Roles="teacher,admin")] — set reviewed_by = current teacher, status = 'under_review'
- PATCH api/disputes/{id}/resolve [Authorize(Roles="teacher,admin")] — requires TeacherNotes; optionally set RevisedScore; status = 'resolved'; update original submission's band_score if revised
- PATCH api/disputes/{id}/reject [Authorize(Roles="teacher,admin")] — requires TeacherNotes; status = 'rejected'

Also add:
- POST api/writing/submissions/{id}/dispute shortcut in WritingController
- POST api/speaking/sessions/{id}/dispute shortcut in SpeakingController

Business rules:
- TeacherNotes is required on resolve/reject — return 400 if missing
- On resolve with revised score: update band_score in tb_writing_submissions or tb_speaking_sessions
- Never delete dispute rows — status changes only
- Keep ApiResponse<T> wrapper on all endpoints
```

### TASK T-3 — Messaging System (MEDIUM RISK)
```
Using the WebIeltsFree project context + Teacher Role Analysis provided:

Implement async direct messaging between students and teachers (open pool model).

Database:
- Add CREATE TABLE tb_conversations and CREATE TABLE tb_messages to DatabaseScripts/
- Add Conversation and Message entities to Entities.cs with [Table][Column] attributes
- Add DbSets to AppDbContext.cs + OnModelCreating relationships

API — Create MessagesController (api/messages):
- POST api/messages/conversations/start [Authorize(Roles="student")] — student creates new conversation with optional subject
- GET api/messages/conversations [Authorize] — students see own conversations; teachers see all open+active conversations
- GET api/messages/conversations/{id}/messages [Authorize] — messages in a thread (enforce ownership for students)
- POST api/messages/conversations/{id}/send [Authorize] — send message; update last_message_at; if sender is teacher and teacher_id is null, set it now and status = 'active'
- PATCH api/messages/conversations/{id}/mark-read [Authorize] — mark messages as read
- PATCH api/messages/conversations/{id}/close [Authorize(Roles="teacher,admin")] — set status = 'closed'

Business rules:
- Students can only see their own conversations and messages
- Teachers can see all conversations in the open pool
- Sanitize message content with InputSanitizer.StripHtmlTags()
- Keep ApiResponse<T> wrapper on all endpoints
```

### TASK T-4 — Teacher Dashboard API (LOW RISK)
```
Using the WebIeltsFree project context + Teacher Role Analysis provided:

Create the TeacherController to power the teacher's overview dashboard.

Create TeacherController (api/teacher) with [Authorize(Roles="teacher,admin")]:

- GET api/teacher/dashboard — returns:
  { pendingDisputes: int, openConversations: int, totalStudents: int, resolvedDisputesThisWeek: int }

- GET api/teacher/students — paginated list of all users with role='student':
  { userId, fullName, email, currentBand, targetBand, lastActiveAt, totalPracticeAttempts }

- GET api/teacher/students/{id}/detail — full academic profile for one student:
  { profile, goals, latestWritingSubmissions (last 5), latestSpeakingSessions (last 5),
    testAttempts (last 5), practiceAttempts summary, roadmap, activeDisputes }

Business rules:
- This endpoint returns another user's data — only teacher and admin roles allowed
- Keep ApiResponse<T> wrapper
- Use existing AppDbContext — no new tables needed
```

### TASK T-5 — Teacher Content CRUD Permissions (MEDIUM RISK)
```
Using the WebIeltsFree project context + Teacher Role Analysis provided:

Add content creation and management permissions to the Teacher role across all content controllers.

LessonsController:
- Add [Authorize(Roles="teacher,admin")] to all POST/PUT/DELETE endpoints for courses, modules, lessons, lesson_contents

WritingController:
- Add POST api/writing/prompts (create writing prompt)
- Add PUT api/writing/prompts/{id} (update)
- Add DELETE api/writing/prompts/{id} (soft-delete: set is_active=false or status='archived')

SpeakingController:
- Add POST/PUT/DELETE api/speaking/topics
- Add POST/PUT/DELETE api/speaking/topics/{id}/parts

ReadingController:
- Add POST/PUT/DELETE api/reading/passages
- Add POST/PUT/DELETE api/reading/passages/{id}/questions

ListeningController:
- Add POST/PUT/DELETE api/listening/materials
- Add POST/PUT/DELETE api/listening/materials/{id}/questions

For all DELETE endpoints: soft-delete only for teachers (set a deleted/archived flag). Hard-delete admin only.
Write all teacher content changes to tb_audit_logs.
Keep ApiResponse<T> wrapper on all endpoints.
```

### TASK T-6 — Dispute & Message Notifications (LOW RISK)
```
Using the WebIeltsFree project context + Teacher Role Analysis provided:

Wire notifications to the dispute and messaging systems so users are kept informed.

Disputes:
- When teacher claims dispute (status→under_review): notify student "Your grade dispute is being reviewed by a teacher"
- When teacher resolves (status→resolved): notify student with teacher_notes + new score if changed
- When teacher rejects (status→rejected): notify student with teacher_notes

Messaging:
- When student starts new conversation: create a notification for users with role='teacher' (broadcast type)
- When teacher replies to student: notify student that they have a new message

Use existing tb_notifications table and the pattern already used in NotificationsController.
Notification types to add: 'dispute_claimed', 'dispute_resolved', 'dispute_rejected', 'new_message', 'new_conversation'
Keep ApiResponse<T> wrapper on all responses.
```

---

*WebIeltsFree — Teacher Role Analysis · 41 total tables after implementation · 3 new controllers · 5 updated controllers*
