using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using E_Commers_Adelia.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace E_Commers_Adelia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly UserManager<EUser> _userManager;
        private readonly SignInManager<EUser> _signInManager;
        private readonly INotification _noti;

        public HomeController(ApplicationDbContext db, 
            IHubContext<NotificationHub> hub, 
            UserManager<EUser> userManager, 
            SignInManager<EUser> signInManager, 
            INotification noti)
        {
            _db = db;
            _hub = hub;
            _userManager = userManager;
            _signInManager = signInManager;
            _noti = noti;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.ToListAsync();
            List<Product> whiteListProducts = new List<Product>();
            foreach (var item in products)
            {
                var owner = await _userManager.FindByIdAsync(item.userId);
                if (owner != null)
                {
                    if (owner.IsOpen)
                    {
                        whiteListProducts.Add(item);
                    }
                }
                
            }
            return View(whiteListProducts);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
