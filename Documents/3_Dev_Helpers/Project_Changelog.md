# WebIeltsFree - Master Changelog

This document tracks all major features, integrations, and architectural changes in the WebIeltsFree platform.

## [Version 2.5.4] - 2026-05-21
### Teacher Test Creation Bugfixes, Safe Audit Serialization, and Seamless P2P Handover
**Status:** ✅ PRODUCTION READY  
**Objective:** Resolve critical database and serialization errors during Teacher Test Creation, implement a seamless post-creation Peer-to-Peer (P2P) assignment flow, and resolve Razor build issues.

---

#### 1. Core Test Creation & Transaction Bugfixes
- **Object Cycle Exception Fix:** Resolved a critical JSON serialization error during audit logging where `JsonSerializer.Serialize(test)` failed due to a circular reference between `Test` and `TestSections`. Refactored to serialize a safe anonymous DTO (`new { test.TestId, test.Title, test.Difficulty, test.DurationMinutes }`).
- **Rollback Safety & Error Clarity:** Wrapped `transaction.RollbackAsync()` inside a try-catch block inside the `CreateTest` catch handler. This prevents rollback exceptions from masking the original database or serialization error.
- **Database Schema Constraint Alignment:** Ensured `tb_answers` in the local DB fully aligns with `DatabaseScripts/ieltsdb.sql` by enforcing `PRIMARY KEY` and `AUTO_INCREMENT` on `answer_id`, preventing DbUpdateConcurrencyException errors on insertions.

---

#### 2. Seamless Peer-to-Peer (P2P) Assignment Flow
- **Auto-Handover Routing:** Configured the `CreateTest` success handler to automatically redirect teachers to the assignment screen: `/Teacher/TestAssignments/Assign?testId={testId}`.
- **Pre-Selected Test Support:** Updated the `Assign(int? testId)` GET action to pass `ViewBag.SelectedTestId` to the view.
- **Context-Aware Assignment UI (`Assign.cshtml`):** 
  - Displays a customized header ("Submit to Peer Teacher") and onboarding tips when coming from test creation.
  - Groups recipients in the dropdown list using `<optgroup>` under "👩‍🏫 Peer Teachers / Admins" and "🎓 Students" for quick selection.
  - Automatically pre-selects the newly created test template in the dropdown.
  - Switches the submit button text dynamically to **"Send to Peer"** when submitting a P2P task.

---

#### 3. Razor Compilation Bugfix
- **CS1501 Compile Fix:** Resolved a compilation crash (`No overload for method 'Write' takes 0 arguments`) caused by a single-line Razor block in `Assign.cshtml` by converting it to a clean C# ternary expression: `@(isFromCreate ? "Send to Peer" : "Assign Test")`.

---

## [Version 2.5.3] - 2026-05-20
### IELTS Writing Engine Enhancements, Backend Validation & Premium UI Hardening
**Status:** ✅ PRODUCTION READY  
**Objective:** Enrich the IELTS Writing Practice Studio with a dynamic Live Suggestions engine, half-band rounding alignment, a comprehensive backend validation defense suite, and premium UX/layout bugfixes.

---

#### 1. Front-End Live Suggestions Engine & Interactive Editor
- **Real-Time Client-Side Engine:** Created a dynamic JavaScript text analyzer that monitors typing in the Quill canvas with a 300ms performance debouncer.
- **Lexical Resource Upgrades & Grammar Warnings:** Expanded vocabulary checkers with 35+ advanced academic transition patterns and 15+ common grammatical rules (e.g. subject-verb agreement).
- **Interactive Quill Auto-Replace:** Wired up interactive checkmarks calling `window.applySuggestion()` to instantly overwrite weak items directly in the Quill canvas and trigger immediate re-scans.
- **Cohesion Flow Analytics:** Built a sentence-starter parser tracking structural repetition to estimate real-time feedback.

---

