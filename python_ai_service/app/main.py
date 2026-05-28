import json
import os
import re
import time
import asyncio
import base64
from typing import List, Optional

import google.generativeai as genai
import requests
from fastapi import FastAPI, Header, HTTPException, WebSocket, WebSocketDisconnect
from pydantic import BaseModel, Field


class WritingEvaluateRequest(BaseModel):
    essay_text: str = Field(min_length=50, max_length=10000)
    task_type: int = Field(ge=1, le=2)
    prompt: Optional[str] = None


class WritingEvaluateResponse(BaseModel):
    overall_band: float
    task_achievement: float
    coherence_cohesion: float
    lexical_resource: float
    grammatical_range: float
    feedback: str
    strengths: List[str] = []
    improvements: List[str] = []


class SpeakingEvaluateRequest(BaseModel):
    transcript: str = Field(min_length=1, max_length=5000)
    topic: str = Field(min_length=1, max_length=500)


class SpeakingEvaluateResponse(BaseModel):
    overall_band: float
    fluency: float
    pronunciation: float
    lexical_resource: float
    grammatical_range: float
    feedback: str
    strengths: List[str] = []
    improvements: List[str] = []


class SpeakingWsMessage(BaseModel):
    type: str
    text: Optional[str] = None
    history: Optional[List[dict]] = None
    part: Optional[int] = 1
    band_estimate: Optional[float] = 5.5
    topic: Optional[str] = None
    force_part: Optional[int] = None
    avatar_fallback_url: Optional[str] = None


STOP_WORDS = {
    "the", "a", "an", "and", "or", "to", "for", "of", "in", "on", "at", "is", "are", "was", "were", "be",
    "about", "with", "that", "this", "it", "as", "by", "from", "you", "your", "i", "my", "we", "our"
}

SAFETY_BLOCK_KEYWORDS = {
    "bomb", "kill", "suicide", "weapon", "terror", "extremist", "rape", "abuse", "hate", "racist", "sex with",
    "nude", "explicit", "self-harm", "how to hack", "credit card fraud", "drugs"
}


def tokenize_keywords(text: str) -> set[str]:
    words = re.findall(r"[a-zA-Z']+", (text or "").lower())
    return {w for w in words if len(w) > 2 and w not in STOP_WORDS}


def has_enough_topic_signal(candidate_text: str) -> bool:
    words = re.findall(r"[a-zA-Z']+", (candidate_text or "").lower())
    meaningful = [w for w in words if len(w) > 2 and w not in STOP_WORDS]
    return len(meaningful) >= 3 and len((candidate_text or "").strip()) >= 18


def topic_relevance_score(topic: str, candidate_text: str) -> float:
    topic_terms = tokenize_keywords(topic)
    candidate_terms = tokenize_keywords(candidate_text)
    if not topic_terms or not candidate_terms:
        return 1.0
    overlap = len(topic_terms.intersection(candidate_terms))
    return overlap / max(1, len(topic_terms))


def is_unsafe_content(text: str) -> bool:
    lowered = (text or "").lower()
    return any(keyword in lowered for keyword in SAFETY_BLOCK_KEYWORDS)


def check_topic_relevance_gemini(topic: str, candidate_text: str) -> bool:
    global_engine = globals().get("engine")
    if not global_engine or not global_engine.enabled:
        return True

    prompt = f"""
        Analyze the candidate's speaking response for relevance to the IELTS topic.

        IELTS Topic: "{topic}"
        Candidate Speaking Response: "{candidate_text}"

        Determine if the candidate is attempting to speak about the topic or answer the question (relevance is broad, e.g. talking about a journey to Paris is highly relevant to a journey topic).
        Only return "no" if the candidate is completely off-topic, speaking about something entirely unrelated, or intentionally ignoring the prompt. Otherwise return "yes".

        Answer with exactly "yes" or "no".
""".strip()

    try:
        response = global_engine._model.generate_content(
            prompt,
            generation_config={"temperature": 0.0, "max_output_tokens": 5},
            safety_settings=[]
        )
        result = getattr(response, "text", "yes").strip().lower()
        return "no" not in result
    except Exception as e:
        print(f"Error checking topic relevance with Gemini: {e}")
        return True


