
using E_Commers_Adelia.Common;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.CodeDom;

namespace E_Commers_Adelia.Repository
{
    public class ROrderHistory : IOrderHistory
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<NotificationHub> _hub;
        public ROrderHistory(ApplicationDbContext db, IHubContext<NotificationHub> hub)
        {
            _db = db;
            _hub = hub;
        }
        public async Task<IActionResult> CreateAsync(string orderNo, string text, int statusId)
        {

            var isExist = await _db.OrderHistory.AnyAsync(x => x.OrderNo == orderNo && x.StatusId == statusId);
            if (!isExist)
            {
                OrderHistory history = new OrderHistory
                {
                    OrderNo = orderNo,
                    Text = text,
                    Created = DateTime.UtcNow,
                    StatusId = statusId,
                };
                await _db.AddAsync(history);
                await _db.SaveChangesAsync();

                // prepair to send realtime tracking
                var model = new List<DisplayOrderHistory>();
                var listOModel = await _db.OrderHistory.Where(x => x.OrderNo == orderNo).ToListAsync();
                foreach (var item in listOModel) 
                {
                    var result = process(item.StatusId);
                    if (result.Value != null)
                    {
                        dynamic data = result.Value;

                        var i = new DisplayOrderHistory
                        {
                            Date = item.Created.ToLocalTime().ToString("d/M/yyyy h:mm:ss tt"),
                            Duration = DurationCalulator.GetTimeAgo(item.Created),
                            Icon = data.icon,
                            Mode = data.mode,
                            Text = item.Text,
                            State = data.state
                        };
                        model.Add(i);
                    }
                }
                
                await _hub.Clients.All.SendAsync("Order-Tracking", orderNo , model);
                return new JsonResult(new { success = true });
            }
            return new JsonResult(new { success = false, message = "Already exist" });
        }
        public async Task<IEnumerable<OrderHistory>> GetHistoryAsync(string orderNo)
        {
            var history = await _db.OrderHistory.Where(x => x.OrderNo == orderNo).ToListAsync();
            return history;
        }

        public async Task<IActionResult> RemoveAsync(string orderNo)
        {
            var toDelete = await _db.OrderHistory.FirstOrDefaultAsync(x =>  x.OrderNo == orderNo);
            if (toDelete != null)
            {
                _db.OrderHistory.Remove(toDelete);
                await _db.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            return new JsonResult( new { success = false });
        }

        private protected JsonResult process(int id)
        {
            var icon = string.Empty;
            var state = string.Empty;
            var mode = string.Empty;

            switch(id)
            {
                case 2:
                    icon = "far fa-paper-plane";
                    state = "";
                    mode = "";
                    break;
                case 3:
                    icon = "far fa-bell";
                    state = "warning";
                    mode = "timeline-inverted";
                    break;
                case 4:
                    icon = "fas fa-sync-alt";
                    state = "info";
                    mode = "";
                    break;
                case 5:
                    icon = "fas fa-check-circle";
                    state = "success";
                    mode = "timeline-inverted";
                    break;
                case 6:
                    icon = "fas fa-times-circle";
                    state = "danger";
                    mode = "timeline-inverted";
                    break;
                case 7:
                    icon = "fas fa-people-carry";
                    state = "success";
                    mode = "";
                    break;
                default:
                    icon = "fas fa-question";
                    state = "warning";
                    mode = "timeline-inverted";
                    break;
            }

            return new JsonResult(new { icon = icon, state = state, mode = mode });
        }
    }
}