#### 2. C# Backend Content Validation & Exam Scoring Alignment
- **IELTS Half-Band Standard Rounding:** Corrected the AI feedback band score logic to strictly calculate overall bands in intervals of `0.5` instead of returning unrounded decimals (e.g., 6.3 or 5.6).
- **Comprehensive Essay Filtering (`ValidateEssayContent`):** Introduced strict server-side rules in `WritingController.cs` to filter out:
  - Empty submissions or under-length inputs (Task 1 < 50 words, Task 2 < 100 words).
  - Keyboard gibberish and vowel-less typing spam.
  - Abusive or profane text (filtered using lists for both English and Vietnamese).
  - Extreme copy-paste repetition (e.g. duplicating words repeatedly).
- **Off-Topic Smart Capping:** If an essay has zero prompt-keyword alignment, the API grades it normally (avoiding absolute blocks) but prepends a high-priority warning toast/header and caps the **Task Achievement** score at `4.5` to simulate real-world IELTS rubric penalties.

---

#### 3. Premium UI/UX Modal & Layout Hardening
- **Universal Close Modal Fix:** Solved the unreliable diagnostic modal closure by promotion to global scope (`window.closeFeedbackModal`) combined with an explicit DOM event listener binding to `#closeFeedbackModalBtn` inside the `DOMContentLoaded` lifecycle block.
- **Glassmorphism Backdrop & Outside-Click Closing:** Added click-outside-to-close on the `#writingFeedbackPanel` overlay backdrop, protected by propagation stop blocks on the inner card. Enhanced the visual feel using premium `backdrop-blur-sm` style tokens.
- **Oversized SVGs Layout Bugfix:** Replaced unsupported Tailwind CSS fractional classes (`h-4.5 w-4.5`) inside *AI Detailed Assessment* and *Key Recommendations* headers with standard, crisp `h-5 w-5 flex-shrink-0` (`20px`) classes, restoring visual consistency.

---

## [Version 2.5.2] - 2026-05-19
### AI IELTS Writing Workspace Redesign & Performance Optimization
**Status:** ✅ PRODUCTION READY  
**Objective:** Redesign the IELTS Writing Task 1 Practice page into an immersive, premium, AI-powered writing studio workspace. Hotfix rendering blocks, infinity loop triggers, and script conflicts with legacy components.

---

#### Hotfix & Performance Optimization 🛠️
- **Scrollability and Non-Blocking Layouts:** Set natural viewport scrolling constraints via `html, body { overflow-x: hidden; overflow-y: auto; }` and removed all rendering blocks from stacked position overlays.
- **Legacy Script Decoupling (`_PracticeScripts.cshtml`):** Added conditional checks to bypass conflicting native text listeners, autosave syncs, and word counter bindings in `_PracticeScripts.cshtml` whenever the Quill editor is initialized.
- **Infinite Loop Resolution:** Added `isUpdating` block locks in Quill event pipelines to prevent recursive sync fires between hidden MVC fields and state caches.
- **CPU Spike Mitigation:** Completely deleted heavy `backdrop-blur` CSS filters, infinite pulse shadow animations, and legacy timeout typing timers.
- **Conflict-free Modal Visibility:** Rewrote Bootstrap/Tailwind class toggles to utilize safe inline style mutations (`style.display = 'none'` / `style.display = 'flex'`), preventing rendering blocks.
- **Recovery Paste Refactoring:** Replaced all recovery text initializations with `quill.clipboard.dangerouslyPasteHTML(cached)` to bypass event dispatch loops.

---

#### Redesigned UI/UX 🎨

##### 1. Responsive Multi-Column Root Layout (`_PracticeWriting.cshtml`)
- Wrapped the entire interface inside a beautiful, flex-row root container: `<div class="h-screen w-full flex overflow-hidden lg:flex-row flex-col">`.
- Hide the right AI sidecar on smaller viewports (`hidden xl:flex`) to guarantee premium mobile responsive behavior.
- Cleaned up body/html style resets, defining modern layout constraints: `* { box-sizing: border-box; }`, removing arbitrary scroll locks, and setting natural view backgrounds (`#f6f8fc`).

##### 2. Re-proportioned Sidebars
- **Left Sidebar (Task Overview):** Standardized width from `340px` to `w-[320px] min-w-[320px] max-w-[320px]` with transition classes.
- **Right Sidebar (AI Mentor / History):** Standardized width from `380px` to `w-[360px] min-w-[360px] max-w-[360px]` with transition classes.
- **Center Editor Column:** Wrapped in `flex-1 min-w-0 flex flex-col relative overflow-hidden` to prevent CSS flex-collapse and ensure automatic expansion.