def evaluate_topic_guard(topic: str, candidate_text: str, warning_count: int) -> tuple[str, str, int]:
    if is_unsafe_content(candidate_text):
        return (
            "terminated",
            "Session has been terminated due to unsafe content. Please restart and keep language appropriate for IELTS practice.",
            warning_count + 1,
        )

    if not has_enough_topic_signal(candidate_text):
        return ("ok", "collecting_more_speech", warning_count)

    score = topic_relevance_score(topic, candidate_text)
    if score < 0.08:
        is_relevant = check_topic_relevance_gemini(topic, candidate_text)
        if not is_relevant:
            new_warnings = warning_count + 1
            if new_warnings >= 3:
                return (
                    "terminated",
                    "Session ended because your responses repeatedly moved away from the IELTS topic. Please restart and stay focused on the prompt.",
                    new_warnings,
                )
            return (
                "warning",
                "Please stay on the IELTS topic and answer the current question directly.",
                new_warnings,
            )

    return ("ok", "on-topic", warning_count)


def clean_json_response(text: str) -> str:
    value = text.strip()
    if value.startswith("```json"):
        value = value[7:]
    elif value.startswith("```"):
        value = value[3:]
    if value.endswith("```"):
        value = value[:-3]
    return value.strip()


def clamp_band(value: float) -> float:
    return max(0.0, min(9.0, round(float(value), 1)))


def decode_base64_secret(value: str) -> str:
    raw = (value or "").strip()
    if not raw:
        return ""

    missing_padding = len(raw) % 4
    if missing_padding:
        raw += "=" * (4 - missing_padding)

    try:
        return base64.b64decode(raw).decode("utf-8").strip()
    except Exception:
        return ""


def read_secret_env(primary_name: str, base64_name: str) -> str:
    direct = os.getenv(primary_name, "").strip()
    if direct:
        return direct
    return decode_base64_secret(os.getenv(base64_name, ""))


def to_writing_response(payload: dict) -> WritingEvaluateResponse:
    return WritingEvaluateResponse(
        overall_band=clamp_band(payload.get("overallBand", 6.0)),
        task_achievement=clamp_band(payload.get("taskAchievement", 6.0)),
        coherence_cohesion=clamp_band(payload.get("coherenceCohesion", 6.0)),
        lexical_resource=clamp_band(payload.get("lexicalResource", 6.0)),
        grammatical_range=clamp_band(payload.get("grammaticalRange", 6.0)),
        feedback=str(payload.get("feedback", "No feedback available.")),
        strengths=[str(x) for x in payload.get("strengths", [])][:8],
        improvements=[str(x) for x in payload.get("improvements", [])][:8],
    )


def to_speaking_response(payload: dict) -> SpeakingEvaluateResponse:
    return SpeakingEvaluateResponse(
        overall_band=clamp_band(payload.get("overallBand", 6.0)),
        fluency=clamp_band(payload.get("fluency", 6.0)),
        pronunciation=clamp_band(payload.get("pronunciation", 6.0)),
        lexical_resource=clamp_band(payload.get("lexicalResource", 6.0)),
        grammatical_range=clamp_band(payload.get("grammaticalRange", 6.0)),
        feedback=str(payload.get("feedback", "No feedback available.")),
        strengths=[str(x) for x in payload.get("strengths", [])][:8],
        improvements=[str(x) for x in payload.get("improvements", [])][:8],
    )


