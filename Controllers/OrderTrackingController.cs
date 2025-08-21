using E_Commers_Adelia.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.CodeDom;

namespace E_Commers_Adelia.Controllers
{
    public class OrderTrackingController : Controller
    {
        private readonly IOrderHistory _history;
        private readonly IHubContext<NotificationHub> _hub;

        public OrderTrackingController(IOrderHistory history,IHubContext<NotificationHub> hub)
        {
            _history = history;
            _hub = hub;
        }
        public async Task<IActionResult> Index(string? orderNo = null)
        {
            var search = orderNo?.Trim().Replace("#", "") ?? "";
            var model = await _history.GetHistoryAsync(search);
            ViewData["search"] = search;
            return View(model);
        }
    }
}