##### 3. Premium Notion-Like AI Editor Experience
- Expanded editor canvas to `max-w-4xl mx-auto`.
- Added **AI Floating Placeholder & Assistant panel** inside the editor canvas, offering real-time guidance prompts: *Sample Introduction*, *Band 7.5 Hints*, and interactive onboarding instructions. Toggles off automatically when typing is detected, and restores when empty.
- Increased Quill canvas limits: set `.ql-container` and `.ql-editor` min-height to `500px` with responsive padding rules.
- Refactored **Live AI Performance Bar** from absolute positioning to relative layout at the editor base, removing layout overlap/breakages.

##### 4. Focus Mode & Interactive Real-Time Suggestions
- **Focus Mode:** Implemented real-time focus listener in Quill: focusing on the editor dynamically applies `.left-sidebar-blur` to the task pane, collapses the right sidebar into a compact layout, and highlights the editor studio.
- **Prefilled AI Annotations:** When no local or backend drafts exist, the editor pre-populates with a premium IELTS essay showing a real-time hover suggestion checker underline. Hovering over the error shows a gorgeous floating tooltip proposing lexical upgrades (*"increased dramatically"* instead of *"went up very fast"*).
- Escaped `@keyframes` as `@@keyframes` to guarantee absolute Razor MVC C# compilation compliance.

---

## [Version 2.5.1] - 2026-05-19
### Critical Navigation Decoupling & Interface Hardening
**Status:** ✅ PRODUCTION READY  
**Objective:** Eliminate all remaining cross-contamination between Learning (educational/tutorial) and Practice (exam simulation) interfaces, down to every clickable link, button label, and JavaScript handler.

---

#### Root Cause Analysis 🔍
The header navbar **"Practice" dropdown** (`_Layout.cshtml` lines 58-76) used `asp-action="Listening"`, `asp-action="Reading"`, `asp-action="Writing"`, `asp-action="Speaking"` — which the `HomeController.cs` defined as **legacy redirects to Learning views** (e.g. `public IActionResult Listening() => RedirectToAction(nameof(LearnListening))`). This caused every "Practice" menu click to open a **Learning** page instead of the exam simulation. The user experienced this as "clicking on listening skills details led to the practice link" — the interfaces were fundamentally crossed.

---

#### Fixed 🩹

##### 1. Header Navigation Routing (`Views/Shared/_Layout.cshtml`)
**Problem:** The "Practice" dropdown menu items used `asp-action="Listening"`, `asp-action="Reading"`, `asp-action="Writing"`, `asp-action="Speaking"` — all of which were controller actions that **redirected to Learning views**, not Practice views.  
**Fix:** Changed all four Practice dropdown items to use `asp-action="Practice"` with `asp-route-type="listening|reading|writing|speaking"`, which correctly routes to `Views/Practice/{Skill}.cshtml` via the `Practice(string type, int? id)` controller action.

| Menu Item | Before (BROKEN) | After (FIXED) |
|---|---|---|
| Listening Practice | `asp-action="Listening"` → ❌ `LearnListening.cshtml` | `asp-action="Practice" asp-route-type="listening"` → ✅ `Practice/Listening.cshtml` |
| Reading Practice | `asp-action="Reading"` → ❌ `LearnReading.cshtml` | `asp-action="Practice" asp-route-type="reading"` → ✅ `Practice/Reading.cshtml` |
| Writing Practice | `asp-action="Writing"` → ❌ `LearnWriting.cshtml` | `asp-action="Practice" asp-route-type="writing"` → ✅ `Practice/Writing.cshtml` |
| Speaking Practice | `asp-action="Speaking"` → ❌ `Speaking.cshtml` (Learn) | `asp-action="Practice" asp-route-type="speaking"` → ✅ `Practice/Speaking.cshtml` |

##### 2. Footer Navigation Labels (`Views/Shared/_Layout.cshtml`)
**Problem:** Footer "Platform" section had links labeled "Listening Practice" and "Reading Practice" that could be confused with learning content.  
**Fix:** Renamed to "Listening Exam" and "Reading Exam" while keeping the correct `/Home/Practice?type=...` URLs.

