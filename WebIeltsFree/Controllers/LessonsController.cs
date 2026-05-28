using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WebIeltsFree.Models;
using WebIeltsFree.Services;

namespace WebIeltsFree.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IGeminiService _geminiService;

    public LessonsController(AppDbContext context, IGeminiService geminiService)
    {
        _context = context;
        _geminiService = geminiService;
    }
    
    [HttpGet("skills")]
    public ActionResult<ApiResponse<List<string>>> GetSkills()
    {
        var skills = new List<string> { "reading", "listening", "writing", "speaking" };
        return Ok(ApiResponse<List<string>>.Ok(skills));
    }

    /// <summary>
    /// Get lessons by skill type
    /// </summary>
    [HttpGet("skills/{skillType}")]
    public async Task<ActionResult<ApiResponse<List<LessonSummaryDto>>>> GetLessonsBySkill(string skillType)
    {
        var userId = GetCurrentUserId();

        var lessons = await _context.Lessons
            .Where(l => l.SkillType == skillType.ToLower() && l.Contents.Any())
            .Select(l => new LessonSummaryDto
            {
                LessonId = l.LessonId,
                Title = l.Title,
                SkillType = l.SkillType,
                DifficultyLevel = l.DifficultyLevel,
                EstimatedMinutes = l.EstimatedMinutes,
                CompletionPercent = _context.UserLearningProgress
                    .Where(p => p.UserId == userId && p.LessonId == l.LessonId)
                    .Select(p => p.CompletionPercent)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(ApiResponse<List<LessonSummaryDto>>.Ok(lessons));
    }

    /// <summary>
    /// Get all courses
    /// </summary>
    [HttpGet("courses")]
    public async Task<ActionResult<ApiResponse<List<CourseDto>>>> GetCourses()
    {
        var courses = await _context.Courses
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Title = c.Title,
                TargetBand = c.TargetBand,
                Description = c.Description,
                ModuleCount = c.Modules.Count,
                LessonCount = c.Modules.SelectMany(m => m.Lessons).Count()
            })
            .ToListAsync();

        return Ok(ApiResponse<List<CourseDto>>.Ok(courses));
    }

    /// <summary>
    /// Get course details with modules
    /// </summary>
    [HttpGet("courses/{id}")]
    public async Task<ActionResult<ApiResponse<CourseDto>>> GetCourse(int id)
    {
        var userId = GetCurrentUserId();

        var course = await _context.Courses
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (course == null)
            return NotFound(ApiResponse<CourseDto>.Fail("Course not found"));

        var courseDto = new CourseDto
        {
            CourseId = course.CourseId,
            Title = course.Title,
            TargetBand = course.TargetBand,
            Description = course.Description,
            ModuleCount = course.Modules.Count,
            LessonCount = course.Modules.SelectMany(m => m.Lessons).Count()
        };

        return Ok(ApiResponse<CourseDto>.Ok(courseDto));
    }

    /// <summary>
    /// Get lesson details by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<LessonDetailDto>>> GetLesson(int id)
    {
        var userId = GetCurrentUserId();

        var lesson = await _context.Lessons
            .Include(l => l.Contents)
            .FirstOrDefaultAsync(l => l.LessonId == id);

        if (lesson == null)
            return NotFound(ApiResponse<LessonDetailDto>.Fail("Lesson not found"));

        var progress = await _context.UserLearningProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == id);

        var lessonDto = new LessonDetailDto
        {
            LessonId = lesson.LessonId,
            Title = lesson.Title,
            SkillType = lesson.SkillType,
            DifficultyLevel = lesson.DifficultyLevel,
            EstimatedMinutes = lesson.EstimatedMinutes,
            CompletionPercent = progress?.CompletionPercent ?? 0,
            Score = progress?.Score,
            Contents = lesson.Contents.Select(c => new LessonContentDto
            {
                ContentId = c.ContentId,
                ContentType = c.ContentType,
                ContentBody = c.ContentBody
            }).ToList()
        };

        if (lesson.SkillType?.ToLower() == "reading")
        {
            var passage = await _context.ReadingPassages.FirstOrDefaultAsync(p => p.LessonId == id);
            if (passage != null)
            {
                if (string.IsNullOrEmpty(passage.PassageTranslation) || string.IsNullOrEmpty(passage.VocabHighlights))
                {
                    await GenerateAiStudyAidForPassageAsync(passage);
                }

                lessonDto.ReadingPassage = new ReadingPassageDto
                {
                    PassageId = passage.PassageId,
                    PassageTitle = passage.PassageTitle,
                    PassageText = passage.PassageText,
                    WordCount = passage.WordCount,
                    DifficultyLevel = passage.DifficultyLevel,
                    TopicCategory = passage.TopicCategory,
                    Source = passage.Source,
                    ImageUrl = passage.ImageUrl,
                    PassageTranslation = passage.PassageTranslation,
                    VocabHighlights = passage.VocabHighlights
                };
            }
        }

        return Ok(ApiResponse<LessonDetailDto>.Ok(lessonDto));
    }

    /// <summary>
    /// Submit an answer for a lesson question
    /// </summary>
    [HttpPost("{id}/submit-answer")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SubmitAnswerResponse>>> SubmitAnswer(int id, [FromBody] SubmitAnswerRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<SubmitAnswerResponse>.Fail("Not authenticated"));

        var question = await _context.Questions
            .Include(q => q.Answer)
            .FirstOrDefaultAsync(q => q.QuestionId == request.QuestionId);

        if (question == null)
            return NotFound(ApiResponse<SubmitAnswerResponse>.Fail("Question not found"));

        var isCorrect = question.Answer?.CorrectAnswer?.Trim().ToLower() == request.Answer.Trim().ToLower();

        // Update lesson progress
        var progress = await _context.UserLearningProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == id);

        if (progress == null)
        {
            progress = new UserLearningProgress
            {
                UserId = userId,
                LessonId = id,
                CompletionPercent = 0,
                LastAccessed = DateTime.UtcNow
            };
            _context.UserLearningProgress.Add(progress);
        }

        progress.LastAccessed = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<SubmitAnswerResponse>.Ok(new SubmitAnswerResponse
        {
            IsCorrect = isCorrect,
            CorrectAnswer = isCorrect ? null : question.Answer?.CorrectAnswer,
            XPEarned = isCorrect ? 10 : 0
        }));
    }

    /// <summary>
    /// Mark a lesson as complete
    /// </summary>
    [HttpPost("{id}/complete")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CompleteLessonResponse>>> CompleteLesson(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<CompleteLessonResponse>.Fail("Not authenticated"));

        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null)
            return NotFound(ApiResponse<CompleteLessonResponse>.Fail("Lesson not found"));

        var progress = await _context.UserLearningProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == id);

        if (progress == null)
        {
            progress = new UserLearningProgress
            {
                UserId = userId,
                LessonId = id
            };
            _context.UserLearningProgress.Add(progress);
        }

        progress.CompletionPercent = 100;
        progress.LastAccessed = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<CompleteLessonResponse>.Ok(new CompleteLessonResponse
        {
            Success = true,
            Score = progress.Score ?? 0,
            XPEarned = 50,
            Message = "Lesson completed!"
        }));
    }

    /// <summary>
    /// Get lesson progress
    /// </summary>
    [HttpGet("{id}/progress")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<LessonSummaryDto>>> GetLessonProgress(int id)
    {
        var userId = GetCurrentUserId();

        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null)
            return NotFound(ApiResponse<LessonSummaryDto>.Fail("Lesson not found"));

        var progress = await _context.UserLearningProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == id);

        return Ok(ApiResponse<LessonSummaryDto>.Ok(new LessonSummaryDto
        {
            LessonId = lesson.LessonId,
            Title = lesson.Title,
            SkillType = lesson.SkillType,
            DifficultyLevel = lesson.DifficultyLevel,
            EstimatedMinutes = lesson.EstimatedMinutes,
            CompletionPercent = progress?.CompletionPercent ?? 0
        }));
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    private async Task GenerateAiStudyAidForPassageAsync(ReadingPassage passage)
    {
        // Try static curated content for known Cambridge 20 passages to avoid API calls and ensure 100% correct translation/vocab.
        if (passage.PassageId == 2001 || passage.PassageTitle.Contains("Urban Green"))
        {
            passage.PassageTranslation = @"Paragraph A: Trong những thập kỷ gần đây, các nhà quy hoạch đô thị đã nhận ra rằng các không gian xanh đô thị — công viên, khu vườn, những con đường rợp bóng cây và bờ sông — đóng vai trò quan trọng trong việc cải thiện sức khỏe thể chất và tinh thần của người dân. Các nghiên cứu được thực hiện tại các thành phố trên khắp châu Âu và Bắc Mỹ liên tục chỉ ra rằng những người sống trong vòng 300 mét từ công viên báo cáo mức độ căng thẳng, lo âu và bệnh tim mạch thấp hơn.

Paragraph B: Các cơ chế đằng sau những lợi ích này vừa trực tiếp vừa gián tiếp. Tiếp xúc trực tiếp với môi trường tự nhiên đã được chứng minh là làm giảm nồng độ cortisol và giảm huyết áp chỉ trong vòng hai mươi phút. Gián tiếp, các công viên khuyến khích hoạt động thể chất: đi bộ, đạp xe và tập thể dục ngoài trời mà có lẽ sẽ không xảy ra trong các môi trường đô thị đông đúc.

Paragraph C: Bất chấp các bằng chứng, nhiều thành phố phát triển nhanh chóng đang đối mặt với tình trạng thiếu không gian xanh dễ tiếp cận. Giá đất ở các khu vực trung tâm thường khiến việc tạo lập công viên không khả thi về mặt tài chính, và các khu vực xanh hiện tại đôi khi bị bán để phát triển thương mại. Để ứng phó, một số chính quyền đô thị đã áp dụng các phương pháp đổi mới như vườn trên sân thượng, rừng thẳng đứng trên mặt đứng tòa nhà, và chuyển đổi các tuyến đường sắt không sử dụng thành công viên tuyến tính.

Paragraph D: Các nhà phê bình chỉ ra rằng sự phân bổ không gian xanh thường không bình đẳng. Các khu dân cư giàu có hơn có xu hướng có nhiều công viên hơn và được bảo dưỡng tốt hơn, trong khi các khu vực có thu nhập thấp hơn chỉ có những khu đất nhỏ, được trang bị kém. Giải quyết sự chênh lệch này đòi hỏi các chính sách đầu tư có mục tiêu ưu tiên các cộng đồng chưa được phục vụ đầy đủ thay vì phân bổ kinh phí đồng đều trên tất cả các quận huyện.";

            passage.VocabHighlights = @"[
  {""word"": ""cardiovascular"", ""ipa"": ""/ˌkɑː.di.əʊˈvæs.kjə.lər/"", ""vi"": ""thuộc tim mạch"", ""en"": ""relating to the heart and blood vessels""},
  {""word"": ""cortisol"", ""ipa"": ""/ˈkɔː.tɪ.zɒl/"", ""vi"": ""hoóc môn cortisol"", ""en"": ""a steroid hormone produced by the adrenal glands in response to stress""},
  {""word"": ""unviable"", ""ipa"": ""/ʌnˈvaɪ.ə.bəl/"", ""vi"": ""không khả thi"", ""en"": ""not capable of working successfully; not financially feasible""},
  {""word"": ""disparity"", ""ipa"": ""/dɪˈspær.ə.ti/"", ""vi"": ""sự chênh lệch, bất bình đẳng"", ""en"": ""a great difference, especially one connected with unfair treatment""},
  {""word"": ""municipalities"", ""ipa"": ""/mjuːˌnɪs.ɪˈpæl.ə.tiz/"", ""vi"": ""chính quyền thành phố"", ""en"": ""towns or districts that have local government""},
  {""word"": ""linear"", ""ipa"": ""/ˈlɪn.i.ər/"", ""vi"": ""tuyến tính, kéo dài"", ""en"": ""arranged in or extending along a straight line""},
  {""word"": ""underserved"", ""ipa"": ""/ˌʌn.dəˈsɜːvd/"", ""vi"": ""chưa được hỗ trợ đầy đủ"", ""en"": ""provided with inadequate services or facilities""}
]";
            _context.ReadingPassages.Update(passage);
            await _context.SaveChangesAsync();
            return;
        }

        if (passage.PassageId == 2002 || passage.PassageTitle.Contains("Sleep"))
        {
            passage.PassageTranslation = @"Paragraph A: Mối quan hệ giữa giấc ngủ và chức năng nhận thức đã là chủ đề của cuộc điều tra khoa học trong hơn một thế kỷ. Thần kinh học hiện đại đã xác nhận điều mà sinh viên từ lâu đã nghi ngờ: giấc ngủ đầy đủ là điều cần thiết để củng cố thông tin mới vào trí nhớ dài hạn. Trong các giai đoạn giấc ngủ sâu, hồi hải mã tái hiện các mô hình hoạt động thần kinh được ghi lại trong ngày, chuyển chúng đến vỏ não mới để lưu trữ vĩnh viễn.

Paragraph B: Nghiên cứu tại Đại học Lübeck đã chứng minh rằng những người tham gia ngủ trong tám giờ sau khi học một bộ cặp từ đã nhớ lại chính xác 92% trong số đó vào ngày hôm sau, so với chỉ 74% ở nhóm thức trong cùng một khoảng thời gian. Quan trọng là, nhóm ngủ cũng cho thấy hiệu suất vượt trội trong các nhiệm vụ giải quyết vấn đề sáng tạo, gợi ý rằng giấc ngủ không chỉ tăng cường trí nhớ học vẹt mà còn cả tư duy linh hoạt.

Paragraph C: Bất chấp bằng chứng này, các cuộc khảo sát chỉ ra rằng sinh viên đại học chỉ ngủ trung bình 6,2 giờ mỗi đêm — thấp hơn nhiều so với mức khuyến nghị từ bảy đến chín giờ. Hậu quả vượt ra ngoài trí nhớ: thiếu ngủ làm suy giảm sự chú ý, ra quyết định và điều chỉnh cảm xúc. Một nghiên cứu dọc theo dõi 3.000 sinh viên đại học phát hiện ra rằng mỗi giờ ngủ mất đi mỗi đêm có liên quan đến việc giảm 0,15 điểm trung bình tích lũy trong một năm học.

Paragraph D: Các trường đại học đã bắt đầu ứng phó. Một số tổ chức hiện không lên lịch thi cử trước 10 giờ sáng, và một số cung cấp các hội thảo về vệ sinh giấc ngủ như một phần của chương trình định hướng. Tuy nhiên, các thái độ văn hóa tôn vinh việc học bài đêm muộn vẫn là một rào cản đáng kể đối với sự thay đổi.";

            passage.VocabHighlights = @"[
  {""word"": ""cognitive"", ""ipa"": ""/ˈɒɡ.nə.tɪv/"", ""vi"": ""thuộc về nhận thức"", ""en"": ""relating to the mental action or process of acquiring knowledge""},
  {""word"": ""consolidating"", ""ipa"": ""/kənˈsɒl.ɪ.deɪ.tɪŋ/"", ""vi"": ""củng cố, hợp nhất"", ""en"": ""reinforcing or strengthening a connection or memory""},
  {""word"": ""hippocampus"", ""ipa"": ""/ˌhɪp.əˈkæm.pəs/"", ""vi"": ""hồi hải mã"", ""en"": ""a part of the brain involved in forming and consolidating memories""},
  {""word"": ""neocortex"", ""ipa"": ""/ˌniː.əʊˈkɔː.teks/"", ""vi"": ""vỏ não mới"", ""en"": ""the part of the brain involved in higher-order brain functions""},
  {""word"": ""rote"", ""ipa"": ""/rəʊt/"", ""vi"": ""học vẹt, học lòng"", ""en"": ""mechanical or habitual repetition of something to be learned""},
  {""word"": ""deprivation"", ""ipa"": ""/ˌdep.rɪˈveɪ.ʃən/"", ""vi"": ""sự thiếu hụt, mất ngủ"", ""en"": ""the state of lacking basic necessities like sleep or food""},
  {""word"": ""longitudinal"", ""ipa"": ""/ˌlɒŋ.ɡɪˈtʃuː.dɪ.nəl/"", ""vi"": ""theo chiều dọc, dài hạn"", ""en"": ""observing or tracking variables over an extended period of time""}
]";
            _context.ReadingPassages.Update(passage);
            await _context.SaveChangesAsync();
            return;
        }

        if (passage.PassageId == 2003 || passage.PassageTitle.Contains("Artificial") || passage.PassageTitle.Contains("Clinical"))
        {
            passage.PassageTranslation = @"Paragraph A: Việc áp dụng trí tuệ nhân tạo vào chẩn đoán lâm sàng đại diện cho một trong những sự phát triển đầy hứa hẹn và gây tranh cãi nhất trong chăm sóc sức khỏe đương đại. Các thuật toán học máy được huấn luyện trên hàng triệu hình ảnh y khoa được dán nhãn hiện có thể xác định một số tình trạng bệnh — bao gồm bệnh võng mạc tiểu đường, ung thư da và viêm phổi — với độ chính xác tương đương và đôi khi vượt qua cả các chuyên gia giàu kinh nghiệm.

Paragraph B: Một phân tích gộp năm 2019 được công bố trên The Lancet Digital Health đã đánh giá 82 nghiên cứu so sánh các hệ thống học sâu với các chuyên gia y tế. Độ nhạy gộp của các hệ thống AI là 87%, khớp với tỷ lệ 86,4% của các bác sĩ lâm sàng. Tuy nhiên, độ đặc hiệu của AI thấp hơn ở một số danh mục, nghĩa là nó tạo ra nhiều kết quả dương tính giả hơn — gắn cờ những bệnh nhân khỏe mạnh là có khả năng bị bệnh.

Paragraph C: Các khung pháp lý đã gặp khó khăn trong việc bắt kịp với công nghệ. Phê duyệt thiết bị y tế truyền thống giả định một sản phẩm cố định, tuy nhiên các mô hình AI có thể được cập nhật liên tục khi có dữ liệu mới. Cục Quản lý Thực phẩm và Dược phẩm Hoa Kỳ đã giới thiệu một chương trình thí điểm vào năm 2021 cho phép các cập nhật lặp lại theo kế hoạch kiểm soát thay đổi định trước, nhưng nhiều cơ quan tài phán vẫn thiếu hướng dẫn tương đương.

Paragraph D: Các mối quan tâm về đạo đức tập trung vào định kiến dữ liệu và trách nhiệm giải trình. Nếu một thuật toán chẩn đoán được huấn luyện chủ yếu trên hình ảnh từ một nhóm dân tộc, độ chính xác của nó có thể giảm đối với những nhóm khác. Hơn nữa, khi một hệ thống AI góp phần vào việc chẩn đoán sai, việc phân chia trách nhiệm pháp lý giữa nhà phát triển, bệnh viện và bác sĩ lâm sàng giám sát vẫn chưa được giải quyết trong hầu hết các hệ thống pháp luật.";

            passage.VocabHighlights = @"[
  {""word"": ""diagnostics"", ""ipa"": ""/ˌdaɪ.əɡˈnɒs.tɪks/"", ""vi"": ""chẩn đoán học"", ""en"": ""the practice or science of identifying the nature of an illness""},
  {""word"": ""contentious"", ""ipa"": ""/kənˈten.ʃəs/"", ""vi"": ""gây tranh cãi"", ""en"": ""causing or likely to cause an argument; controversial""},
  {""word"": ""retinopathy"", ""ipa"": ""/ˌret.ɪˈnɒp.ə.θi/"", ""vi"": ""bệnh võng mạc"", ""en"": ""disease of the retina which results in impairment or loss of vision""},
  {""word"": ""sensitivity"", ""ipa"": ""/ˌsen.sɪˈtɪv.ə.ti/"", ""vi"": ""độ nhạy"", ""en"": ""the ability of a diagnostic test to correctly identify those with the disease""},
  {""word"": ""specificity"", ""ipa"": ""/ˌspes.ɪˈfɪs.ə.ti/"", ""vi"": ""độ đặc hiệu"", ""en"": ""the ability of a diagnostic test to correctly identify those without the disease""},
  {""word"": ""iterative"", ""ipa"": ""/ˈɪt.ər.ə.tɪv/"", ""vi"": ""lặp đi lặp lại"", ""en"": ""relating to or involving repetition, doing something repeatedly to improve it""},
  {""word"": ""jurisdictions"", ""ipa"": ""/ˌdʒʊə.rɪsˈdɪk.ʃənz/"", ""vi"": ""khu vực tài phán"", ""en"": ""the official power to make legal decisions and judgments""}
]";
            _context.ReadingPassages.Update(passage);
            await _context.SaveChangesAsync();
            return;
        }

        // Dynamic AI Generation Fallback for new/unknown passages
        try
        {
            var systemPrompt = "You are an expert IELTS Reading AI Assistant. You must parse the provided passage and respond ONLY with a raw JSON object containing the translations and vocabularies. Do not include any explanation or markdown formatting like ```json.";
            var prompt = $@"Analyze the following English IELTS reading passage.
Perform two tasks:
1. Translate each paragraph into high-quality, academic Vietnamese. Keep the exact same paragraph structure, and start each translated paragraph with 'Paragraph A: ', 'Paragraph B: ', etc., exactly matching the input paragraphs. Separate the translated paragraphs with exactly two newlines (\n\n).
2. Identify 8-12 advanced, academic, or challenging vocabulary words from the passage. For each word, provide:
   - 'word': The exact word as it appears in the passage (base form or conjugated, e.g., 'consolidating' or 'consolidate').
   - 'ipa': The standard IPA phonetic pronunciation.
   - 'vi': A short, clear Vietnamese translation of the word.
   - 'en': A concise English definition explaining its meaning in this context.

Passage Text:
{passage.PassageText}

Return the result strictly as a valid JSON object in this exact format:
{{
  ""translation"": ""Paragraph A: ...\n\nParagraph B: ..."",
  ""vocabulary"": [
    {{
      ""word"": ""example"",
      ""ipa"": ""/ɪɡˈzɑːm.pəl/"",
      ""vi"": ""ví dụ"",
      ""en"": ""a thing characteristic of its kind or illustrating a general rule""
    }}
  ]
}}";

            var response = await _geminiService.GenerateContentAsync(prompt, systemPrompt);
            response = CleanJsonResponse(response);

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;
            if (root.TryGetProperty("translation", out var transProp))
            {
                passage.PassageTranslation = transProp.GetString();
            }
            if (root.TryGetProperty("vocabulary", out var vocabProp))
            {
                passage.VocabHighlights = vocabProp.GetRawText();
            }

            _context.ReadingPassages.Update(passage);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in dynamic AI translation generation: {ex.Message}");
        }
    }

    private static string CleanJsonResponse(string response)
    {
        response = response.Trim();
        if (response.StartsWith("```json"))
            response = response[7..];
        else if (response.StartsWith("```"))
            response = response[3..];
        
        if (response.EndsWith("```"))
            response = response[..^3];
        
        return response.Trim();
    }

    #endregion
}
