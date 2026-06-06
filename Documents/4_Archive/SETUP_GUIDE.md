# 🚀 IELTS Learning Platform - Setup Guide

## Mục Lục
1. [AI Setup (Gemini - Miễn phí)](#1-ai-setup-gemini---miễn-phí)
2. [Database Setup (MySQL/phpMyAdmin)](#2-database-setup-mysqlphpmyadmin)
3. [JWT Authentication](#3-jwt-authentication)
4. [Redis Cache](#4-redis-cache)
5. [Docker Deployment](#5-docker-deployment)
6. [MongoDB (Optional)](#6-mongodb-optional)
7. [Vector Database (Optional)](#7-vector-database-optional)

---

## 1. AI Setup (Gemini - Miễn phí)

### Tại sao chọn Gemini?
| AI Service | Free Tier | Ưu điểm | Nhược điểm |
|------------|-----------|---------|------------|
| **Google Gemini** ✅ | 60 req/phút, 1500 req/ngày | Miễn phí, API mạnh, đa ngôn ngữ | Cần Google Cloud |
| OpenAI | $5 credit (hết sau ~1 tháng) | Chất lượng cao | Tốn phí |
| Claude | Không có free API | Tốt cho văn bản dài | Không miễn phí |
| Groq | 30 req/phút | Cực nhanh | Model nhỏ hơn |

**Khuyến nghị: Google Gemini** - Miễn phí, mạnh, phù hợp dự án phi lợi nhuận.

### Bước 1: Lấy Gemini API Key
1. Truy cập: https://makersuite.google.com/app/apikey
2. Đăng nhập Google Account
3. Click "Create API Key"
4. Copy API Key (bắt đầu bằng `AIza...`)

### Bước 2: Cấu hình trong appsettings.json
```json
{
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY_HERE",
    "Model": "gemini-1.5-flash",
    "MaxTokens": 2048
  }
}
```

### Bước 3: Code Integration
Xem file `WebIeltsFree/Models/GeminiService.cs` (sẽ tạo bên dưới)

---

## 2. Database Setup (MySQL/phpMyAdmin)

### Yêu cầu
- XAMPP/WAMP/MAMP với MySQL 8.0+
- phpMyAdmin

### Bước 1: Tạo Database
1. Mở phpMyAdmin: http://localhost/phpmyadmin
2. Tạo database mới tên: `ieltsdb`
3. Chọn Collation: `utf8mb4_unicode_ci`

### Bước 2: Import SQL
1. Chọn database `ieltsdb`
2. Tab "Import" → Choose file → `ieltsdb_new.sql`
3. Click "Go"

### Bước 3: Cấu hình Connection String
Trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=ieltsdb;User=root;Password=;"
  }
}
```

**Lưu ý:**
- Port mặc định XAMPP: `3306` (không phải 3307)
- Nếu dùng MySQL root không có password, để `Password=;`
- Nếu có password: `Password=your_password;`

### Kiểm tra kết nối
```bash
cd WebIeltsFree
dotnet ef dbcontext info
```

---

## 3. JWT Authentication

### JWT là gì?
JSON Web Token - token để xác thực người dùng không cần session.

### Cấu hình trong appsettings.json
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!@#$%",
    "Issuer": "WebIeltsFree",
    "Audience": "WebIeltsFreeUsers",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

### Cách hoạt động
1. User đăng nhập → Server tạo JWT token
2. Token được lưu trong localStorage/cookie của browser
3. Mỗi request gửi kèm token trong header: `Authorization: Bearer <token>`
4. Server verify token và xử lý request

### Test JWT
```bash
# Đăng ký user mới
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test@123!","confirmPassword":"Test@123!","targetBand":6.5}'

# Đăng nhập
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test@123!"}'

# Sử dụng token
curl http://localhost:5000/api/users/profile \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

---

## 4. Redis Cache

### Redis là gì?
In-memory cache giúp tăng tốc ứng dụng bằng cách cache dữ liệu thường xuyên truy cập.

### Cài đặt Redis

**Windows (Docker - Khuyến nghị):**
```bash
docker run -d --name redis -p 6379:6379 redis:7-alpine
```

**Windows (Native - Memurai):**
1. Download: https://www.memurai.com/get-memurai
2. Cài đặt và khởi động service

**Linux/Mac:**
```bash
# Ubuntu
sudo apt install redis-server

# Mac
brew install redis
brew services start redis
```

### Cấu hình trong appsettings.json
```json
{
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "IELTSCache_"
  }
}
```

### Sử dụng trong code (đã có sẵn)
```csharp
// Inject ICacheService
public class MyController : ControllerBase
{
    private readonly ICacheService _cache;
    
    public async Task<ActionResult> GetData()
    {
        var cached = await _cache.GetAsync<MyData>("my_key");
        if (cached != null) return Ok(cached);
        
        var data = await FetchFromDatabase();
        await _cache.SetAsync("my_key", data, TimeSpan.FromMinutes(10));
        return Ok(data);
    }
}
```

### Test Redis
```bash
redis-cli ping
# Response: PONG
```

---

## 5. Docker Deployment

### Cấu trúc files
```
WebIeltsFree/
├── Dockerfile              # Build image
├── docker-compose.yml      # Development
├── docker-compose.prod.yml # Production
├── .env.template           # Environment template
└── .env                    # Your config (create from template)
```

### Bước 1: Tạo .env từ template
```bash
cp .env.template .env
```

### Bước 2: Cấu hình .env
```env
# Database
DB_ROOT_PASSWORD=your_secure_password_123
DB_PASSWORD=your_db_password_456

# JWT (Generate: openssl rand -base64 32)
JWT_SECRET_KEY=K8xYz2mN4pQ7rS9tU1vW3xZ5aB7cD9eF

# AI (Gemini)
GEMINI_API_KEY=AIza...your_gemini_key

# Redis (optional)
REDIS_PASSWORD=your_redis_password

# Environment
ASPNETCORE_ENVIRONMENT=Production
```

### Bước 3: Chạy Development
```bash
# Build và chạy
docker-compose up -d

# Xem logs
docker-compose logs -f web

# Truy cập:
# - Web: http://localhost:5000
# - Swagger: http://localhost:5000/swagger
# - phpMyAdmin: http://localhost:8080 (nếu có)
```

### Bước 4: Chạy Production
```bash
docker-compose -f docker-compose.prod.yml up -d
```

### Commands hữu ích
```bash
# Rebuild image
docker-compose build --no-cache

# Restart service
docker-compose restart web

# Xem resource usage
docker stats

# Xóa tất cả và làm lại
docker-compose down -v
docker-compose up -d --build
```

---

## 6. MongoDB (Optional)

> **Lưu ý:** MongoDB là optional, dùng để lưu AI conversation logs. Có thể bỏ qua nếu không cần.

### Cài đặt (Docker)
```bash
docker run -d --name mongodb -p 27017:27017 mongo:7
```

### Cấu hình
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "IeltsDB"
  }
}
```

### Collections tự động tạo
- `ai_conversations` - Lịch sử chat với AI
- `ai_analytics` - Phân tích hành vi học tập

---

## 7. Vector Database (Optional)

> **Lưu ý:** Vector DB dùng cho RAG chatbot (search semantic). Có thể bỏ qua ban đầu - hệ thống có fallback.

### Option 1: Pinecone (Hosted - Free tier)
1. Đăng ký: https://www.pinecone.io/
2. Tạo index với dimension: 1536
3. Copy API key và Environment

```json
{
  "Pinecone": {
    "ApiKey": "your-pinecone-api-key",
    "Environment": "gcp-starter",
    "IndexUrl": "https://your-index.svc.gcp-starter.pinecone.io"
  }
}
```

### Option 2: Chroma (Self-hosted - Free)
```bash
docker run -d --name chroma -p 8000:8000 chromadb/chroma
```

### Option 3: In-Memory (Default)
Hệ thống đã có implementation in-memory, đủ dùng cho dự án nhỏ.

---

## 📋 Checklist Hoàn Thành

- [ ] Gemini API Key đã có
- [ ] MySQL đã import `ieltsdb_new.sql`
- [ ] `appsettings.json` đã cấu hình
- [ ] `.env` đã tạo (nếu dùng Docker)
- [ ] Test `dotnet run` thành công
- [ ] Test API `/health` trả về OK
- [ ] Test đăng ký/đăng nhập hoạt động

---

## 🆘 Troubleshooting

### Lỗi MySQL Connection
```
Unable to connect to MySQL server
```
**Fix:** Kiểm tra:
- MySQL service đang chạy
- Port đúng (3306 hay 3307)
- Username/password đúng

### Lỗi JWT Invalid Token
```
401 Unauthorized
```
**Fix:** 
- Token hết hạn → đăng nhập lại
- Key trong appsettings phải giống nhau ở mọi nơi

### Lỗi Gemini API
```
API key not valid
```
**Fix:**
- Kiểm tra API key đúng
- Enable Generative Language API trong Google Cloud Console

### Lỗi Docker
```
port is already allocated
```
**Fix:**
```bash
# Tìm process dùng port
netstat -ano | findstr :5000
# Kill process
taskkill /PID <PID> /F
```

---

## 📞 Support

Nếu gặp vấn đề, kiểm tra:
1. File logs: `WebIeltsFree/bin/Debug/net8.0/logs/`
2. Swagger docs: http://localhost:5000/swagger
3. Health check: http://localhost:5000/health