##### 3. Controller Routing Consistency (`Controllers/HomeController.cs`)
**Problem:** `Speaking()` action directly rendered `Views/Learn/Speaking.cshtml` instead of redirecting like the other legacy actions. This created inconsistent behavior where `/Home/Speaking` opened a Learning view directly, bypassing the redirect pattern.  
**Fix:**
- Added new `LearnSpeaking()` action method pointing to `Views/Learn/Speaking.cshtml`
- Changed `Speaking()` to redirect to `LearnSpeaking()`, matching the pattern of `Writing() → LearnWriting()`, `Listening() → LearnListening()`, `Reading() → LearnReading()`
- Updated comments to clearly annotate the two sections: "LEARNING only — educational, tutorial-based" vs "Legacy redirects → learning views (preserved for bookmark compatibility)"

##### 4. Practice Reading Highlighter (`Views/Practice/_PracticeReading.cshtml`)
**Problem:** The `toggleHighlighter()` function only toggled the cursor style — it never actually highlighted any text. The highlighter button appeared to do nothing when clicked.  
**Fix:** Implemented complete text selection highlighting:
- Added `mouseup` event listener on `#lessonContentReading` that fires only when `highlighterActive === true`
- Selected text is wrapped in `<span class="highlighted-text" style="background-color: rgba(255, 255, 0, 0.4)">` using `Range.surroundContents()`
- Added visual feedback on the highlighter button itself: toggles `.active` class and `backgroundColor` to `var(--exam-primary-soft)` when enabled
- Added try/catch for cross-element boundary selections that cannot be wrapped in a single `<span>`

##### 5. Practice Answer Options Not Clickable (`Views/Practice/_PracticeScripts.cshtml`)
**Problem:** The `renderReadingQuestion()` and `renderListeningQuestion()` functions generated answer options using `.ac-q-option` and `.ac-q-option-wrap` CSS classes from the old `_PracticeContent.cshtml` stylesheet. After the Practice views were decoupled to use `_PracticeStyles.cshtml` instead, these classes had **no CSS definitions**, making the answer radio buttons invisible and unclickable.  
**Fix:**
- **Reading questions:** Replaced `<label class="ac-q-option-wrap">` + `<span class="ac-q-option">` with `<label class="exam-option w-100 mb-0">` + `<div class="exam-option-label">${letter}</div>` + `<div>${text}</div>`, matching the `.exam-option` styles defined in `_PracticeStyles.cshtml`
- **Listening questions:** Same replacement pattern as reading
- **Selection handler:** Changed from `.ac-q-option-wrap input` → `.exam-option input`, and from `.ac-q-option.active` → `.exam-option.selected`
- **Layout:** Changed answer options from `d-flex flex-wrap gap-2` (inline pills) to `d-flex flex-column gap-2` (vertical list with letter labels A/B/C/D), matching real IELTS exam format
- **Text input fallback:** Changed from `class="form-control ac-input"` to `class="form-control exam-input mt-3"` for reading, and from `class="form-control ac-textarea"` to `class="form-control exam-textarea mt-3"` for listening

##### 6. Learning Viewer Empty State Text (`Views/Learn/_LessonViewerScripts.cshtml`)
**Problem:** When a lesson had no content blocks, the message read "You can still practice this skill using the button below" — referencing a Practice button that was already removed in version 2.5.0.  
**Fix:** Changed to "Check back later or explore other lessons."

##### 7. Practice Writing Submission History Overflow (`Views/Practice/_PracticeScripts.cshtml`, `Views/Practice/_PracticeStyles.cshtml`)
**Problem:** In the Practice Writing page, the AI submission history items rendered as custom `<button>` elements with very long prompt titles. Because the buttons lacked a responsive width limit, they expanded horizontally, overflowing the sidebar card boundaries and overlapping on the right side of the container.
**Fix:** 
- Applied `w-100` and `mt-1` classes to the rendered history `<button>` elements.
- Implemented custom styling for `.writing-history-item` in the `_PracticeStyles.cshtml` design system to force block display, allow normal text layout, and prevent overflow.
- Properly truncated long prompt titles with elegant text-overflow ellipsis, and integrated smooth premium hover animations and box-shadow feedback.

