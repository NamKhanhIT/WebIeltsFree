# Python AI Service (Speaking + Writing)

## Run locally

Quick start with script (Windows PowerShell):

- `./start_python_ai.ps1`
- Skip reinstall dependencies: `./start_python_ai.ps1 -SkipInstall`
- Custom port: `./start_python_ai.ps1 -Port 8010`
- Force recreate virtual env: `./start_python_ai.ps1 -RecreateVenv`

The startup script automatically prefers Python 3.12/3.11/3.10 (if installed)
to reduce native build issues on very new interpreters.

1. Create venv and install deps:
   - `python -m venv .venv`
   - `.venv\Scripts\activate`
   - `pip install -r requirements.txt`

Recommended Python version: 3.10-3.12.

2. Set environment variables:
   - `GEMINI_API_KEY` (optional, fallback mode works without it)
   - `GEMINI_MODEL` (optional, default `gemini-2.5-flash`)
   - `PYTHON_AI_API_KEY` (optional, for ASP.NET header auth)
   - `DID_API_KEY` (optional, for D-ID avatar video generation)
   - `DID_API_KEY_B64` (optional, Base64 version of DID_API_KEY, used when DID_API_KEY is not set)
   - `DID_SOURCE_URL` (optional, public image URL used by D-ID as avatar face)
   - `DID_AVATAR_URL` (optional, default fallback avatar video URL used when provider is unavailable)
   - `DID_BASE_URL` (optional, default `https://api.d-id.com`)

3. Start service:
   - `uvicorn app.main:app --host 0.0.0.0 --port 8000 --reload`

## Troubleshooting

If you see:

- Building wheel for pydantic-core ...
- linker link.exe not found

Then run:

- `./start_python_ai.ps1 -RecreateVenv`

If it still fails, install Python 3.12 and run again.

When Python service is down, ASP.NET websocket proxy `/ws/speaking` will log
"Failed to connect to Python speaking websocket at ws://localhost:8000/ws/speaking".
Bring Python AI service up first, then refresh the speaking page.

## Endpoints

- `GET /health`
- `POST /v1/writing/evaluate`
- `POST /v1/speaking/evaluate`
- `WS /ws/speaking`

## Avatar setup (D-ID)

1. Create account at D-ID and get API key.
2. Upload one public avatar image (JPG/PNG) to storage/CDN and copy direct URL.
3. Set environment:
    - `DID_API_KEY=<your base64 key payload>`
   - or `DID_API_KEY_B64=<base64-encoded DID_API_KEY>`
    - `DID_SOURCE_URL=https://.../avatar.jpg`
4. Restart Python service.

PowerShell example to encode API key to Base64:

- `[Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes("YOUR_DID_API_KEY"))`

Set Base64 key in environment:

- `setx DID_API_KEY_B64 "<encoded_value>"`

Note: Base64 is obfuscation only. Real security comes from not committing secrets and rotating keys if exposed.

### How to get DID_SOURCE_URL

`DID_SOURCE_URL` must be a direct public URL of an image (`.jpg`/`.png`) used as avatar face.

Recommended flow:

1. Prepare an avatar image (front-facing portrait, clear face, good light).
2. Upload image to public storage (S3, Cloudinary, GitHub raw, CDN, etc.).
3. Open the image URL in browser to verify it returns the image directly.
4. Use that direct URL as `DID_SOURCE_URL`.

Important:

- D-ID agent share links (for example `https://studio.d-id.com/agents/share?...`) are not `source_url` image links.
- `source_url` must point directly to an image file, not a webpage.

If D-ID is not configured, the frontend can still play a fallback MP4 URL from the Speaking page.
You can also set `DID_AVATAR_URL` to provide a default fallback URL (Google Drive public links are normalized automatically).

## Realtime topic/safety guard

- Every candidate final transcript chunk is checked by topic relevance and safety keywords.
- Off-topic behavior:
   - warning at first and second violation
   - automatic session termination at third repeated off-topic violation
- Unsafe content triggers immediate termination.

## Notes

- If Gemini is unavailable, service returns deterministic fallback evaluations.
- ASP.NET Core keeps the same existing API endpoints and calls this service internally.
