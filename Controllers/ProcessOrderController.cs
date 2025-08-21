using AspNetCoreGeneratedDocument;
using E_Commers_Adelia.Common;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Migrations;
using E_Commers_Adelia.Models;
using E_Commers_Adelia.Repository;
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
        private readonly ICustomerPayment _payment;
        private readonly IOrderHistory _orderHistory;

        public ProcessOrderController(UserManager<EUser> userManager, 
            ApplicationDbContext db, 
            ICustomerPayment payment,
            IOrderHistory orderHistory) 
        {
            _userManager = userManager;
            _db = db;
            _payment = payment;
            _orderHistory = orderHistory;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var orders = await _db.Orders.Where(x => x.SellerId == currentUser.Id && x.StatusId > OrderStatus.ToPay.Id).OrderBy(x => x.PlaceDateTime).ToListAsync();
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
                    if (item.StatusId == OrderStatus.OrderSend.Id)
                    {
                        item.StatusId = OrderStatus.PickupBySeller.Id;
                        item.UpdateDateTime = DateTime.UtcNow;
                        await _orderHistory.CreateAsync(item.OrderNo, "Your order has been picked up by the seller.", item.StatusId);
                        _db.Orders.Update(item);
                        await _db.SaveChangesAsync();
                    }
                }
               
            }
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ComfirmPayment(string id)
        {
            await _payment.Comfirm(id);
            var order = await _db.Orders.Where(x => x.OrderNo == id).ToListAsync();
            if (order != null)
            {
                // update status order to process
                foreach (var item in order)
                {
                    if (item.StatusId == OrderStatus.PickupBySeller.Id)
                    {
                        item.StatusId = OrderStatus.Processing.Id;
                        item.UpdateDateTime = DateTime.UtcNow;
                        _db.Orders.Update(item);
                        await _orderHistory.CreateAsync(orderNo: item.OrderNo, text: "Seller has confirmed and processed your order.", item.StatusId);
                    }
                }
                await _db.SaveChangesAsync();
            }
             return RedirectToAction("View", new { id = id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinishOrder(int id)
        {
            // Update finish order
            var order = await _db.Orders.FindAsync(id);
            if (order != null)
            {
                if (order.StatusId < 4)
                {
                    TempData["DialogWarning"] = "Please comfirm payment first";
                    return RedirectToAction("View", new { id = order.OrderNo });
                }
                order.StatusId = OrderStatus.Completed.Id;
                order.UpdateDateTime = DateTime.UtcNow;
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();

                // Send update to user
                var totalOrder = await _db.Orders.Where(x => x.OrderNo == order.OrderNo).CountAsync();
                var totalComplete = await _db.Orders.Where(x => x.OrderNo == order.OrderNo && x.StatusId > OrderStatus.Processing.Id).CountAsync();
                var totalCancel = await _db.Orders.Where(x => x.OrderNo == order.OrderNo && x.StatusId > OrderStatus.Cancelled.Id).CountAsync();

                if (totalOrder == totalComplete)
                {
                    await _orderHistory.CreateAsync(orderNo: order.OrderNo, text: "Your order has been completed and is ready for delivery/pickup.", (totalCancel == totalOrder ? OrderStatus.Cancelled.Id : order.StatusId));
                }
            }
            
            return RedirectToAction("View", new { id = order?.OrderNo });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            order.StatusId = OrderStatus.Cancelled.Id;
            order.UpdateDateTime = DateTime.UtcNow;
            await _orderHistory.CreateAsync(orderNo: order.OrderNo, text: "Your order has been cancelled by the seller.", order.StatusId);
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();

            return RedirectToAction("View", new { id = order.OrderNo });
        }
    }
}
