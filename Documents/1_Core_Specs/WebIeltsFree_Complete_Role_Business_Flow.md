# WebIeltsFree — Complete Role Functions & Business Flows

> **Purpose:** This document is the single source of truth for every role's capabilities, business rules, and UI flows. Use it as context when prompting AI to build dashboard pages.

---

## Platform Overview

WebIeltsFree is an AI-powered IELTS learning platform (ASP.NET Core 8 MVC + API). It has **41 MySQL tables**, MongoDB for logs/AI data, Pinecone for vector search, and Redis for caching.

**Roles stored in `tb_users.role` (lowercase strings):**
- `student` — Learner (default on registration)
- `teacher` — Academic authority (human-in-the-loop)
- `admin` — System operator
- `moderator` — Reserved for future use

**User statuses in `tb_users.status`:** `active` | `banned` | `inactive`

---

# ROLE 1: STUDENT

## 1.1 Authentication & Onboarding

| Step | Endpoint / Action | What Happens | DB Tables |
|---|---|---|---|
| Register | `POST /api/auth/register` | Creates `tb_users` (role=student), `tb_user_profiles`, `tb_user_goals`. Sends verification email. Returns JWT. | `tb_users`, `tb_user_profiles`, `tb_user_goals`, `tb_user_sessions` |
| Verify Email | `GET /api/auth/verify-email?token=` | Sets `email_verified = true`. Revokes verification session. | `tb_users`, `tb_user_sessions` |
| Login | `POST /api/auth/login` | Validates BCrypt password. Checks email verification & ban status. Returns JWT + refresh token. Increments `login_count`, sets `last_login_at`. | `tb_users`, `tb_user_sessions` |
| Token Refresh | `POST /api/auth/refresh` | Rotates refresh token (token family rotation). Detects reuse → revokes entire family. | `tb_user_sessions` |
| Forgot/Reset Password | `POST /api/auth/forgot-password` / `reset-password` | Generates reset token (24h expiry), emails link. On reset: hashes new password, nullifies token. | `tb_users` |
| Onboarding Check | `GET /api/users/onboarding-status` | Returns: `HasPlacement`, `HasGoals`, `HasRoadmap`, `HasCompletedTutorial`. | `tb_user_goals`, `tb_ai_roadmaps`, `tb_user_test_attempts` |
| Complete Tutorial | `POST /api/users/complete-tutorial` | Sets `has_completed_onboarding = true`. | `tb_users` |

### Onboarding Sequence (after first login):
```
1. Placement Test → saves result to tb_user_placement_results
2. Set Goals → target_band (0-9), exam_date, study_hours_per_day → tb_user_goals
3. AI Roadmap Generation → generates personalized study plan → tb_ai_roadmaps + tb_ai_roadmap_steps
4. Tutorial Completion → marks onboarding done
```

## 1.2 Profile & Goals Management

| Action | Endpoint | Editable Fields | Table |
|---|---|---|---|
| View Profile | `GET /api/users/profile` | — | `tb_users` + `tb_user_profiles` + `tb_user_goals` |
| Update Profile | `PUT /api/users/profile` | `FullName`, `AvatarUrl`, `Country`, `Timezone`, `PreferredLanguage` | `tb_user_profiles` |
| View Goals | `GET /api/users/goals` | — | `tb_user_goals` |
| Set Goals | `POST /api/users/goals` | `TargetBand` (0-9), `ExamDate`, `StudyHoursPerDay`, `LearningReason` | `tb_user_goals` |
| View Progress | `GET /api/users/progress` | — | Aggregates from `tb_user_learning_progress`, `tb_user_test_attempts`, `tb_ai_skill_analysis` |

## 1.3 Learning & Practice

### Reading Practice
| Action | Endpoint | Logic | Tables |
|---|---|---|---|
| Get Passages | `GET /api/reading/passages?difficulty=band_5_6` | Returns random passage + questions (answers hidden) | `tb_reading_passages`, `tb_reading_questions` |
| Submit Answer | `POST /api/reading/submit` | Validates by question_type (MC, TFNG, fill_in_blank, matching_heading, sentence/summary_completion). Case-insensitive, trimmed. | `tb_user_practice_attempts` |

