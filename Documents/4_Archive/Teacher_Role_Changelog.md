# Teacher Role Implementation — Changelog

> **Started:** 2026-05-15  
> **Author:** AI Architect (Antigravity)  
> **Reference:** `File MD/WebIeltsFree_Teacher_Role_Analysis.md`

---

## Implementation Order (Low Risk → High Impact)

| Order | Task | Risk | Status |
|-------|------|------|--------|
| 1 | T-1: Teacher role in Auth + Program.cs policy | LOW | ✅ |
| 2 | T-4: Teacher Dashboard API | LOW | ✅ |
| 3 | T-2: Grade Dispute System | MEDIUM | ✅ |
| 4 | T-3: Messaging System | MEDIUM | ✅ |
| 5 | T-5: Teacher Content CRUD Permissions | MEDIUM | ✅ |
| 6 | T-6: Dispute & Message Notifications | LOW (depends on T-2, T-3) | ✅ |

---

## Foundation Changes (Applied Across All Tasks)

### New Files Created
| File | Purpose |
|------|---------|
| `Models/AuditHelper.cs` | Static helper to write audit log entries — avoids repeating code in every controller |
| `Controllers/DisputesController.cs` | Grade dispute ticket system (T-2) |
| `Controllers/MessagesController.cs` | Student ↔ Teacher async messaging (T-3) |
| `Controllers/TeacherController.cs` | Teacher dashboard + student detail views (T-4) |

### Modified Files
| File | Changes |
|------|---------|
| `Models/Entities.cs` | Added 3 new entities: `GradeDispute`, `Conversation`, `Message`. Added nav properties to `User`. |
| `Models/AppDbContext.cs` | Added 3 new `DbSet<>` entries + `OnModelCreating` FK relationships |
| `Program.cs` | Added `TeacherOrAdmin` authorization policy |
| `Controllers/UsersController.cs` | Added `PATCH api/admin/users/{id}/role` (admin only) |
| `Controllers/WritingController.cs` | Added teacher CRUD for writing prompts (POST/PUT/DELETE) |
| `Controllers/SpeakingController.cs` | Added teacher CRUD for speaking topics + parts |
| `Controllers/ReadingController.cs` | Added teacher CRUD for reading passages + questions |
| `Controllers/ListeningController.cs` | Added teacher CRUD for listening materials + questions |
| `Controllers/LessonsController.cs` | Added `[Authorize(Roles="teacher,admin")]` to write endpoints |

### Design Decisions
1. **Soft-delete standardized on `IsDeleted` flag** — all teacher deletes set `IsDeleted = true`, never remove rows
2. **AuditHelper** — centralized `AuditHelper.LogAsync(context, userId, action, entityType, entityId, oldValues, newValues)` to DRY audit logging
3. **Parallel code principle** — new teacher endpoints are added alongside existing student logic, never modifying stable student flows
4. **Authorization pattern** — `[Authorize(Roles = "teacher,admin")]` on all teacher endpoints; `[Authorize(Roles = "student")]` on dispute submission

---

## Task Details

### T-1: Teacher Role in Auth (LOW RISK)
- **Program.cs**: Added `TeacherOrAdmin` policy via `builder.Services.AddAuthorization()`
- **JWT claims**: Verified `ClaimTypes.Role` already included in `GenerateJwtToken()` — no change needed
- **UsersController**: Added `PATCH api/admin/users/{id}/role` (admin-only) to promote users to teacher
- **No SQL changes**: User confirmed DB already has teacher role in ENUM

### T-4: Teacher Dashboard API (LOW RISK)
- **TeacherController**: New controller at `api/teacher` with `[Authorize(Roles = "teacher,admin")]`
  - `GET api/teacher/dashboard` — summary stats (pending disputes, open conversations, total students, resolved this week)
  - `GET api/teacher/students` — paginated student list with bands and activity
  - `GET api/teacher/students/{id}/detail` — full academic profile for one student

### T-2: Grade Dispute System (MEDIUM RISK)
- **GradeDispute entity**: Mapped to `tb_grade_disputes` with all columns
- **DisputesController**: 
  - `POST api/disputes/submit` — student submits dispute (student-only)
  - `GET api/disputes` — list pending/under_review disputes (teacher/admin)
  - `GET api/disputes/{id}` — dispute detail (teacher/admin)
  - `PATCH api/disputes/{id}/claim` — teacher claims dispute
  - `PATCH api/disputes/{id}/resolve` — teacher resolves with notes + optional revised score
  - `PATCH api/disputes/{id}/reject` — teacher rejects with notes
- **Score override**: On resolve with revised score, updates `band_score` in the original submission

### T-3: Messaging System (MEDIUM RISK)
- **Conversation + Message entities**: Mapped to `tb_conversations` and `tb_messages`
- **MessagesController**:
  - `POST api/messages/conversations/start` — student opens new thread
  - `GET api/messages/conversations` — students see own; teachers see all open/active
  - `GET api/messages/conversations/{id}/messages` — messages in thread (ownership enforced)
  - `POST api/messages/conversations/{id}/send` — send message; teacher auto-assigned on first reply
  - `PATCH api/messages/conversations/{id}/mark-read` — mark messages read
  - `PATCH api/messages/conversations/{id}/close` — teacher/admin closes thread

### T-5: Teacher Content CRUD (MEDIUM RISK)
- Added teacher-authorized CRUD endpoints across all content controllers
- All DELETE endpoints use soft-delete (`IsDeleted = true`)
- All mutations write to `tb_audit_logs` via `AuditHelper`

### T-6: Dispute & Message Notifications (LOW RISK, depends on T-2/T-3)
- Notifications fire on: dispute claimed, resolved, rejected, new message, new conversation
- Uses existing `tb_notifications` + `NotificationsController` patterns
