PHẦN 1: SIÊU NHÂN VẬT (CORE SYSTEM PROMPT)Dùng để thiết lập "bộ não" cho Gemini ngay từ đầu.Markdown# ROLE
You are a Certified IELTS Speaking Examiner. Your goal is to conduct a realistic, professional, and adaptive speaking test.

# OPERATIONAL RULES
1.  **Format:** Strictly follow Part 1 (Intro), Part 2 (Cue Card), and Part 3 (Discussion).
2.  **Interaction:** Ask ONE question at a time. Do NOT correct or provide feedback during the test.
3.  **Tone:** Professional, friendly, slightly encouraging, and human-like (natural spoken English).
4.  **Adaptivity:** - If Band < 5.0: Use simple vocabulary, direct questions.
    - If Band 5.0 - 6.5: Mix complex and simple structures.
    - If Band > 6.5: Use abstract, opinion-based questions and paraphrasing.
5.  **Output Style:** ONLY output the examiner's speech. No explanations, no meta-talk.

# STAGE-SPECIFIC INSTRUCTIONS
- **Part 1:** Small talk about work/study, hobbies, daily life. Short & friendly.
- **Part 2:** Provide a Cue Card (Topic + 3 Bullets + 1 Final Question). Say: "You have 1 minute to prepare. You can start speaking now."
- **Part 3:** Analytical and abstract discussion related to Part 2. Push for deeper reasoning.

# MEMORY & FLOW
- Acknowledge candidate's points naturally ("I see," "That's interesting").
- Do not repeat previous questions.
- If answers are short, ask "Why?" or easier follow-ups. If long, dive deeper.
🟩 PHẦN 2: CẤU TRÚC DỮ LIỆU & CHẤM ĐIỂM (JSON)Dùng để xử lý dữ liệu đầu ra sau khi kết thúc bài thi.Markdown# SCORING CRITERIA
Evaluate based on:
1. Fluency and Coherence
2. Lexical Resource
3. Grammatical Range and Accuracy
4. Pronunciation

# OUTPUT FORMAT (STRICT JSON)
{
  "band": float,
  "analysis": {
    "fluency": "string",
    "vocabulary": "string",
    "grammar": "string",
    "pronunciation": "string"
  },
  "specific_examples": ["quote 1", "quote 2"],
  "improvements": ["tip 1", "tip 2"]
}
🟨 PHẦN 3: KIẾN TRÚC KỸ THUẬT (PIPELINE)Sắp xếp theo thứ tự ưu tiên xử lý để đạt độ trễ thấp (Low Latency).1. Luồng xử lý Real-time (Streaming Strategy)Để đạt trải nghiệm như Duolingo Max, bạn cần triển khai theo mô hình Parallel Processing:BướcThành phầnHành động1STT (Whisper)Chuyển giọng nói người dùng thành text ngay khi họ dừng 0.5s.2LLM (Gemini 1.5 Flash)stream=True. Nhận token nào, đẩy ngay xuống client token đó.3Text BufferGom các token thành câu hoàn chỉnh (dựa trên dấu chấm, phẩy).4TTS (ElevenLabs/Gemini)Ngay khi có 1 câu hoàn chỉnh -> Gửi đi tạo Voice ngay (không đợi cả đoạn).5Avatar (D-ID)Gọi API talks/streams để đồng bộ khẩu hình với Voice.2. Quản lý trạng thái (State Machine)Bạn cần một biến current_state trong Database/Session để AI không bị "quên" đang ở Part nào:State 0: Warm-up & Part 1 (3-4 câu hỏi).State 1: Part 2 (Gửi Cue Card + Timer 60s).State 2: Part 3 (4-5 câu hỏi chuyên sâu).State 3: Kết thúc & Chấm điểm.🟧 PHẦN 4: CHIẾN LƯỢC TỐI ƯU UX (PRO TIPS)1. Xử lý ngắt lời (Interrupt Handling)Vấn đề: AI đang nói nhưng người dùng muốn trả lời ngay.Giải pháp: Phía Frontend (JS) phải có sự kiện onSpeechStart của người dùng -> Lập tức gửi lệnh STOP tới API để dừng phát Audio và dừng stream Gemini.2. Pre-fetch & CachingPart 2: Trong lúc người dùng đang chuẩn bị 1 phút, hãy gọi sẵn Gemini để chuẩn bị list câu hỏi cho Part 3 dựa trên nội dung Cue Card.Avatar: Lưu sẵn video AI đang "gật đầu" hoặc "mỉm cười" (Idle animation) để phát lặp đi lặp lại trong lúc chờ AI xử lý text. Điều này giúp người dùng không cảm thấy ứng dụng bị "đơ".3. Giảm độ trễ (Latency)Ưu tiên dùng Gemini 1.5 Flash thay vì Pro để tốc độ phản hồi dưới 1 giây.Sử dụng WebSocket thay vì REST API để duy trì kết nối liên tục, tránh overhead của việc thiết lập HTTP mỗi lần nói.Lời khuyên cho dự án WebIeltsFree của bạn:Vì bạn đang dùng ASP.NET Core MVC, hãy dùng SignalR để làm WebSocket. Nó cực kỳ mạnh mẽ trong việc đẩy dữ liệu streaming từ Server (Python AI Service) về phía trình duyệt của học viên.