### Listening Practice
| Action | Endpoint | Logic | Tables |
|---|---|---|---|
| Get Materials | `GET /api/listening/materials?difficulty=band_5_6` | Returns audio material + questions | `tb_listening_materials`, `tb_listening_questions` |
| Submit Answer | `POST /api/listening/submit` | Same validation types as Reading | `tb_user_practice_attempts` |

### Writing Practice (AI-Graded)
| Action | Endpoint | Logic | Tables |
|---|---|---|---|
| Get Prompts | `GET /api/writing/prompts?taskType=task2` | Returns published prompts only (`status = "published"` or `is_active = true`) | `tb_writing_prompts` |
| Submit Essay | `POST /api/writing/submit` | AI (Gemini/Python) evaluates: Task Achievement, Coherence & Cohesion, Lexical Resource, Grammar. Returns band_score + 4 sub-scores + ai_feedback. | `tb_writing_submissions` |

### Speaking Practice (AI Examiner)
| Action | Endpoint | Logic | Tables |
|---|---|---|---|
| Get Topics | `GET /api/speaking/topics` | Returns published topics (Part 1/2/3) | `tb_speaking_topics`, `tb_speaking_topic_parts` |
| Submit Audio | `POST /api/speaking/submit-audio` | Python AI evaluates: Fluency, Pronunciation, Grammar. Returns sub-scores + overall band + ai_feedback. Fallback mock if AI fails. | `tb_speaking_sessions` |

### Tests
| Action | Endpoint | Tables |
|---|---|---|
| List Tests | `GET /api/tests` | `tb_tests`, `tb_test_sections` |
| Start Attempt | `POST /api/tests/{id}/start` | `tb_user_test_attempts` |
| Submit Answers | `POST /api/tests/{id}/submit` | `tb_user_answers` → auto-scored |
| View Results | `GET /api/tests/attempts/{id}` | `tb_user_test_attempts` + `tb_user_answers` |

## 1.4 Grade Dispute (Student Side)

```
FLOW:
Student sees AI score they disagree with (Writing or Speaking)
  → POST /api/disputes/submit { submissionType, submissionId, reason }
  → System validates: submission exists, belongs to student, no duplicate open dispute
  → Creates tb_grade_disputes (status="pending", original_score captured)
  → Student waits for teacher action
  → Student receives notifications: dispute_claimed → dispute_resolved OR dispute_rejected
  → If resolved with RevisedScore: original submission's band_score is updated
```

| Rule | Detail |
|---|---|
| Duplicate check | Only one open dispute per submission (`status = pending OR under_review`) |
| Submission ownership | Student can only dispute their own submissions |
| Score capture | `original_score` is copied from the submission at dispute creation time |

## 1.5 Messaging (Student Side)

```
FLOW:
Student needs help
  → POST /api/messages/conversations/start { subject, message }
  → Creates tb_conversations (status="open", teacher_id=null)
  → Adds initial message to tb_messages
  → All teachers receive "new_conversation" notification
  → Student waits for teacher reply
  → When teacher replies: teacher auto-assigned, status→"active"
  → Async messaging continues
  → Student can mark messages as read: PATCH /conversations/{id}/mark-read
  → Teacher or Admin closes conversation → status="closed"
```

| Rule | Detail |
|---|---|
| Data isolation | Students can ONLY see their own conversations |
| Closed conversations | Students cannot send messages to closed conversations |

## 1.6 Notifications, Achievements, Activity

| Feature | Table | Details |
|---|---|---|
| Notifications | `tb_notifications` | Types: `dispute_claimed`, `dispute_resolved`, `dispute_rejected`, `new_message`, `content_approved`, `content_rejected` |
| Achievements | `tb_gamification` + `tb_user_achievements` | Badge system with codes, points, criteria (JSON) |
| Daily Activity | `tb_user_daily_activity` | Tracks: lessons_completed, questions_answered, writing_submissions, speaking_sessions, minutes_spent, xp_earned, streak_day |

---

# ROLE 2: TEACHER

> **Authorization:** `[Authorize(Roles = "teacher,admin")]` — Admins can do everything teachers can.