---

#### Enhanced ✨

##### 7. Learning Writing — Personal Draft Pad (`Views/Learn/LearnWriting.cshtml`)
**Addition:** Added a "Personal Draft Pad" section below the AI Writing Mentor panel, implementing the "Document Editor" requirement from the CriticalChanges spec. Includes:
- `<textarea>` with 8 rows, styled with warm `#faf8f2` background and Inter font
- Placeholder text guiding students to "draft essays, note down vocabulary, or practice writing structures"
- "Save Notes" button with `bi-save` icon
- Session-local note persistence indicator

##### 8. Learning Speaking — Card-Based Tutor (`Views/Learn/Speaking.cshtml`)
**Complete rewrite** from a lesson-card grid (which used the generic `_LessonViewer` inline viewer) to a purpose-built **Interactive Card-Based Tutor** UI:
- **Sidebar:** Lesson list with `sp-lesson-item` cards showing title, estimated time, and difficulty level
- **Main area:** Flashcard-based study system with:
  - `sp-flashcard` component with centered content, card type labels (Theory/Practice/Vocabulary), and card counter
  - Progress bar showing position in card stack (`sp-progress-fill`)
  - "Show Hint" toggle button that reveals tips extracted from lesson content
  - Previous/Next navigation with animated card transitions (translateX + opacity fade)
  - Lesson completion screen with trophy icon and "Review Again" button
- **Design system:** Custom `--sp-*` CSS variables (purple primary `#8b5cf6`, amber accent `#f59e0b`, light `#f8fafc` background)
- **Data flow:** Loads lesson contents via `/lessons/{id}` API, converts each content block into a study card with type detection (exercise → Practice card, vocab → Vocabulary card, default → Theory card)

##### 9. Learning Reading — Practice Questions Section (`Views/Learn/LearnReading.cshtml`)
**Addition:** Added a dedicated "Practice Questions" container (`#readingQuestionsContainer`) below the article content:
- Styled with warm `#fffcf8` background and golden border to visually distinguish from the main passage
- Question icon with `bi-question-circle` indicator
- The `loadReadingLesson()` function now separates lesson contents by `contentType`: items containing "exercise" are rendered in the questions section, all others appear in the main article
- Container is hidden when no exercise-type content exists

---

#### Architecture (Final Routing Map) 🏗️

```
LEARNING (Educational, Tutorial)              PRACTICE (Exam Simulation)
──────────────────────────────                ──────────────────────────
Header Nav: "Learn" dropdown                  Header Nav: "Practice" dropdown
  → /Home/Skill?type=reading                    → /Home/Practice?type=reading
  → /Home/Skill?type=listening                  → /Home/Practice?type=listening
  → /Home/Skill?type=writing                    → /Home/Practice?type=writing
  → /Home/Skill?type=speaking                   → /Home/Practice?type=speaking

Controller: Skill(type)                       Controller: Practice(type, id)
  → Views/Learn/LearnReading.cshtml             → Views/Practice/Reading.cshtml
  → Views/Learn/LearnListening.cshtml           → Views/Practice/Listening.cshtml
  → Views/Learn/LearnWriting.cshtml             → Views/Practice/Writing.cshtml
  → Views/Learn/Speaking.cshtml                 → Views/Practice/Speaking.cshtml

Design: Warm, editorial, serif               Design: Clinical, high-contrast, sans-serif
Features: AI tutor, vocabulary,               Features: Timer, highlighter, submit,
  bilingual, notes, strategies                  scoring, no hints, no guides
```

