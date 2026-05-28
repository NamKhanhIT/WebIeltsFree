using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebIeltsFree.Models;

namespace WebIeltsFree.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() => View();
        public IActionResult Dashboard() => View();
        public IActionResult Login() => View();
        public IActionResult Register() => View();
        public IActionResult OnboardingQuestions() => View();
        public IActionResult Privacy() => View();
        public IActionResult SupportChat() => View();

        // ───── Learning Mode ─────
        public IActionResult Courses() => View("~/Views/Learn/Courses.cshtml");
        public IActionResult AiTutor() => View("~/Views/Learn/AiTutor.cshtml");
        public IActionResult AiRoadmap() => View("~/Views/Learn/AiRoadmap.cshtml");

        public IActionResult Skill(string type)
        {
            var normalizedType = string.IsNullOrWhiteSpace(type) ? "reading" : type.ToLower();
            ViewData["SkillType"] = normalizedType;
            
            return normalizedType switch
            {
                "listening" => View("~/Views/Learn/LearnListening.cshtml"),
                "reading" => View("~/Views/Learn/LearnReading.cshtml"),
                "writing" => View("~/Views/Learn/LearnWriting.cshtml"),
                "speaking" => View("~/Views/Learn/Speaking.cshtml"),
                _ => View("~/Views/Learn/LearnReading.cshtml")
            };
        }

        // Skill-specific learning views (LEARNING only — educational, tutorial-based)
        public IActionResult LearnWriting() => View("~/Views/Learn/LearnWriting.cshtml");
        public IActionResult LearnListening() => View("~/Views/Learn/LearnListening.cshtml");
        public IActionResult LearnReading() => View("~/Views/Learn/LearnReading.cshtml");
        public IActionResult LearnSpeaking() => View("~/Views/Learn/Speaking.cshtml");

        // Legacy redirects → learning views (preserved for bookmark compatibility)
        public IActionResult Writing() => RedirectToAction(nameof(LearnWriting));
        public IActionResult Listening() => RedirectToAction(nameof(LearnListening));
        public IActionResult Reading() => RedirectToAction(nameof(LearnReading));
        public IActionResult Speaking() => RedirectToAction(nameof(LearnSpeaking));

        // ───── Practice Mode (Exam Simulation) ─────
        public IActionResult Tests() => View("~/Views/Tests/Tests.cshtml");
        public IActionResult PlacementTest() => View("~/Views/Tests/PlacementTest.cshtml");

        public IActionResult TakeTest(int id)
        {
            ViewData["TestId"] = id;
            return View("~/Views/Tests/TakeTest.cshtml");
        }

        public IActionResult Practice(string type, int? id)
        {
            var normalizedSkill = string.IsNullOrWhiteSpace(type) ? "reading" : type.ToLower();
            ViewData["SkillType"] = normalizedSkill;

            var viewName = normalizedSkill switch
            {
                "listening" => "Listening",
                "writing" => "Writing",
                "speaking" => "Speaking",
                _ => "Reading"
            };

            return View($"~/Views/Practice/{viewName}.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
