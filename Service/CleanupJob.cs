using E_Commers_Adelia.Common;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Hangfire.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
namespace E_Commers_Adelia.Service
{
    public class CleanupJob : ICleanupJob
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<EUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CleanupJob(ApplicationDbContext db, UserManager<EUser> userManager, IWebHostEnvironment env) 
        {
            _db = db;
            _userManager = userManager;
            _webHostEnvironment = env;
        }
        public async Task RunCleaningAsync()
        {
            Log.Information("Start cleaning");
            await cleanGuestOrder();
            await cleanOldComplete();

            await Task.CompletedTask;
        }
        public async Task<List<CleaningItem>> ListItemToCleanAsync()
        {
            List<CleaningItem> items = new List<CleaningItem>();
            List<string> orderNo = new List<string>();
            var orders = await _db.Orders.Where(x => x.StatusId == OrderStatus.ToPay.Id && (x.CustomerId == null || x.CustomerId == string.Empty) && x.PlaceDateTime < DateTime.UtcNow.AddMinutes(-30)).ToListAsync();
            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    items.Add(new CleaningItem
                    {
                        Table = "Order",
                        ID = order.Id,
                    });
                    orderNo.Add(order.OrderNo);
                }
            }

            orders = await _db.Orders.Where(x => x.StatusId >= OrderStatus.Completed.Id && x.UpdateDateTime < DateTime.UtcNow.AddDays(-30)).ToListAsync();
            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    items.Add(new CleaningItem
                    {
                        Table = "Order",
                        ID = order.Id,
                    });
                    orderNo.Add(order.OrderNo);
                }
            }

            foreach (var no in orderNo)
            {
                var histories = await _db.OrderHistory.Where(x => x.OrderNo == no).ToListAsync();
                if (histories.Any())
                {
                    foreach (var i in histories)
                    {
                        items.Add(new CleaningItem
                        {
                            Table = "OrderHistory",
                            ID = i.Id
                        });
                    }
                }
                var payments = await _db.CustomerPayments.Where(x => x.OrderNo == no).ToListAsync();
                if (payments.Any())
                {
                    foreach (var i in payments)
                    {
                        items.Add(new CleaningItem
                        {
                            Table = "CustomerPayment",
                            ID = i.Id
                        });
                    }
                }
                var delivery = await _db.CustomerDeliveryInfo.Where(x => x.OrderNo == no).ToListAsync();
                if (delivery.Any())
                {
                    foreach (var i in delivery)
                    {
                        items.Add(new CleaningItem
                        {
                            Table = "CustomerDeliveryInfo",
                            ID = i.Id
                        });
                    }
                }
            }

            return items;
        }
        private protected async Task cleanGuestOrder()
        {
            List<Models.Order> removeOrder = new List<Models.Order>();

            // remove guest order one week ago status to pay
            var orders = await _db.Orders.Where(x => x.StatusId == OrderStatus.ToPay.Id && (x.CustomerId == null || x.CustomerId == string.Empty) && x.PlaceDateTime <  DateTime.UtcNow.AddMinutes(-30)).ToListAsync();
            if (orders.Count > 0)
            {
                foreach (var item in orders)
                {
                    removeOrder.Add(item);
                    await removeHistory(item.OrderNo);
                }
                _db.Orders.RemoveRange(removeOrder);
                await _db.SaveChangesAsync();

                Log.Information($"{removeOrder.Count()} order removed");
            }  
        }
        private protected async Task cleanOldComplete() 
        {
            // remove customer order, old > 30days, status completed, cancel, \
            List<Models.Order> removeOrder = new List<Models.Order>();
            var orders = await _db.Orders.Where(x => x.StatusId >= OrderStatus.Completed.Id && x.UpdateDateTime < DateTime.UtcNow.AddDays(-30)).ToListAsync();
            if (orders.Count > 0)
            {
                foreach (var item in orders)
                {
                    removeOrder.Add(item);
                    await removeHistory(item.OrderNo);
                    await removeDeliveryInfo(item.OrderNo);
                    await removePayment(item.OrderNo);
                }
                _db.Orders.RemoveRange(removeOrder);
                await _db.SaveChangesAsync();

                Log.Information($"{removeOrder.Count()} order completed removed");
            }
        }
        private protected async Task removeHistory(string orderNo)
        {
            // remove order history
            var history = await _db.OrderHistory.FirstOrDefaultAsync(x => x.OrderNo == orderNo);
            if (history != null)
            {
                try 
                { 
                    _db.OrderHistory.Remove(history);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Log.Error($"Error when remove history,{ex.Message}");
                }

                Log.Information($"history {orderNo} removed");
            }

        }
        private protected async Task removePayment(string orderNo)
        {
            // remove order payment
            var payment = await _db.CustomerPayments.FirstOrDefaultAsync(x => x.OrderNo == orderNo);
            if (payment != null)
            {
                //remove attachment
                if (payment.ResitUrl != null)
                {
                    UploadFileHelper.removeFile(_webHostEnvironment, payment.ResitUrl);
                }

                try
                {
                    _db.CustomerPayments.Remove(payment);
                    await _db.SaveChangesAsync();
                } catch (Exception ex) {
                    Log.Error($"Error when remove payment,{ex.Message}");
                }

                Log.Information($"payment {orderNo} removed");
            }

        }
        private protected async Task removeDeliveryInfo(string orderNo)
        {
            // remove order payment
            var delivery = await _db.CustomerDeliveryInfo.FirstOrDefaultAsync(x => x.OrderNo == orderNo);
            if (delivery != null)
            {
                try
                { 
                _db.CustomerDeliveryInfo.Remove(delivery);
                await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Log.Error($"Error when remove DeliveryInfo,{ex.Message}");
                }

                Log.Information($"deliveryinfo {orderNo} removed");
            }

        }
    }
}