#### Files Modified (Complete List) 📁
| File | Lines Changed | Nature |
|---|---|---|
| `Views/Shared/_Layout.cshtml` | ~20 | Header Practice dropdown routing fixed; footer labels renamed |
| `Controllers/HomeController.cs` | ~12 | Added `LearnSpeaking()`, `Speaking()` → redirect, comments updated |
| `Views/Practice/_PracticeReading.cshtml` | ~50 | Highlighter fully implemented with text selection and visual feedback |
| `Views/Practice/_PracticeScripts.cshtml` | ~48 | `renderReadingQuestion()` and `renderListeningQuestion()` rewritten to use `.exam-option` |
| `Views/Learn/LearnWriting.cshtml` | ~12 | Added Personal Draft Pad section |
| `Views/Learn/Speaking.cshtml` | ~260 | Complete rewrite to Card-Based Tutor UI |
| `Views/Learn/LearnReading.cshtml` | ~35 | Added Practice Questions section, content type separation logic |
| `Views/Learn/_LessonViewerScripts.cshtml` | 1 | Empty state text updated |

#### Unchanged (Preserved) 🔒
- All API controllers (`ReadingController`, `ListeningController`, `WritingController`, `SpeakingController`, etc.) — Zero backend changes
- Database schema (`Models/Entities.cs`) — Zero modifications
- Admin/Teacher area functionality — Untouched
- `_PracticeContent.cshtml` — Still preserved for legacy reference, not referenced by active views
- `_PracticeScripts.cshtml` — Core logic preserved; only answer rendering functions updated


## [Version 2.5.0] - 2026-05-19
### Complete Learning & Practice Interface Redesign
**Status:** ✅ PRODUCTION READY

#### Architecture 🏗️
- **Product-Level Separation:** Learning and Practice now behave as two completely different products. Learning = guided understanding, AI tutoring, vocabulary. Practice = exam simulation, timing, scoring.
- **Skill-Specific Learning Views:** Each IELTS skill has a unique, dedicated learning UI:
  - Writing → Workshop/classroom layout with sidebar navigation, AI mentor, grammar reference
  - Listening → Dark immersive audio lab with synchronized transcript, keyword highlighting, speed controls
  - Reading → Warm article-focused workspace with bilingual toggle (EN/VI), vocabulary popup, notes system
  - Speaking → Conversation/tutor card-based UI (existing, enhanced)
- **Skill-Specific Practice Views:** Each skill has a dedicated exam-simulation partial:
  - `_PracticeReading.cshtml` → Split-pane (passage left, questions right), highlighter
  - `_PracticeListening.cshtml` → Audio-centered, section navigation, waveform display
  - `_PracticeWriting.cshtml` → Distraction-free editor, word count, AI scoring panel
  - `_PracticeSpeaking.cshtml` → Dark fullscreen AI examiner, camera/mic, real-time transcription

#### Added ✨
- **`Views/Learn/LearnWriting.cshtml`** — Writing workshop with lesson sidebar, band descriptors, sample essay rendering, AI mentor panel
- **`Views/Learn/LearnListening.cshtml`** — Immersive dark-themed listening lab with audio controls, transcript sync, vocabulary extraction
- **`Views/Learn/LearnReading.cshtml`** — Interactive reading workspace with bilingual toggle, AI translation popup, notes, paragraph summaries
- **`Views/Practice/_PracticeStyles.cshtml`** — Shared exam-mode design system (clinical, high-contrast, zero distractions)
- **`Views/Practice/_PracticeTimer.cshtml`** — Advisory timer with elapsed/recommended display, warning colors (never auto-submits)
- **`Views/Practice/_PracticeReading.cshtml`** — Split-pane exam layout, highlighter, question navigation
- **`Views/Practice/_PracticeListening.cshtml`** — Audio-first exam UI, section tabs, waveform, speed control
- **`Views/Practice/_PracticeWriting.cshtml`** — Editor with task checklist, word count, AI scoring panel
- **`Views/Practice/_PracticeSpeaking.cshtml`** — Dark fullscreen AI examiner with camera preview, safety monitor

#### Modified 🔄
- **`Controllers/HomeController.cs`:** Removed `lessonId` parameter from `Practice()`. Added `LearnWriting()`, `LearnListening()`, `LearnReading()` action methods. Legacy `/Writing`, `/Listening`, `/Reading` routes now redirect to learning views.
- **`Views/Practice/Reading.cshtml`:** Now uses `_PracticeReading.cshtml` instead of monolithic `_PracticeContent.cshtml`
- **`Views/Practice/Listening.cshtml`:** Now uses `_PracticeListening.cshtml`
- **`Views/Practice/Writing.cshtml`:** Now uses `_PracticeWriting.cshtml`
- **`Views/Practice/Speaking.cshtml`:** Now uses `_PracticeSpeaking.cshtml`