def fallback_writing(task_type: int) -> WritingEvaluateResponse:
    min_words = 150 if task_type == 1 else 250
    return WritingEvaluateResponse(
        overall_band=6.0,
        task_achievement=6.0,
        coherence_cohesion=6.0,
        lexical_resource=6.0,
        grammatical_range=5.5,
        feedback=f"Your response is understandable. Focus on clearer paragraph progression and stronger lexical variety. Ensure you fully meet the task requirement (>= {min_words} words).",
        strengths=["Clear attempt to address the prompt", "Basic paragraphing present"],
        improvements=["Use more topic-specific vocabulary", "Add more complex sentence structures"],
    )


def fallback_speaking() -> SpeakingEvaluateResponse:
    return SpeakingEvaluateResponse(
        overall_band=6.0,
        fluency=6.0,
        pronunciation=5.5,
        lexical_resource=6.0,
        grammatical_range=6.0,
        feedback="Good effort. Speak in longer turns, reduce hesitation, and improve pronunciation of multisyllabic words.",
        strengths=["Ideas are understandable", "Some natural expressions used"],
        improvements=["Reduce filler words", "Develop answers with reasons and examples"],
    )


def speaking_system_prompt(part: int) -> str:
    base = (
        "You are a certified IELTS Speaking Examiner. "
        "Follow IELTS Part 1, Part 2, Part 3 format. "
        "Ask one question at a time. "
        "Before each new question, add one short transition sentence that sounds natural (for example: brief acknowledgement, gentle challenge, or contrast). "
        "Do not provide band score or explicit grading feedback during the test. "
        "Keep natural spoken English, professional and friendly."
    )
    style = (
        " Output format rules: maximum 2 sentences total; sentence 1 is the transition line; sentence 2 is exactly one question. "
        "No markdown, no bullet points unless Part 2 cue card is required."
    )
    if part == 2:
        return base + " You are in Part 2 now: provide a cue card with topic, 3 bullets, and 1 final question." + style
    if part == 3:
        return base + " You are in Part 3 now: ask abstract, analytical follow-up questions." + style
    return base + " You are in Part 1 now: short personal intro/interview questions." + style


def fallback_next_question(part: int, previous_count: int) -> str:
    if part == 1:
        defaults = [
            "Thank you for your introduction. Could you tell me your full name, please?",
            "That's clear, thank you. Do you work or are you a student?",
            "Interesting. What do you usually do in your free time?"
        ]
        return defaults[min(previous_count, len(defaults) - 1)]
    if part == 2:
        return (
            "That sounds like a useful starting point. Describe a memorable journey you had.\n"
            "- where you went\n"
            "- who you went with\n"
            "- what happened during the trip\n"
            "and explain why it was memorable.\n"
            "You have 1 minute to prepare. You can start speaking now."
        )
    return "You raised an interesting point. Do you think technology has changed the way people travel? Why or why not?"


class GeminiEngine:
    def __init__(self) -> None:
        self.api_key = os.getenv("GEMINI_API_KEY", "").strip()
        self.model_name = os.getenv("GEMINI_MODEL", "gemini-2.5-flash").strip()
        self._model = None
        if self.api_key:
            genai.configure(api_key=self.api_key)
            self._model = genai.GenerativeModel(self.model_name)

    @property
    def enabled(self) -> bool:
        return self._model is not None

    def generate_json(self, prompt: str, system_instruction: str) -> dict:
        if not self._model:
            raise RuntimeError("Gemini API key is not configured")
        response = self._model.generate_content(
            prompt,
            generation_config={"temperature": 0.2},
            safety_settings=[],
            system_instruction=system_instruction,
        )
        text = getattr(response, "text", None) or ""
        if not text:
            raise RuntimeError("Empty Gemini response")
        return json.loads(clean_json_response(text))

    def stream_text(self, prompt: str, system_instruction: str):
        if not self._model:
            raise RuntimeError("Gemini API key is not configured")
        response = self._model.generate_content(
            prompt,
            generation_config={"temperature": 0.4},
            safety_settings=[],
            system_instruction=system_instruction,
            stream=True,
        )
        for chunk in response:
            text = getattr(chunk, "text", None) or ""
            if text:
                yield text


