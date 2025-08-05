using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace E_Commers_Adelia.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CheckoutController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string orderNo)
        {
            decimal totalPrice = 0;
            var orders = await _db.Orders.Where(o => o.OrderNo == orderNo).ToListAsync();
            foreach (var order in orders)
            {
                totalPrice += order.SubTotalPrice;
            }

            ViewData["TotalToPay"] = totalPrice;

            return View(orders);
        }
    }
}
