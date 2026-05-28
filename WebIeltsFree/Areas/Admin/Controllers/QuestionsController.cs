using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebIeltsFree.Models;
using System.Security.Claims;
using WebIeltsFree.Utilities;
using System.Text.Json;

namespace WebIeltsFree.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class QuestionsController : Controller
{
    private readonly AppDbContext _context;

    public QuestionsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    // GET: Admin/Questions
    public async Task<IActionResult> Index(string sectionFilter, string searchString, int pageNumber = 1)
    {
        int pageSize = 15;
        
        var query = _context.Questions.Include(q => q.Answer).AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(q => q.QuestionText.Contains(searchString));
        }

        if (!string.IsNullOrEmpty(sectionFilter))
        {
            // Section name matching requires join or just filtering if section enum is known, 
            // skipping for simplicity as Section is relation here.
        }

        var totalItems = await query.CountAsync();
        var questions = await query
            .OrderByDescending(q => q.QuestionId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.CurrentPage = pageNumber;
        ViewBag.SearchString = searchString;
        ViewBag.SectionFilter = sectionFilter;

        return View(questions);
    }

    // GET: Admin/Questions/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Questions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Question question, string correctAnswer, string answerExplanation)
    {
        if (ModelState.IsValid)
        {
            // First save the question
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            // Then create and save the linked answer
            var answer = new Answer
            {
                QuestionId = question.QuestionId,
                CorrectAnswer = correctAnswer ?? ""
            };
            
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(
                _context,
                GetCurrentUserId(),
                "create_question",
                "Question",
                question.QuestionId,
                null,
                JsonSerializer.Serialize(new { question.SectionId, CorrectAnswer = answer.CorrectAnswer }),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            TempData["Success"] = "Question and answer created successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(question);
    }

    // GET: Admin/Questions/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var question = await _context.Questions
            .Include(q => q.Answer)
            .FirstOrDefaultAsync(q => q.QuestionId == id);
            
        if (question == null) return NotFound();

        return View(question);
    }

    // POST: Admin/Questions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Question question, string correctAnswer, string answerExplanation)
    {
        if (id != question.QuestionId) return NotFound();

        if (ModelState.IsValid)
        {
            var existingQuestion = await _context.Questions
                .Include(q => q.Answer)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.QuestionId == id);
                
            var oldValues = JsonSerializer.Serialize(new { 
                existingQuestion?.QuestionText, 
                existingQuestion?.Difficulty,
                CorrectAnswer = existingQuestion?.Answer?.CorrectAnswer
            });
            
            _context.Update(question);
            
            // Handle answer update
            var answer = await _context.Answers.FirstOrDefaultAsync(a => a.QuestionId == id);
            if (answer != null)
            {
                answer.CorrectAnswer = correctAnswer ?? "";
                _context.Update(answer);
            }
            else
            {
                // Create if missing
                _context.Answers.Add(new Answer
                {
                    QuestionId = question.QuestionId,
                    CorrectAnswer = correctAnswer ?? ""
                });
            }
            
            await _context.SaveChangesAsync();

            await AuditHelper.LogAsync(
                _context,
                GetCurrentUserId(),
                "edit_question",
                "Question",
                question.QuestionId,
                oldValues,
                JsonSerializer.Serialize(new { 
                    question.QuestionText, 
                    question.Difficulty,
                    CorrectAnswer = correctAnswer
                }),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            TempData["Success"] = "Question updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(question);
    }

    // POST: Admin/Questions/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question == null) return NotFound();

        // In this system, questions are typically hard deleted or soft deleted based on foreign key constraints.
        // For safety, we will just delete the answer and question directly.
        var answer = await _context.Answers.FirstOrDefaultAsync(a => a.QuestionId == id);
        if (answer != null)
        {
            _context.Answers.Remove(answer);
        }
        
        _context.Questions.Remove(question);
        
        try 
        {
            await _context.SaveChangesAsync();
            
            await AuditHelper.LogAsync(
                _context,
                GetCurrentUserId(),
                "delete_question",
                "Question",
                id,
                null,
                null,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers.UserAgent.ToString()
            );

            TempData["Success"] = "Question deleted successfully.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Cannot delete this question because it is referenced by active test sessions.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Questions/BulkImport
    public IActionResult BulkImport()
    {
        return View();
    }
}