def normalize_public_media_url(url: Optional[str]) -> Optional[str]:
    value = (url or "").strip()
    if not value:
        return None

    file_match = re.search(r"drive\.google\.com/file/d/([a-zA-Z0-9_-]+)", value)
    if file_match:
        file_id = file_match.group(1)
        return f"https://drive.google.com/uc?export=download&id={file_id}"

    id_match = re.search(r"[?&]id=([a-zA-Z0-9_-]+)", value)
    if "drive.google.com" in value and id_match:
        file_id = id_match.group(1)
        return f"https://drive.google.com/uc?export=download&id={file_id}"

    return value


class AvatarEngine:
    def __init__(self) -> None:
        self.did_api_key = read_secret_env("DID_API_KEY", "DID_API_KEY_B64")
        self.did_source_url = normalize_public_media_url(os.getenv("DID_SOURCE_URL", "")) or ""
        self.did_avatar_url = normalize_public_media_url(os.getenv("DID_AVATAR_URL", "")) or ""
        self.did_base_url = os.getenv("DID_BASE_URL", "https://api.d-id.com").strip().rstrip("/")

    @property
    def enabled(self) -> bool:
        return bool(self.did_api_key and self.did_source_url)

    async def create_video(self, text: str, fallback_url: Optional[str] = None) -> tuple[Optional[str], str]:
        normalized_fallback = normalize_public_media_url(fallback_url)
        if normalized_fallback:
            return normalized_fallback, "fallback"

        if self.enabled and text.strip():
            try:
                return await asyncio.to_thread(self._create_did_video_blocking, text.strip())
            except Exception:
                pass

        if self.did_avatar_url:
            return self.did_avatar_url, "fallback-env"

        if not text.strip():
            return None, "empty-text"
        if not self.enabled:
            return None, "not-configured"

        return None, "failed"

    def _create_did_video_blocking(self, text: str) -> tuple[Optional[str], str]:
        headers = {
            "Authorization": f"Basic {self.did_api_key}",
            "Content-Type": "application/json",
        }
        payload = {
            "script": {
                "type": "text",
                "input": text,
            },
            "source_url": self.did_source_url,
        }
        create_res = requests.post(f"{self.did_base_url}/talks", json=payload, headers=headers, timeout=20)
        create_res.raise_for_status()
        talk_id = create_res.json().get("id")
        if not talk_id:
            return None, "failed"

        for _ in range(12):
            poll_res = requests.get(f"{self.did_base_url}/talks/{talk_id}", headers=headers, timeout=20)
            poll_res.raise_for_status()
            data = poll_res.json()
            status = str(data.get("status", "")).lower()
            result_url = data.get("result_url")
            if result_url:
                return str(result_url), "provider"
            if status in {"error", "failed"}:
                return None, "failed"
            time.sleep(1)
        return None, "timeout"


app = FastAPI(title="WebIeltsFree Python AI Service", version="1.0.0")
engine = GeminiEngine()
avatar_engine = AvatarEngine()
python_api_key = os.getenv("PYTHON_AI_API_KEY", "").strip()


def validate_api_key(header_key: Optional[str]) -> None:
    if not python_api_key:
        return
    if header_key != python_api_key:
        raise HTTPException(status_code=401, detail="Invalid Python AI API key")


@app.get("/health")
def health():
    return {"ok": True, "gemini_enabled": engine.enabled}