## 2.1 Teacher Dashboard API

| Endpoint | Returns | Source |
|---|---|---|
| `GET /api/teacher/dashboard` | `PendingDisputes`, `OpenConversations`, `TotalStudents`, `ResolvedDisputesThisWeek` | Aggregates from `tb_grade_disputes`, `tb_conversations`, `tb_users` |
| `GET /api/teacher/students?page=1&size=20` | Paginated student list: name, email, current/target band, last active, practice count | `tb_users` + `tb_user_profiles` + `tb_user_goals` |
| `GET /api/teacher/students/{id}/detail` | Full academic profile: latest 5 writing submissions, latest 5 speaking sessions, test/practice counts, active disputes | Multiple tables |

## 2.2 Grade Dispute Workflow (Teacher Side — Dashboard MVC Area)

```
FLOW:
1. Teacher opens Grade Disputes page → sees tabbed view:
   [Pending] [Under Review] [Resolved] [Rejected] — each tab has badge count

2. PENDING tab: Teacher sees list of unclaimed disputes
   → Clicks "Claim" → POST Claim action
   → Status: pending → under_review
   → ReviewedBy = current teacher
   → Student notified: "dispute_claimed"
   → Redirect to Details page

3. DETAILS page: Two-column layout
   LEFT: Original submission (essay/transcript, AI scores, sub-scores, AI feedback)
   RIGHT: Student's dispute reason + resolution form

4. RESOLVE/REJECT form (Edit page):
   → TeacherNotes (REQUIRED — reject form if empty)
   → RevisedScore (optional, 0-9, step 0.5)
   → Two buttons: "Resolve" or "Reject"

5. RESOLVE action:
   → Status → "resolved", ResolvedAt = now
   → If RevisedScore provided:
     - writing → UPDATE tb_writing_submissions.band_score
     - speaking → UPDATE tb_speaking_sessions (fluency/pronunciation/grammar all set to revised value)
   → Student notified: "dispute_resolved" with teacher notes
   → AuditHelper.LogAsync(action: "Resolve")

6. REJECT action:
   → Status → "rejected", ResolvedAt = now
   → Student notified: "dispute_rejected"
   → AuditHelper.LogAsync(action: "Reject")
```

**Business Rules:**
| Rule | Implementation |
|---|---|
| Can only claim `pending` disputes | Validate `status == "pending"` before claim |
| Can only resolve/reject `under_review` disputes | Validate status before action |
| TeacherNotes is mandatory | Return ModelState error if empty/whitespace on both resolve AND reject |
| Polymorphic score sync | Branch on `submission_type == "writing"` vs `"speaking"` |
| Non-admin teachers see only their claimed disputes in "Under Review" | Filter: `reviewed_by == currentUserId` (admins see all) |

## 2.3 Content Management — Writing Prompts & Speaking Topics

> Both follow identical workflow patterns. The entities are `tb_writing_prompts` and `tb_speaking_topics`.

### Content Lifecycle (State Machine):
```
  draft ──→ pending_review ──→ published
    ↑              │                │
    │              ↓                │
    └── (edit) ← rejected ←────────┘ (edit resets to draft)
```

### CRUD Flow:
```
CREATE:
  → Teacher fills form (prompt_text/title, task_type/part, difficulty, sample_answer)
  → All text sanitized with InputSanitizer.StripHtmlTags()
  → Status = "draft", CreatedBy = currentUserId
  → AuditHelper.LogAsync(action: "Create")
  → Content is NOT visible to students yet

EDIT:
  → Teacher modifies content fields
  → If status was "published" or "rejected" → reset to "draft" (requires re-review)
  → AuditHelper.LogAsync(action: "Edit")

SUBMIT FOR REVIEW:
  → Status changes to "pending_review"
  → Awaits peer approval

APPROVE (Peer Review):
  → BLOCK: CreatedBy == currentUserId → "You cannot approve your own content"
  → Status → "published"
  → ReviewedBy = currentUserId, ReviewedAt = now
  → Creator notified: "content_approved"
  → AuditHelper.LogAsync(action: "Approve")

REJECT (Peer Review):
  → Same self-approval block applies
  → Status → "rejected"
  → Creator notified: "content_rejected"
  → AuditHelper.LogAsync(action: "Reject")

SOFT DELETE:
  → NEVER call _context.Remove() — Teachers cannot hard-delete
  → Set Status = "rejected" (archives it, hides from students)
  → AuditHelper.LogAsync(action: "SoftDelete")
```

