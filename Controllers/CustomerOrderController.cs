using AspNetCoreGeneratedDocument;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Migrations;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commers_Adelia.Controllers
{
    public class CustomerOrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<EUser> _userManager;
        public CustomerOrderController(ApplicationDbContext db, UserManager<EUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = _userManager.GetUserId(User);
            var order = await _db.Orders.Where(x => x.CustomerId == currentUser).ToListAsync();
            return View(order);
        }
    }
}