@app.post("/v1/writing/evaluate", response_model=WritingEvaluateResponse)
def evaluate_writing(payload: WritingEvaluateRequest, x_api_key: Optional[str] = Header(default=None)):
    validate_api_key(x_api_key)

    if not engine.enabled:
        return fallback_writing(payload.task_type)

    task_type_label = "Task 1 (Report)" if payload.task_type == 1 else "Task 2 (Essay)"
    system_instruction = (
        "You are a certified IELTS Writing examiner. "
        "Score strictly by IELTS descriptors and output only valid JSON."
    )
    prompt = f"""
Evaluate IELTS Writing {task_type_label}.

Prompt:
{payload.prompt or "N/A"}

Essay:
{payload.essay_text}

Return JSON only:
{{
  "overallBand": 6.5,
  "taskAchievement": 6.5,
  "coherenceCohesion": 6.0,
  "lexicalResource": 6.5,
  "grammaticalRange": 6.0,
  "feedback": "string",
  "strengths": ["string", "string"],
  "improvements": ["string", "string"]
}}
""".strip()

    try:
        raw = engine.generate_json(prompt, system_instruction)
        return to_writing_response(raw)
    except Exception:
        return fallback_writing(payload.task_type)


@app.post("/v1/speaking/evaluate", response_model=SpeakingEvaluateResponse)
def evaluate_speaking(payload: SpeakingEvaluateRequest, x_api_key: Optional[str] = Header(default=None)):
    validate_api_key(x_api_key)

    if not engine.enabled:
        return fallback_speaking()

    system_instruction = (
        "You are a certified IELTS Speaking examiner. "
        "Score strictly by IELTS descriptors and output only valid JSON."
    )
    prompt = f"""
Evaluate IELTS Speaking response.

Topic: {payload.topic}
Transcript:
{payload.transcript}

Return JSON only:
{{
  "overallBand": 6.5,
  "fluency": 6.5,
  "pronunciation": 6.0,
  "lexicalResource": 6.5,
  "grammaticalRange": 6.0,
  "feedback": "string",
  "strengths": ["string", "string"],
  "improvements": ["string", "string"]
}}
""".strip()

    try:
        raw = engine.generate_json(prompt, system_instruction)
        return to_speaking_response(raw)
    except Exception:
        return fallback_speaking()


