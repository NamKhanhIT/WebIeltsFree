# Design System Specification: The Academic Curator

## 1. Overview & Creative North Star
**Creative North Star: "The Digital Curator"**
This design system moves away from the "gamified" clutter of casual learning apps to embrace the sophisticated, structured world of high-end academic publishing. Our goal is to create an environment that feels like a private, sunlit library—quiet, authoritative, and meticulously organized.

To break the "template" look, we utilize **Intentional Asymmetry** and **Tonal Depth**. Instead of rigid, boxed-in grids, the layout uses overlapping surfaces and varying typographic scales to guide the eye. We don't just present information; we curate a learning journey through white space and editorial-grade hierarchy.

---

## 2. Colors & Surface Architecture
The color palette is rooted in a deep, scholarly blue, accented by a success-oriented emerald. However, the premium feel is established in the neutrals.

### The "No-Line" Rule
**Strict Mandate:** Designers are prohibited from using 1px solid borders for sectioning. 
Boundaries must be defined solely through background color shifts. Use `surface-container-low` for a section sitting on a `surface` background. This creates a seamless, modern flow that feels organic rather than mechanical.

### Surface Hierarchy & Nesting
Treat the UI as physical layers of fine stationery.
- **Base Layer:** `surface` (#fbf8ff)
- **Secondary Sectioning:** `surface-container-low` (#f3f2ff)
- **Active Cards/Modules:** `surface-container-lowest` (#ffffff)
By nesting a `lowest` (pure white) card inside a `low` (tinted grey/blue) section, you achieve depth through value contrast alone, eliminating the need for heavy shadows.

### The Glass & Gradient Rule
For floating elements (modals, persistent navigation, or AI hint bubbles), use **Glassmorphism**:
- **Background:** `surface` at 70% opacity.
- **Backdrop-blur:** 12px to 20px.
- **Signature Texture:** Use a subtle linear gradient (e.g., `primary` to `primary-container`) on Hero CTAs to provide a "soulful" glow that feels high-end and custom-coded.

---

## 3. Typography: The Editorial Voice
Our typography pairing balances the geometric authority of **Lexend** with the functional precision of **Inter**.

*   **Display & Headlines (Lexend):** Used for "Big Moments"—score reveals, module titles, and welcome screens. Lexend’s open counters provide a modern, premium academic feel.
*   **Body & Titles (Inter):** Used for the "Work"—reading passages, questions, and feedback. Inter is selected for its high x-height and readability in long-form academic content.

**Key Scales:**
- **Display-lg (3.5rem):** Reserved for celebratory milestones.
- **Headline-sm (1.5rem):** The standard for module headers.
- **Body-md (0.875rem):** The workhorse for IELTS reading passages, optimized with a 1.6 line-height for focus.

---

## 4. Elevation & Depth
We convey importance through **Tonal Layering** rather than structural scaffolding.

*   **The Layering Principle:** Stack `surface-container` tiers. A `surface-container-highest` element should only be used for the most critical interactive components (like an AI chat input) to make it "pop" against the rest of the layout.
*   **Ambient Shadows:** If an element must float (e.g., a "Check Answer" button), use an extra-diffused shadow: `box-shadow: 0 12px 32px rgba(0, 21, 82, 0.06);`. The shadow is tinted with the `on-surface` blue to maintain tonal harmony.
*   **The "Ghost Border" Fallback:** If accessibility requires a stroke (e.g., in high-contrast modes), use the `outline-variant` token at **15% opacity**. Never use 100% opaque borders.

---

## 5. Components

### Buttons
- **Primary:** Gradient fill (`primary` to `primary-container`), `md` (12px) rounded corners. White text. No border.
- **Secondary:** `surface-container-high` background with `primary` text. This feels more integrated than a ghost button.
- **Tertiary:** Text-only with an underline on hover, using `label-md` styling.

### Input Fields
- **Default State:** `surface-container-lowest` background with a "Ghost Border" (15% `outline-variant`).
- **Focus State:** Increase border opacity to 40% and add a subtle `primary_fixed` outer glow.
- **Error State:** Use `error` text and a background shift to `error_container` (10% opacity).

### Cards & Progress
- **The IELTS Passage Card:** Never use dividers between paragraphs. Use `1.5rem` vertical spacing.
- **Progress Chips:** Use `secondary_container` (#4bfc61) with `on_secondary_container` (#00711d) for "Correct" states. The vibrant emerald indicates progress without being jarring.

### AI Assistant (The "Curator" Bubble)
- Use the **Glassmorphism** rule. A floating pill with a 20px blur and `surface_bright` background. It should feel like it's hovering over the academic content, not part of it.

---

## 6. Do's and Don'ts

### Do
- **Do** use `Lexend` for all numerical data (scores, timers, percentages) to give them a distinct, premium look.
- **Do** leverage wide margins (at least 24px on mobile, 80px+ on desktop) to allow the "Academic Curator" vibe to breathe.
- **Do** use `surface-dim` for "disabled" states instead of grey, keeping the UI within the blue tonal family.

### Don't
- **Don't** use black (#000000) for text. Use `on_surface` (#001552) to maintain the "Deep Blue" trust profile.
- **Don't** use default 8px rounded corners for everything. Mix `md` (12px) for cards and `full` (9999px) for buttons/chips to create a dynamic visual rhythm.
- **Don't** use horizontal divider lines to separate list items. Use a 4px background shift (`surface` to `surface-container-low`) on alternating items.

---

## 7. Token Reference Summary

| Token | Value | Role |
| :--- | :--- | :--- |
| **Primary** | #0057c2 | Trust, Brand, Primary Actions |
| **Secondary** | #006e1c | Success, Progress, AI Feedback |
| **Surface** | #fbf8ff | Base App Background |
| **Surface-Container-Low** | #f3f2ff | Content Sectioning (The "No-Line" Rule) |
| **Radius-md** | 0.75rem (12px) | Standard Card/Input Rounding |
| **Radius-xl** | 1.5rem (24px) | Hero Section / Large Container Rounding |