using E_Commers_Adelia.Common;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
namespace E_Commers_Adelia.Service
{
    public class CleanupJob : ICleanupJob
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<EUser> _userManager;
        public CleanupJob(ApplicationDbContext db, UserManager<EUser> userManager) 
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task RunCleaningAsync()
        {
            Log.Information("Start cleaning panding order");
            List<Order> removeOrder = new List<Order>();
            var orders = await _db.Orders.Where(x => x.StatusId == OrderStatus.ToPay.Id).ToListAsync();
            if (orders != null)
            {
                foreach (var order in orders)
                {
                    // Jika ade user dalam order
                    if(order.CustomerId != null)
                    {
                        // Remove order jika user dh tiada
                        var user = await _userManager.FindByIdAsync(order.CustomerId);
                        if (user == null) {
                            removeOrder.Add(order);
                        }

                    }
                    // Remove jika tiada user dalam order
                    else
                    {
                        removeOrder.Add(order);
                    }

                }
                _db.Orders.RemoveRange(removeOrder);
                await _db.SaveChangesAsync();

                Log.Information($"{removeOrder.Count()} order removed");
            }

            await Task.CompletedTask;
        }
    }
}