@app.websocket("/ws/speaking")
async def speaking_ws(ws: WebSocket):
    await ws.accept()
    conversation_history: List[dict] = []
    candidate_turn_count = 0
    current_part = 1
    band_estimate = 5.5
    active_topic = "General IELTS Speaking"
    warning_count = 0
    fallback_avatar_url = None

    try:
        while True:
            incoming = await ws.receive_json()
            msg = SpeakingWsMessage(**incoming)

            if msg.part is not None:
                current_part = max(1, min(3, int(msg.part)))
            if msg.force_part is not None:
                current_part = max(1, min(3, int(msg.force_part)))
            if msg.band_estimate is not None:
                band_estimate = max(0.0, min(9.0, float(msg.band_estimate)))
            if msg.topic:
                active_topic = msg.topic.strip() or active_topic
            if msg.avatar_fallback_url is not None:
                fallback_avatar_url = msg.avatar_fallback_url.strip() or None

            if msg.type == "reset":
                conversation_history = []
                candidate_turn_count = 0
                current_part = 1
                band_estimate = 5.5
                warning_count = 0
                await ws.send_json({"type": "state", "part": current_part, "status": "reset"})
                continue

            if msg.type == "candidate_live":
                # Passive live chunks are advisory only; topic guard is enforced on explicit user_answer.
                await ws.send_json({"type": "topic_ok", "warnings": warning_count, "part": current_part, "mode": "passive"})
                continue

            if msg.type == "user_answer" and msg.text:
                if is_unsafe_content(msg.text):
                    warning_count += 1
                    await ws.send_json({
                        "type": "terminated",
                        "message": "Session has been terminated due to unsafe content. Please restart and keep language appropriate for IELTS practice.",
                        "warnings": warning_count,
                        "part": current_part
                    })
                    await ws.close(code=1008)
                    break

                candidate_turn_count += 1
                enforce_topic_guard = not (current_part == 1 and candidate_turn_count <= 2)
                if enforce_topic_guard:
                    verdict, message, warning_count = evaluate_topic_guard(active_topic, msg.text, warning_count)
                    if verdict == "warning":
                        await ws.send_json({"type": "warning", "message": message, "warnings": warning_count, "part": current_part})
                        continue
                    if verdict == "terminated":
                        await ws.send_json({"type": "terminated", "message": message, "warnings": warning_count, "part": current_part})
                        await ws.close(code=1008)
                        break
                else:
                    await ws.send_json({"type": "topic_ok", "warnings": warning_count, "part": current_part, "mode": "grace"})

                conversation_history.append({"role": "candidate", "content": msg.text.strip()})

                # Count the actual candidate turns submitted in total
                candidate_messages = [m for m in conversation_history if m["role"] == "candidate"]
                total_candidate_turns = len(candidate_messages)

                if current_part == 1:
                    if total_candidate_turns >= 3:
                        current_part = 2
                elif current_part == 2:
                    current_part = 3

                system_instruction = speaking_system_prompt(current_part)
                user_prompt = (
                    f"Current IELTS part: {current_part}\n"
                    f"Candidate band estimate: {band_estimate}\n"
                    f"Topic: {active_topic}\n"
                    f"Conversation history:\n{json.dumps(conversation_history[-12:], ensure_ascii=False)}\n"
                    "Generate examiner response in max 2 sentences: first a short transition line reacting naturally to candidate's answer, then exactly one next question."
                )

                full_text = ""
                prefetched_part3 = None
                if engine.enabled:
                    try:
                        for token in engine.stream_text(user_prompt, system_instruction):
                            full_text += token
                            await ws.send_json({"type": "stream", "text": token, "part": current_part})
                        if current_part == 2:
                            prefetch_prompt = (
                                f"You are now preparing IELTS Speaking Part 3. Topic: {active_topic}. "
                                "Generate exactly 3 short analytical questions, JSON only as "
                                '{"questions":["q1","q2","q3"]}.'
                            )
                            try:
                                prefetched_part3 = engine.generate_json(
                                    prefetch_prompt,
                                    "Return valid JSON only. No markdown."
                                )
                            except Exception:
                                prefetched_part3 = None
                    except Exception:
                        full_text = fallback_next_question(current_part, len(conversation_history))
                        await ws.send_json({"type": "stream", "text": full_text, "part": current_part})
                else:
                    full_text = fallback_next_question(current_part, len(conversation_history))
                    await ws.send_json({"type": "stream", "text": full_text, "part": current_part})

                conversation_history.append({"role": "examiner", "content": full_text.strip()})
                avatar_video_url, avatar_status = await avatar_engine.create_video(full_text, fallback_avatar_url)
                if avatar_video_url:
                    await ws.send_json({"type": "avatar", "video_url": avatar_video_url, "status": avatar_status, "part": current_part})
                await ws.send_json({"type": "done", "part": current_part, "avatar_video_url": avatar_video_url, "avatar_status": avatar_status})
                if prefetched_part3 and isinstance(prefetched_part3, dict):
                    questions = prefetched_part3.get("questions") or []
                    if isinstance(questions, list) and questions:
                        await ws.send_json({"type": "prefetch_part3", "questions": [str(x) for x in questions][:3]})
                continue

            if msg.type == "start":
                opener = "Good morning. This is the IELTS speaking test. Could you tell me your full name, please?"
                conversation_history = [{"role": "examiner", "content": opener}]
                candidate_turn_count = 0
                warning_count = 0
                await ws.send_json({"type": "stream", "text": opener, "part": 1})
                avatar_video_url, avatar_status = await avatar_engine.create_video(opener, fallback_avatar_url)
                if avatar_video_url:
                    await ws.send_json({"type": "avatar", "video_url": avatar_video_url, "status": avatar_status, "part": 1})
                await ws.send_json({"type": "done", "part": 1, "avatar_video_url": avatar_video_url, "avatar_status": avatar_status})
                continue

            if msg.type == "interrupt":
                await ws.send_json({"type": "interrupted", "part": current_part})
                continue

            await ws.send_json({"type": "error", "message": "Unsupported message type"})
    except WebSocketDisconnect:
        return