#### Fixed 🩹
- **Defensive JavaScript in `_PracticeScripts.cshtml`:** Resolved uncaught `TypeError` crashes during page initialization and result submissions by adding robust, null-safe checks on DOM queries (e.g. `listeningHub`, `readingHub`, `writingHub`, `speakingHub`, `progressBar`, and `progressLabel`).
- **Defensive JavaScript in `loadLesson`:** Prevented `TypeError: Cannot set properties of null (setting 'textContent')` when loading lessons under pure timing practice routes by implementing safe guards on `lessonTitle`, `lessonMeta`, and `lessonContentCommon` DOM nodes.
- **Robust Multi-View Submission Support:** Rewrote `collectSkillAnswer()` for Speaking practice to dynamically resolve both the legacy and the new decoupled layouts safely querying elements like `#speakingTranscriptArea` and `#speakingCallTimer`/`#speakingDuration` without crashes.

#### Removed ❌
- **`lessonId` from Practice routing** — Practice mode no longer references lessons
- **Learning aids from Practice views** — Removed guided transcripts, vocabulary panels, module navigation, AI curator insights, lesson progress from practice mode
- **Monolithic `_PracticeContent.cshtml` usage** — All 4 stubs now use skill-specific partials (file preserved for legacy reference)

#### Unchanged (Preserved) 🔒
- All API controllers, entities, DTOs — Zero backend changes
- Database schema — Zero modifications
- Admin/Teacher area functionality
- `_PracticeContent.cshtml` file preserved (not deleted, just no longer referenced)
- `_PracticeScripts.cshtml` — Still loaded for API interaction logic


## [Version 2.4.0] - 2026-05-18
### Dual Interface Architecture: Learning vs. Practice
**Status:** ✅ PRODUCTION READY

#### Architecture 🏗️
- **Learning Interface (LMS):** Warm editorial design (Serene Focus Design System — sage/teal palette, Playfair Display serif headlines, paper-textured canvas). Focused on courses, lessons, and teacher-curated content consumption.
- **Practice Interface (Gym):** Clinical high-contrast design (dark hero, sans-serif, blue/orange skill accents). Focused on timed exercises, AI grading, and exam simulation.
- **Separation Principle:** Learning uses `tb_courses → tb_modules → tb_lessons → tb_lesson_contents`. Practice uses `tb_reading_passages`, `tb_listening_materials`, `tb_writing_prompts`, `tb_speaking_topics`.

#### Added ✨
- **`Views/Learn/` folder** — Dedicated folder for the learning/LMS ecosystem:
  - `Courses.cshtml` — Editorial course catalog (moved from `Views/Home`)
  - `Skill.cshtml` — Lesson library with stats dashboard + inline viewer (moved from `Views/Home`)
  - `Speaking.cshtml` — Speaking lesson library with Part 1/2/3 strategy guides (moved from `Views/Home`)
  - `AiRoadmap.cshtml` — Personalized learning path roadmap (moved from `Views/Home`)
  - `AiTutor.cshtml` — AI chat assistant (moved from `Views/Home`)
  - `_LearnStyles.cshtml` — Complete CSS design system for the editorial learning UI
  - `_LessonViewer.cshtml` — Inline lesson content viewer partial
  - `_LessonViewerScripts.cshtml` — JavaScript for loading lesson content and marking completion
- **`Views/Practice/` folder additions** — Dedicated folder for practice & exam simulators:
  - `Tests.cshtml` — All practice tests catalog (moved from `Views/Home`)
  - `TakeTest.cshtml` — Exam simulator screen (moved from `Views/Home`)
  - `PlacementTest.cshtml` — Placement exam screen (moved from `Views/Home`)