### WritingPrompt Fields:
`PromptId`, `TaskType` (task1/task2), `PromptText`, `PromptImageUrl`, `ChartType`, `DifficultyLevel`, `Category`, `BandTarget`, `SampleAnswer`, `IsActive`, `NotesForTeacher`, `Status`, `CreatedBy`, `ReviewedBy`, `ReviewedAt`, `ReviewerNote`, `IsDeleted`

### SpeakingTopic Fields:
`TopicId`, `Part` (1/2/3), `TopicTitle`, `Description`, `DifficultyLevel`, `BandTarget`, `IsActive`, `Status`, `CreatedBy`, `ReviewedBy`, `ReviewedAt`, `ReviewerNote`, `IsDeleted` + child `tb_speaking_topic_parts` (PartNumber, ContentText, TimeLimitSeconds, IsFollowUp)

## 2.4 Student Messaging (Teacher Side — Dashboard MVC Area)

```
FLOW:
1. Teacher opens Conversations page → Two sections:
   OPEN (Unassigned): status="open", teacher_id=null — visible to ALL teachers
   MY ACTIVE: status="active", teacher_id=currentUserId

2. Teacher clicks into an OPEN conversation → Details page:
   → Banner: "You are the first to reply. Replying will assign this conversation to you."
   → Chat thread: messages ordered by created_at ASC, alternating left/right by sender role
   → Reply form at bottom

3. Teacher sends first reply (POST):
   → Message added to tb_messages
   → conversation.TeacherId = currentUserId (auto-assign)
   → conversation.Status = "active"
   → Student notified: "new_message" — "A teacher has joined your conversation"
   → AuditHelper.LogAsync(action: "Reply")

4. Ongoing replies:
   → New messages added
   → conversation.LastMessageAt updated
   → Student notified each time

5. CLOSE conversation (POST):
   → Status → "closed"
   → AuditHelper.LogAsync(action: "Close")
```

## 2.5 Teacher Dashboard MVC Pages (Areas/Teacher/)

| Controller | Views | Purpose |
|---|---|---|
| `GradeDisputesController` | Index, Details, Edit | Tabbed dispute queue, submission viewer, resolve/reject form |
| `WritingPromptsController` | Index, Create, Edit, Details, Delete | CRUD + peer-review for writing prompts |
| `SpeakingTopicsController` | Index, Create, Edit, Details, Delete | CRUD + peer-review for speaking topics |
| `ConversationsController` | Index, Details, Edit | Message queue, chat thread, reply form |

---

# ROLE 3: ADMIN

> **Authorization:** `[Authorize(Roles = "admin")]` for admin-only actions. Admins also inherit all teacher capabilities.

## 3.1 User Management (Admin Area — `Areas/Admin/UsersController`)

### INDEX — User List
```
FEATURES:
  - Paginated table (10 per page)
  - Searchable: email OR full_name (case-insensitive)
  - Filter by role: student | teacher | admin | moderator
  - Filter by status: active | banned | inactive
  - Ordered by CreatedAt DESC
  - Columns: Avatar (initials fallback), Full Name, Email, Role (badge), Status (badge), Created At, Actions
  - Actions per row: Details, Edit, Ban/Unban toggle, Delete
  - TempData["Success"] / TempData["Warning"] alert banners
```

### CREATE — New User
```
FLOW:
  → Form: Email, Role (select), Status (select), Password
  → Validate: password not empty, email not already taken
  → Hash password: BCrypt.Net.BCrypt.HashPassword(password)
  → Create tb_users row
  → Create tb_user_profiles row (FullName defaults to email prefix)
  → AuditHelper.LogAsync(action: "Create")
```

