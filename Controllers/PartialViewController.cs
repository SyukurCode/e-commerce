using Microsoft.AspNetCore.Mvc;

namespace E_Commers_Adelia.Controllers
{
    public class PartialViewController : Controller
    {
        public IActionResult NavbarLoad()
        {
            return PartialView("_PartialNavBar");
        }
    }
}