#### Modified 🔄
- **Folder Reorganization:** Replaced the generic `Views/Home` view mix with clean folder boundaries. Now `Views/Home` contains only core system/infrastructure files (`Index`, `Dashboard`, `Login`, `Register`, `OnboardingQuestions`, `Privacy`).
- **`Controllers/HomeController.cs`:** Updated view rendering paths in Tests, TakeTest, Speaking, AiTutor, Courses, PlacementTest, AiRoadmap, and Skill action methods to render moved views from their new respective folders (`Views/Learn` and `Views/Practice`) while fully preserving routing URLs.

#### Unchanged (Preserved) 🔒
- All pre-existing `Views/Practice/` views (Listening, Reading, Writing, Speaking, `_PracticeContent.cshtml`, and `_PracticeScripts.cshtml`)
- All API controllers, entities, DTOs — Zero backend changes
- Database schema — Zero modifications to `Entities.cs`

## [Version 2.3.0] - 2026-05-18
### Password Visibility Toggle Feature
**Status:** ✅ PRODUCTION READY

#### Added ✨
- **Show/Hide Password Feature:** Added a custom password visibility toggle (eye/eye-slash icon button) directly within the password input groups on both login screens (`Views/Account/Login.cshtml` and `Views/Home/Login.cshtml`).
- **Interactive Scripting:** Embedded robust, vanilla JavaScript listeners to toggle input `type` (between `password` and `text`) and dynamically switch between `bi-eye` and `bi-eye-slash` Bootstrap Icons for a premium user experience.

#### Modified 🔄
- **Account Login View (`Views/Account/Login.cshtml`):** Refactored the password field structure and added a `@section Scripts` with vanilla JS handling password toggle functionality.
- **Home Login View (`Views/Home/Login.cshtml`):** Aligned password group design with Bootstrap input-group button append and added interactive toggle JS in `@section Scripts`.

## [Version 2.2.0] - 2026-05-15
### Teacher Role & Human-in-the-Loop Implementation
**Status:** ✅ PRODUCTION READY

#### Added ✨
- **Teacher Role Authorization:** Added `TeacherOrAdmin` policy and `teacher` enum to `tb_users`. Added admin endpoint to promote users.
- **Teacher Dashboard API (`TeacherController`):** Added endpoints for summary stats, paginated student lists, and detailed academic profiles for students.
- **Grade Dispute System (`DisputesController`):** 
  - Students can dispute AI scores on Writing/Speaking.
  - Teachers can claim, review, and resolve disputes (uphold or override AI score).
  - Score overrides automatically update the original submission's band score.
- **Messaging System (`MessagesController`):** 
  - Async direct messaging between students and an open pool of teachers.
  - First teacher to reply is auto-assigned to the thread.
- **Teacher Content CRUD:** Added Teacher authorization to create, update, and soft-delete content across `WritingController`, `SpeakingController`, `ReadingController`, and `ListeningController`.
- **Notification Integration:** Automated notifications for dispute status changes (claimed, resolved, rejected) and new messages/conversations.
- **Audit Logging:** Centralized `AuditHelper` to log all teacher mutations to `tb_audit_logs`.

#### Added ✨ (Dashboard Area Controllers)
- **MVC Areas Configuration:** Registered MVC routing for `Admin` and `Teacher` areas in `Program.cs`.
- **Admin Users Controller:** Implemented `UsersController` in Admin area for managing system users, including full CRUD, pagination, banning, and hard-delete capabilities.
- **Teacher Content Approval System:** Implemented `WritingPromptsController` and `SpeakingTopicsController` in Teacher area with draft, edit, peer-approval, and soft-delete workflows.

#### Modified 🔄
- Entity models updated with `GradeDispute`, `Conversation`, and `Message`.
- `AppDbContext` updated with new `DbSet`s and foreign key relationships (`DeleteBehavior.Restrict` applied to prevent cascade cycles).

---

## [Version 2.1.0] - 2026-04-09
### Navbar & Layout Integration
**Status:** ✅ PRODUCTION READY

#### Added ✨
- 🎵 Listening Practice link in navbar & footer platform.
- 📖 Reading Practice link in navbar & footer platform.
- Navbar dropdown divider for better visual organization.

#### Modified 🔄
- Footer Skills section URLs updated to map directly to dedicated practice controllers.
- Reordered footer links for better UX.

#### Removed ❌
- AI Tutor and API Docs removed from the footer platform section to make room for practice links.