### EDIT — Modify User
```
FLOW:
  → ONLY editable: Role (select) and Status (select)
  → Role whitelist validation: ["student", "teacher", "admin", "moderator"]
  → BLOCK: Cannot edit own account (R10: targetUserId == GetCurrentUserId() → redirect with warning)
  → AuditHelper.LogAsync(action: "Edit", details: "role: old→new, status: old→new")
```

### TOGGLE BAN — Quick Action
```
FLOW:
  → POST only (from Index page button)
  → Toggles Status between "active" and "banned"
  → BLOCK: Cannot ban own account
  → AuditHelper.LogAsync()
```

### DELETE — Hard Delete
```
FLOW:
  → GET: Confirmation page showing user details
  → POST: HARD DELETE using _context.Users.Remove(user)
  → This is the ONLY place .Remove() is permitted in the entire codebase
  → BLOCK: Cannot delete own account
  → AuditHelper.LogAsync(action: "HardDelete") BEFORE removing
  → Note: FK cascade constraints may require deleting child records first
```

### Role Change via API
```
  → PATCH /api/admin/users/{id}/role { role: "teacher" }
  → Validates against whitelist
  → AuditHelper.LogAsync()
```

## 3.2 Admin Capabilities Summary

| Capability | Admin | Teacher | Student |
|---|---|---|---|
| View all users | ✅ | ❌ | ❌ |
| Create users | ✅ | ❌ | ❌ |
| Edit user role/status | ✅ | ❌ | ❌ |
| Ban/Unban users | ✅ | ❌ | ❌ |
| Hard-delete users | ✅ | ❌ | ❌ |
| Change user roles | ✅ | ❌ | ❌ |
| View audit logs | ✅ | ❌ | ❌ |
| Claim/Resolve disputes | ✅ | ✅ | ❌ |
| See ALL under_review disputes | ✅ | Only own claimed | ❌ |
| Create/Edit content | ✅ | ✅ | ❌ |
| Approve/Reject content (peer) | ✅ | ✅ (not own) | ❌ |
| Hard-delete content | ✅ (only via Users) | ❌ (soft only) | ❌ |
| Reply to conversations | ✅ | ✅ | Own only |
| Close conversations | ✅ | ✅ | ❌ |
| Submit disputes | ❌ | ❌ | ✅ |
| Take tests/practice | ❌ | ❌ | ✅ |
| View own progress | ❌ | ❌ | ✅ |

## 3.3 Admin Dashboard MVC Pages (Areas/Admin/)

| Controller | Views | Purpose |
|---|---|---|
| `UsersController` | Index, Create, Edit, Details, Delete | Full user CRUD with search, filter, pagination |

---

# CROSS-CUTTING SYSTEMS

## Audit Logging

Every mutation across Admin and Teacher areas MUST call:
```csharp
await AuditHelper.LogAsync(context, userId, action, entityType, entityId, oldValues, newValues);
```

| Field | Values |
|---|---|
| `action` | `create_writing_prompt`, `edit_speaking_topic`, `claim_dispute`, `resolve_dispute`, `reject_dispute`, `hard_delete_user`, `change_user_role`, etc. |
| `entityType` | `WritingPrompt`, `SpeakingTopic`, `GradeDispute`, `User`, `Conversation` |
| Stored in | `tb_audit_logs` with `old_values_json` / `new_values_json`, IP, user agent, timestamp |

## Notification System

| Type | Trigger | Recipient |
|---|---|---|
| `dispute_claimed` | Teacher claims dispute | Student |
| `dispute_resolved` | Teacher resolves dispute | Student |
| `dispute_rejected` | Teacher rejects dispute | Student |
| `new_message` | Teacher replies in conversation | Student |
| `new_conversation` | Student starts conversation | All teachers |
| `content_approved` | Peer approves content | Content creator |
| `content_rejected` | Peer rejects content | Content creator |

All notifications: `tb_notifications` with `UserId`, `Type`, `Title`, `Message`, `IsRead`, `CreatedAt`, `ExpiresAt` (30 days default).

## Input Sanitization

Every user text input MUST be sanitized before DB save:
```csharp
var clean = InputSanitizer.StripHtmlTags(rawInput);
```

