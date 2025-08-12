using E_Commers_Adelia.Common;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Migrations;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.Threading.Tasks;

namespace E_Commers_Adelia.Controllers
{
    [Authorize]
    public class ProcessOrderController : Controller
    {
        private readonly UserManager<EUser> _userManager;
        private readonly ApplicationDbContext _db;
        public ProcessOrderController(UserManager<EUser> userManager, ApplicationDbContext db) 
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var orders = await _db.Orders.Where(x => x.SellerId == currentUser.Id && x.StatusId == OrderStatus.OrderSend.Id).OrderBy(x => x.PlaceDateTime).ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> View(string id)
        {
            var order = await _db.Orders.Where(x=>x.OrderNo == id).ToListAsync();
            if(order != null)
            {
                // update status order to pickup
                foreach(var item in order)
                {
                    item.StatusId = OrderStatus.PickupBySeller.Id;
                    _db.Orders.UpdateRange(item);
                }
                await _db.SaveChangesAsync();
            }
            return View(order);
        }
    }
}
