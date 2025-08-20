
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;

namespace E_Commers_Adelia.Repository
{
    public class ROrderHistory : IOrderHistory
    {
        private readonly ApplicationDbContext _db;
        public ROrderHistory(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Create(string orderNo, string text, int statusId)
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

            return new JsonResult( new { success  = true });
        }
        public async Task<OrderHistory> GetHistoryAsync(string orderNo)
        {
            var history = await _db.OrderHistory.FirstOrDefaultAsync(x => x.OrderNo == orderNo);
            return history;
        }

        public async Task<IActionResult> Remove(string orderNo)
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
    }
}
