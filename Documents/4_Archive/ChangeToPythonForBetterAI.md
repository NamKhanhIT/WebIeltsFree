I. Build AI IELTS Examiner

1. SYSTEM PROMPT (CORE – QUAN TRỌNG NHẤT)

👉 Dùng cho mọi request



You are a certified IELTS Speaking Examiner.



Your role is to conduct a realistic IELTS Speaking test in a professional, natural, and human-like manner.



STRICT RULES:

- Follow the official IELTS Speaking format (Part 1, Part 2, Part 3).

- Ask ONE question at a time.

- Do NOT give feedback during the test.

- Keep responses short, natural, and conversational.

- Adapt difficulty based on the candidate’s level.

- Encourage the candidate without correcting them.

- Use natural spoken English (not robotic or academic tone).



PERSONALITY:

- Friendly but professional

- Slightly encouraging

- Patient and adaptive



OUTPUT STYLE:

- Only ask the next question or respond naturally.

- Do NOT explain anything.



🎤 2. CONVERSATION PROMPT (REAL-TIME)

🔹 Start Speaking Test

Start an IELTS Speaking test.



Part 1 – Introduction and Interview.



Ask the candidate:

- Their name

- Whether they work or study

- A few simple questions about their daily life



Keep it natural and friendly.

Ask one question at a time.



🔹 Continue Conversation

Continue the IELTS Speaking test.



Current Part: {part}

Candidate level: {band_estimate}



Conversation history:

{history}



INSTRUCTIONS:

- Ask the next question naturally.

- If candidate answers are short → ask easier follow-up.

- If answers are detailed → ask deeper questions.

- Keep IELTS style.

- One question only.



🔹 Part 2 (Cue Card)

You are now in Part 2 of the IELTS Speaking test.



Give the candidate a cue card.



Structure:

- Topic

- 3 bullet points

- 1 final question



Then say:

"You have 1 minute to prepare. You can start speaking now."



🔹 Part 3 (Discussion)

You are now in Part 3 of the IELTS Speaking test.



Ask more abstract and analytical questions related to the Part 2 topic.



Make questions:

- More complex

- Opinion-based

- Require explanation



Ask one question at a time.



🧠 3. SCORING PROMPT (CHẤM ĐIỂM CHUẨN IELTS)

👉 Dùng sau khi kết thúc test



You are a certified IELTS Speaking Examiner.



Evaluate the candidate based on the official IELTS band descriptors.



CRITERIA:

1. Fluency and Coherence

2. Lexical Resource

3. Grammatical Range and Accuracy

4. Pronunciation



TASK:

- Give a band score (0–9)

- Provide detailed feedback for each criterion

- Give specific examples from the transcript

- Suggest improvements



IMPORTANT:

- Be strict but fair (real IELTS standard)

- Do NOT inflate scores



OUTPUT FORMAT (JSON):

{

  "band": 6.5,

  "fluency": "...",

  "vocabulary": "...",

  "grammar": "...",

  "pronunciation": "...",

  "improvement": ["...", "..."]

}



TRANSCRIPT:

{full_conversation}



🧩 4. ADAPTIVE PROMPT (CÁ NHÂN HÓA)

👉 Đây là phần giúp bạn “ăn đứt Duolingo” 🔥



Adjust your speaking questions based on the candidate’s level:



IF band < 5:

- Use simple vocabulary

- Ask short, direct questions

- Avoid abstract topics



IF band 5–6.5:

- Mix simple and slightly complex questions

- Encourage explanation



IF band > 6.5:

- Ask abstract, opinion-based questions

- Push for deeper reasoning

- Use paraphrasing



Always adapt naturally without mentioning levels.



🔥 5. PROMPT COMBO (SỬ DỤNG THỰC TẾ)

Trong Python:





messages = [

    {"role": "system", "content": SYSTEM_PROMPT},

    {"role": "user", "content": conversation_prompt}

]



🎯 6. PROMPT FLOW FULL (REAL SYSTEM)

START →

    Part 1 →

        adaptive Q&A →

    Part 2 →

        cue card →

    Part 3 →

        discussion →

END →

    scoring prompt



🚀 7. NÂNG CẤP PRO (RẤT QUAN TRỌNG)

🔥 1. Memory-aware prompt

Avoid repeating previous questions:

{previous_questions}



🔥 2. Error-detection prompt (ẩn)

Note candidate weaknesses silently for later scoring.



🔥 3. Emotion / human-like

React naturally:

- "That sounds interesting"

- "I see"

- "That's a good point"

II. Build gemini speaking streaming

1. KIẾN TRÚC HOÀN CHỈNH (REAL-TIME)

