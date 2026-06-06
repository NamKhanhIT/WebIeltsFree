# WebIeltsFree - Business Logic & Workflows

This document outlines the detailed workflows and business logic rules of the platform, providing a comprehensive basis for writing test cases and validating the system.

## 1. User Registration & Onboarding Flow
1. **Registration:** User registers via `POST /api/auth/register`. A user account is created with the default role `student`. Email verification token is generated and sent.
2. **Email Verification:** User clicks link -> `GET /api/auth/verify-email?token={token}` sets `email_verified = true`.
3. **Placement Test:** User takes an initial adaptive test (`tb_user_placement_results`). 
4. **Roadmap Generation:** Based on placement results, the AI generates a customized `AIRoadmap` mapping out the user's study plan.

## 2. Learning & Practice Workflows

### 2.1 Reading & Listening Practice
- **Data Fetching:** Students request practice sets by difficulty (e.g., `band_5_6`). The API returns a random passage/material and its questions, **omitting the correct answers**.
- **Submission:** Student submits an answer via `POST /api/reading/submit` or `/api/listening/submit`.
- **Validation Logic:**
  - `multiple_choice`: Case-insensitive exact match (A, B, C, D).
  - `true_false_not_given`: Case-insensitive exact match of standard terms (`true`, `false`, `not given`).
  - `fill_in_blank` / `sentence_completion` / `summary_completion`: Case-insensitive, whitespace-trimmed comparison against a comma-separated word bank (aliases allowed, e.g. "3" or "three").
  - `matching_heading`: All correct answers must be provided, no extras.
- **Recording:** The attempt is logged in `tb_user_practice_attempts` with `TimeSpentSeconds` and `IsCorrect`.

### 2.2 Speaking & Writing Practice (AI Grading)
- **Speaking:** Student submits an audio recording/transcript via `POST /api/speaking/submit-audio`.
  - The Python AI service evaluates the transcript for Fluency, Pronunciation, Lexical Resource, and Grammatical Range.
  - Generates an overall Band Score and detailed feedback. A fallback mock evaluation is used if the AI service fails.
- **Writing:** Student submits an essay based on a prompt via `POST /api/writing/submit`.
  - The AI evaluates Task Achievement, Coherence & Cohesion, Lexical Resource, and Grammar.
  - Both submissions are saved to their respective tables (`tb_speaking_sessions`, `tb_writing_submissions`).

## 3. Teacher Workflows & Interventions

### 3.1 Grade Dispute Workflow
If a student feels the AI graded them unfairly on a Writing or Speaking task:
1. **Submission:** Student calls `POST /api/disputes/submit` with a `reason`. Ticket created with status `pending`.
2. **Discovery:** Teachers view the open pool via `GET /api/disputes`.
3. **Claiming:** A teacher calls `PATCH /api/disputes/{id}/claim`. Status changes to `under_review`, `reviewed_by` is set.
4. **Resolution:** 
   - Teacher calls `PATCH /api/disputes/{id}/resolve`. They **must** provide `TeacherNotes`. 
   - They may optionally provide a `RevisedScore`. If provided, the original submission's band score is updated.
   - Status changes to `resolved`.
   - Alternatively, teacher calls `PATCH /api/disputes/{id}/reject` (requires notes, status `rejected`).
5. **Notification:** Student receives a notification regarding the resolution and teacher's feedback.

### 3.2 Student Support & Messaging Workflow
1. **Initiation:** Student starts a thread via `POST /api/messages/conversations/start`. Status is `open`. Notification sent to all teachers.
2. **Claiming/Replying:** A teacher views the open thread. Upon sending the first reply (`POST /api/messages/conversations/{id}/send`), the teacher is auto-assigned to the conversation (`teacher_id` set) and status becomes `active`.
3. **Ongoing Chat:** Async messaging continues. Users can mark messages as read.
4. **Closure:** Teacher or Admin can close the conversation (`PATCH /api/messages/conversations/{id}/close`), changing status to `closed`.

### 3.3 Content Management Workflow
Teachers can create and manage curriculum content (Prompts, Topics, Passages, Materials).
- **Creation/Update:** `POST` and `PUT` requests allow teachers to add or modify questions, prompts, and audio links.
- **Deletion:** Teachers can only perform **soft deletes**. `DELETE` requests set flags like `IsActive = false` (or remove lesson links, effectively archiving them without deleting DB rows).
- **Auditing:** Every mutation triggers `AuditHelper.LogAsync()`, recording the `AdminId`/`UserId`, Action, and Entity details in `tb_audit_logs`.

## 4. Security & Access Control Rules
- **Role Enforcement:** Endpoints strictly use `[Authorize(Roles = "teacher,admin")]` or `[Authorize(Roles = "student")]`.
- **Data Isolation:** Students can only view their own practice history, conversations, and disputes. Teachers can view any student's academic profile via specific teacher-only endpoints.
- **Pedagogical Limits:** Teachers cannot access system logs, ban users, change system settings, or perform hard database deletions.
