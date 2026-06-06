# IELTS Learning Platform - Practice APIs

## Listening Practice API

### Base URL
`/api/listening`

### Endpoints

#### 1. Get Listening Practice Set by Difficulty
```
GET /api/listening/practice/{difficulty}
```
- **Parameters**: `difficulty` = band_3_4 | band_5_6 | band_7_8 | band_8_9
- **Returns**: Random listening material with questions (no answers)
- **Response**: `{ material, questions: [], transcript }`

#### 2. Get Specific Material
```
GET /api/listening/material/{materialId}
```
- **Returns**: Listening material with all questions
- **Response**: `{ material, questions: [], transcript }`

#### 3. List All Materials
```
GET /api/listening/materials?difficulty=band_5_6
```
- **Optional Query**: `difficulty`
- **Returns**: Array of available listening materials

#### 4. Submit Answer
```
POST /api/listening/submit
Authorization: Bearer {token}
```
**Request Body:**
```json
{
  "questionId": 1,
  "userAnswer": "opportunity",
  "timeSpentSeconds": 30
}
```
**Response:**
```json
{
  "questionId": 1,
  "isCorrect": true,
  "userAnswer": "opportunity",
  "correctAnswer": "opportunity",
  "explanation": "...",
  "bandTarget": 6.5
}
```

#### 5. Get Practice History
```
GET /api/listening/history?limit=20
Authorization: Bearer {token}
```
- **Returns**: Last 20 listening attempts

#### 6. Get Practice Statistics
```
GET /api/listening/stats
Authorization: Bearer {token}
```
**Response:**
```json
{
  "skillType": "listening",
  "totalQuestions": 50,
  "correctAnswers": 38,
  "accuracyPercent": 76.0,
  "averageBandTarget": 6.8,
  "totalTimeSeconds": 1200
}
```

---

## Reading Practice API

### Base URL
`/api/reading`

### Endpoints

#### 1. Get Reading Practice Set by Difficulty
```
GET /api/reading/practice/{difficulty}
```
- **Parameters**: `difficulty` = band_3_4 | band_5_6 | band_7_8 | band_8_9
- **Returns**: Random passage with questions (no answers)
- **Response**: `{ passage, questions: [] }`

#### 2. Get Specific Passage
```
GET /api/reading/passage/{passageId}
```
- **Returns**: Reading passage with all questions
- **Response**: `{ passage, questions: [] }`

#### 3. List All Passages
```
GET /api/reading/passages?difficulty=band_5_6
```
- **Optional Query**: `difficulty`
- **Returns**: Array of available reading passages

#### 4. Submit Answer
```
POST /api/reading/submit
Authorization: Bearer {token}
```
**Request Body:**
```json
{
  "questionId": 1,
  "userAnswer": "True",
  "timeSpentSeconds": 45
}
```
**Response:**
```json
{
  "questionId": 1,
  "questionNumber": 1,
  "isCorrect": true,
  "userAnswer": "True",
  "correctAnswer": "True",
  "explanation": "...",
  "bandTarget": 7.0,
  "questionType": "true_false_not_given"
}
```

#### 5. Get Practice History
```
GET /api/reading/history?limit=20
Authorization: Bearer {token}
```
- **Returns**: Last 20 reading attempts

#### 6. Get Practice Statistics
```
GET /api/reading/stats
Authorization: Bearer {token}
```
**Response:**
```json
{
  "skillType": "reading",
  "totalQuestions": 45,
  "correctAnswers": 36,
  "accuracyPercent": 80.0,
  "averageBandTarget": 7.2,
  "totalTimeSeconds": 1800
}
```

---

## Question Types Supported

### Listening
- **fill_in_blank**: Text matching (case-insensitive, alternatives separated by comma)
- **multiple_choice**: Select A/B/C/D

### Reading
1. **true_false_not_given**: Exact match True/False/Not Given
2. **multiple_choice**: Select A/B/C/D
3. **matching_heading**: Multiple headings to paragraphs (comma/semicolon separated)
4. **sentence_completion**: Complete the sentence (word bank, case-insensitive)
5. **summary_completion**: Complete summary from word list

---

## Answer Validation Logic

### Listening
- **fill_in_blank**: Case-insensitive, whitespace trimmed, supports aliases (e.g., "3" or "three")
- **multiple_choice**: Case-insensitive exact match (A, B, C, D)

### Reading
- **true_false_not_given**: Case-insensitive exact match
- **multiple_choice**: Case-insensitive exact match
- **matching_heading**: All correct answers must be provided (no extra)
- **sentence_completion**: Case-insensitive exact match
- **summary_completion**: Case-insensitive exact match from word bank

---

## Performance Considerations

### Optimization
- No transcript included in practice list (reduces payload)
- Transcript only provided when user explicitly fetches material
- Questions exclude correct answers in practice mode
- Indexes on material_id, passage_id, skill_type, attempt_date
- Use pagination for history (default limit: 20)

### Payloads
- Material DTO: ~2-5 KB
- Passage DTO: ~8-12 KB
- Practice set response: ~10-20 KB
- Answer result: ~1-2 KB

### Caching (Future)
- Cache practice materials (1 hour TTL)
- Cache passage text (24 hours TTL)
- No caching on POST answers (user-specific)

---

## Database Schema

### Tables
1. `tb_listening_materials` - Listening audio/transcript content
2. `tb_listening_questions` - Questions for each material
3. `tb_reading_passages` - Reading text passages
4. `tb_reading_questions` - Questions for each passage
5. `tb_user_practice_attempts` - User answer submissions + scoring

### Indexes
- material_id, passage_id, user_id, skill_type, attempt_date
- Query performance: <100ms for most requests

---

## Authentication

All `POST` endpoints require JWT Bearer token in Authorization header:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

---

## Error Responses

```json
{
  "success": false,
  "message": "Material not found",
  "errors": null
}
```

Common error codes:
- 400: Invalid difficulty level, missing fields
- 401: Not authenticated
- 404: Material/passage/question not found
- 500: Server error

---

## Migration Script

Before using these APIs, run the migration:
```bash
mysql -u root -h localhost --port=3307 ieltsdb < ieltsdb_listening_reading_migration.sql
```

This creates:
- 5 new tables
- Performance indexes
- Sample data (3 listening materials, 2 reading passages)
