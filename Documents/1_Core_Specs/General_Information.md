# WebIeltsFree - General Information & Architecture

## 1. Project Overview
WebIeltsFree is a comprehensive, free IELTS learning platform powered by AI. The platform provides adaptive learning, personalized study plans, interactive AI-driven practice (Speaking, Writing, Reading, Listening), and human teacher support to help students achieve their target IELTS bands.

## 2. Technology Stack
- **Backend:** ASP.NET Core 8 (MVC + Web API)
- **Frontend:** ASP.NET Core MVC Razor Views (with Next.js frontend planned for future decoupling)
- **Primary Database:** MySQL (User data, content, progress, transactions)
- **NoSQL Database:** MongoDB (Behavior logs, detailed AI analysis, conversation history)
- **Vector Database:** Pinecone (Semantic search, RAG, content recommendations)
- **Caching & Rate Limiting:** Redis
- **AI Integration:** Google Gemini API (gemini-2.5-flash) for grading, feedback, and chatbot / Python FastAPI service for specific evaluations.
- **Infrastructure:** Docker support, deployed across free tiers (Render for backend, Vercel for frontend, Aiven for MySQL, MongoDB Atlas)

## 3. Core Features
- **Adaptive, Personalized Learning:** Entrance placement tests generate a customized study plan. The system tracks progress and dynamically injects targeted practice (e.g., using spaced repetition).
- **Comprehensive Practice & Feedback:** Thousands of questions across all 4 skills. AI grades Speaking and Writing submissions in real-time, providing tentative band scores and detailed feedback (Grammar, Fluency, Coherence, Lexical Resource).
- **Interactive Speaking Practice (AI Examiner):** Real-time speaking practice with an AI role-player that adapts to the user's level and evaluates responses against IELTS descriptors.
- **AI-Powered Q&A Chatbot:** A RAG-based chatbot using Pinecone to answer grammar questions, site navigation, and IELTS procedures based on the platform's knowledge base.
- **Teacher/Instructor Support:** A dedicated academic role for human oversight, grade disputes, direct messaging, and content management.

## 4. User Roles
1. **Student:** Can take courses, practice skills, submit assignments, dispute grades, and message teachers.
2. **Teacher / Instructor:** Academic authority. Can review grade disputes, override AI scores, message students for support, and manage content (soft-delete only). Cannot access admin settings.
3. **Admin:** System operator. Manages user accounts (ban/unban), role assignments, platform analytics, system settings, and hard-deletions.

## 5. Database Architecture
### MySQL (Relational Data - 41 Tables)
- **User Domain:** `tb_users`, `tb_user_profiles`, `tb_user_sessions`, etc.
- **Learning Domain:** `tb_courses`, `tb_modules`, `tb_lessons`, `tb_vocabulary`.
- **Test & Practice:** `tb_tests`, `tb_questions`, `tb_user_test_attempts`, `tb_reading_passages`, `tb_listening_materials`, `tb_writing_prompts`, `tb_speaking_topics`.
- **Teacher Domain:** `tb_grade_disputes`, `tb_conversations`, `tb_messages`.
- **System & AI:** `tb_audit_logs`, `tb_notifications`, `tb_ai_roadmaps`.

### MongoDB (Unstructured & High-Volume Data)
- `user_behavior_logs`, `learning_patterns`, `ai_predictions`
- `speaking_sessions_detail` (transcripts, detailed fluency analysis)
- `writing_analyses_detail` (paragraph breakdowns, grammar errors)
- `ai_conversations` (chat history)

### Pinecone (Vector Search)
- **Namespaces:** `lessons`, `vocabulary`, `reading_passages`, `writing_samples`, `speaking_topics`, `knowledge_base`
- **Dimensions:** 768 (text-embedding-3-small) or 1536 (text-embedding-3-large).