## Security Rules Summary

| Rule | Detail |
|---|---|
| Roles are lowercase | `"student"`, `"teacher"`, `"admin"`, `"moderator"` — case-sensitive in JWT claims |
| No EF migrations | Use `Database.EnsureCreated()` + raw SQL scripts only |
| No `.Remove()` in Teacher area | Teachers soft-delete only (set status/flag) |
| Admin self-protection | Admin cannot edit/delete/ban own account |
| Self-approval block | Teacher cannot approve their own content |
| TeacherNotes required | On both resolve AND reject of disputes |
| Data isolation | Students see only their own data |
| Delete behavior | `DeleteBehavior.Restrict` on all FKs — no cascades |

---

# DATABASE ENTITY MAP (41 Tables)

## User Domain
| Table | Key | Role Access |
|---|---|---|
| `tb_users` | `user_id` | Admin: CRUD. Teacher: read students. Student: read self. |
| `tb_user_profiles` | `user_id` (1:1) | Same as users |
| `tb_user_goals` | `goal_id` | Student: own. Teacher: read. |
| `tb_user_sessions` | `session_id` | System (auth tokens) |
| `tb_user_placement_results` | `result_id` | Student: own |
| `tb_user_learning_progress` | `id` | Student: own. Teacher: read. |
| `tb_user_daily_activity` | `activity_id` | Student: own |

## Learning Domain
| Table | Key | Purpose |
|---|---|---|
| `tb_courses` | `course_id` | Course containers |
| `tb_modules` | `module_id` | Sections within courses |
| `tb_lessons` | `lesson_id` | Individual learning units |
| `tb_lesson_contents` | `content_id` | Video/article/audio/exercise |
| `tb_vocabulary` | `vocab_id` | Word bank |

## Test & Practice Domain
| Table | Key | Purpose |
|---|---|---|
| `tb_tests` | `test_id` | Test definitions |
| `tb_test_sections` | `section_id` | Sections by skill type |
| `tb_questions` | `question_id` | Individual questions |
| `tb_answers` | `answer_id` | Correct answers |
| `tb_user_test_attempts` | `attempt_id` | Test attempts with band scores |
| `tb_user_answers` | `id` | User's answers per attempt |
| `tb_reading_passages` | `passage_id` | Reading materials |
| `tb_reading_questions` | `question_id` | Reading questions (5 types) |
| `tb_listening_materials` | `material_id` | Audio materials |
| `tb_listening_questions` | `question_id` | Listening questions |
| `tb_user_practice_attempts` | `attempt_id` | Reading/Listening attempts |
| `tb_writing_prompts` | `prompt_id` | Essay prompts (with approval workflow) |
| `tb_writing_submissions` | `submission_id` | Student essays + AI scores |
| `tb_speaking_topics` | `topic_id` | Speaking topics (with approval workflow) |
| `tb_speaking_topic_parts` | `part_id` | Cue cards/questions per topic |
| `tb_speaking_sessions` | `session_id` | Student speaking + AI scores |

## Teacher Domain
| Table | Key | Purpose |
|---|---|---|
| `tb_grade_disputes` | `dispute_id` | Student→Teacher dispute tickets |
| `tb_conversations` | `conversation_id` | Messaging threads |
| `tb_messages` | `message_id` | Individual messages |

## System Domain
| Table | Key | Purpose |
|---|---|---|
| `tb_notifications` | `notification_id` | User notifications |
| `tb_audit_logs` | `log_id` | All mutation audit trail |
| `tb_admin_actions` | `action_id` | Legacy admin actions |
| `tb_system_settings` | `setting_key` | Key-value config |
| `tb_reports` | `report_id` | User reports/feedback |
| `tb_gamification` | `achievement_id` | Achievement definitions |
| `tb_user_achievements` | `user_achievement_id` | Earned achievements |

## AI Domain
| Table | Key | Purpose |
|---|---|---|
| `tb_ai_roadmaps` | `roadmap_id` | AI-generated study plans |
| `tb_ai_roadmap_steps` | `step_id` | Individual roadmap steps |
| `tb_ai_skill_analysis` | `id` | AI skill-level analysis |