Browser (Mic + UI + Audio Player)

        ⇅ WebSocket (low latency)

ASP.NET Core MVC (Gateway / Auth / Session)

        ⇅ WebSocket / HTTP

Python AI Service (FastAPI)

   ├── Streaming STT (Whisper / Realtime)

   ├── Conversation Engine (GPT prompts)

   ├── TTS (AI voice)

   ├── Session Manager (state Part 1–2–3)

   └── Scoring Engine (band descriptors)



👉 Điểm mấu chốt:



WebSocket (không dùng HTTP thuần) để giảm delay

Streaming audio (chunk 0.5–1s)

State machine cho IELTS flow

⚙️ 2. CÀI ĐẶT (AI SERVICE)



pip install pip install google-generativeai



GEMINI STREAMING (CORE CODE)

🔹 Basic streaming



import google.generativeai as genai

import os



genai.configure(api_key=os.getenv("GEMINI_API_KEY"))



model = genai.GenerativeModel("gemini-2.5-flash")



def stream_response(prompt):

    response = model.generate_content(

        prompt,

        stream=True

    )



    for chunk in response:

        if chunk.text:

            yield chunk.text



🎤 4. ÁP DỤNG CHO SPEAKING AI

🔹 Streaming examiner



def stream_examiner(history, band):

    prompt = f"""

    You are an IELTS examiner.



    Band: {band}

    Conversation:

    {history}



    Ask next question naturally.

    """



    for chunk in stream_response(prompt):

        yield chunk



🔌 5. KẾT HỢP WEBSOCKET (REALTIME)

FastAPI WebSocket



from fastapi import WebSocket



@app.websocket("/ws/speaking")

async def speaking_ws(ws: WebSocket):

    await ws.accept()



    while True:

        data = await ws.receive_json()



        if data["type"] == "ask":

            history = data["history"]



            for chunk in stream_examiner(history, 5.5):

                await ws.send_json({

                    "type": "stream",

                    "text": chunk

                })



            await ws.send_json({"type": "done"})



🌐 6. FRONTEND STREAM UI



<script>

let ws = new WebSocket("ws://localhost:8000/ws/speaking");



let fullText = "";



ws.onmessage = (event) => {

    const data = JSON.parse(event.data);



    if (data.type === "stream") {

        fullText += data.text;

        document.getElementById("ai").innerText = fullText;

    }



    if (data.type === "done") {

        speak(fullText); // TTS

        fullText = "";

    }

};

</script>



🔊 7. STREAMING + TTS (QUAN TRỌNG)

👉 Cách chuẩn:



❌ Sai:



đợi full text rồi mới đọc

✅ Đúng:

Gemini stream → buffer → TTS từng phần



Ví dụ:



buffer = ""



for chunk in stream_response(prompt):

    buffer += chunk



    if "." in buffer:  # gặp câu hoàn chỉnh

        speak(buffer)

        buffer = ""



⚡ 8. ULTRA REALTIME (LEVEL DUOLINGO)

🔥 Kỹ thuật nâng cao

1. Sentence chunking



phát audio theo câu

2. Interrupt handling

User nói chen → dừng AI



3. Pre-fetch response



# generate trước câu tiếp theo



🧠 9. TỐI ƯU PROMPT CHO STREAMING

👉 Prompt phải:



- Ngắn

- Trực tiếp

- Không lan man



Ví dụ tốt:

Ask ONE short IELTS speaking question.



❌ Tránh:

Explain in detail...



🚀 10. KẾT HỢP FULL PIPELINE

User voice

   ↓

STT

   ↓

Gemini streaming

   ↓

TTS streaming

   ↓

Avatar video



🎯 11. PERFORMANCE TIPS

🔥 Bắt buộc



dùng model:

gemini-1.5-flash (fast)



🔥 Cache



câu hỏi lặp lại



cue card

🔥 Async



chạy TTS song song

⚠️ 12. VẤN ĐỀ THỰC TẾ

❗ Delay vẫn có

👉 fix:





chunk nhỏ



giảm prompt size

❗ Text bị ngắt

👉 fix:





buffer theo câu

🎯 13. KẾT LUẬN

Gemini streaming giúp bạn:

✔ AI nói gần như realtime



✔ Giống Duolingo Max



✔ UX cực mượt



🌐 11. ASP.NET CORE (GATEWAY)

JavaScript (Frontend)



<script>

let ws = new WebSocket("ws://localhost:8000/ws/speaking");



ws.onmessage = (event) => {

    const data = JSON.parse(event.data);



    if (data.type === "question") {

        speak(data.text); // TTS play

    }



    if (data.type === "result") {

        console.log(data.data);

    }

};



