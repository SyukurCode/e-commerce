using E_Commers_Adelia.Repository;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;

namespace E_Commers_Adelia.Controllers
{
    public class OrderTrackingController : Controller
    {
        private readonly IOrderHistory _history;

        public OrderTrackingController(IOrderHistory history)
        {
            _history = history;
        }
        public async Task<IActionResult> Index(string? orderNo = null)
        {
            if (orderNo != null)
            {
                var model = await _history.GetHistoryAsync(orderNo);
                return View(model);
            }
            return View();
        }
    }
}
