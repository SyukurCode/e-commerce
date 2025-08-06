using Microsoft.AspNetCore.Mvc;

namespace E_Commers_Adelia.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
