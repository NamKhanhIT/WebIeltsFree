using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebIeltsFree.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = "teacher,admin")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