function sendAnswer(text) {

    ws.send(JSON.stringify({ type: "answer", text }));

}

</script>



🎤 12. UX GIỐNG DUOLINGO MAX

Flow:



User click “Start Speaking”



AI hỏi (voice)



User trả lời (mic)



AI phản hồi ngay



Lặp 10–15 câu



Kết thúc → band score

⚡ 13. TỐI ƯU REAL-TIME

🔥 Bắt buộc làm



Chunk audio nhỏ (500ms)



Async xử lý



Cache model

🔥 Nâng cấp



Streaming GPT (token-by-token)



Interrupt handling (user nói chen)

🧠 14. NÂNG CẤP LEVEL 

🎭 Avatar



Video AI (D-ID / HeyGen)

🎯 Emotion AI



Detect hesitation

📊 Progress tracking



Lưu band theo thời gian 

III. Build AI avatar with video and voice

1. KIẾN TRÚC AI AVATAR

User (Browser)

   ↓ mic + UI

Frontend (Video Avatar UI)

   ↓ WebSocket

Python AI Service

   ├── Conversation Engine (IELTS examiner)

   ├── TTS (voice)

   └── Avatar Video API

         ↓

Avatar Provider (video AI)

         ↓

Video stream → Browser



👉 Bạn KHÔNG tự render video bằng Python



👉 Mà dùng dịch vụ avatar AI (giống Duolingo)

🎭 2. CÔNG NGHỆ AVATAR (CHỌN 1)

🔥 Option tốt nhất (khuyên dùng)

D-ID → dễ dùng, có API



⚙️ 3. FLOW HOẠT ĐỘNG

Khi AI nói:

Gemini → text

   ↓

TTS → audio

   ↓

Avatar API → video talking

   ↓

Frontend hiển thị



🚀 4. IMPLEMENT (PYTHON)

🔹 1. TTS → audio



def generate_voice(text):

    # dùng Gemini hoặc ElevenLabs

    return "https://audio-url.mp3"



🔹 2. Gọi API Avatar (ví dụ D-ID)



import requests



def create_avatar_video(text):

    url = "https://api.d-id.com/talks"



    payload = {

        "script": {

            "type": "text",

            "input": text

        },

        "source_url": "https://your-avatar-image.jpg"

    }



    headers = {

        "Authorization": "Basic YOUR_API_KEY",

        "Content-Type": "application/json"

    }



    res = requests.post(url, json=payload, headers=headers)

    return res.json()



🔹 3. Poll video result



def get_video_result(talk_id):

    url = f"https://api.d-id.com/talks/{talk_id}"



    res = requests.get(url, headers={

        "Authorization": "Basic YOUR_API_KEY"

    })



    return res.json()



🌐 5. FRONTEND (HIỂN THỊ AVATAR)



<video id="avatar" autoplay></video>



<script>

function playAvatar(videoUrl) {

    const video = document.getElementById("avatar");

    video.src = videoUrl;

}

</script>



🔄 6. TÍCH HỢP VỚI SPEAKING SYSTEM

Trong WebSocket flow:

User trả lời

   ↓

AI generate question

   ↓

→ gửi text về frontend

→ gọi avatar API

   ↓

Frontend nhận video URL

   ↓

Play video + voice



🧠 7. TỐI ƯU GIỐNG DUOLINGO MAX

🔥 1. Streaming response



Không đợi full video



Hiển thị:

text trước



video sau

🔥 2. Pre-generate câu hỏi



next_q = generate_question(...)

preload_avatar(next_q)



👉 giảm delay

🔥 3. Avatar emotion



smile



serious (Part 3)

🎯 8. UX FLOW HOÀN CHỈNH

Start speaking

   ↓

Avatar xuất hiện

   ↓

Avatar hỏi (video)

   ↓

User trả lời (mic)

   ↓

AI phản hồi

   ↓

Loop

   ↓

End → result



⚠️ 9. NHỮNG THỨ KHÓ NHẤT

❗ 1. Delay video

👉 Giải pháp:





preload



async

❗ 2. Sync audio + video

👉 Dùng:





video API có sẵn (D-ID xử lý)

❗ 3. Chi phí

👉 Giải pháp:





chỉ dùng avatar cho câu hỏi



không dùng cho mọi response

🔥 10. NÂNG CẤP PRO

🧠 Memory-aware avatar

"Oh, that's interesting, you mentioned traveling earlier..."



🎭 Multi-avatar



Examiner nữ



Examiner nam 

Tôi muốn đổi toàn bộ AI trong code cũ sang code python để xử lý các thư viện AI tốt hơn. THực hiện theo yêu cầu của tôi như phân tích trên. Hỏi trước khi thực hiện