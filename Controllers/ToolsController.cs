using E_Commers_Adelia.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace E_Commers_Adelia.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ToolsController : Controller
    {
        private readonly ICleanupJob _cleaner;
        public ToolsController(ICleanupJob cleaner)
        {
            _cleaner = cleaner;
        }
        public async Task<IActionResult> Index()
        {
            
            return View(await _cleaner.ListItemToCleanAsync());
        }
    }
}
