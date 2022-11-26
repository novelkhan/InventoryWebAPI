using Microsoft.AspNetCore.Mvc;

namespace LoginDemo.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("JWToken");
            return View();
        }
    }
}
