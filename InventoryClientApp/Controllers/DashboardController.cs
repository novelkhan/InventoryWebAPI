using Microsoft.AspNetCore.Mvc;

namespace InventoryClientApp.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
