using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;
namespace E_Commers_Adelia.Repository
{
    public class RNotification : INotification
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<NotificationHub> _hub;
        public RNotification(ApplicationDbContext db, IHubContext<NotificationHub> hub) { 
            _db = db;
            _hub = hub;
        }

        public async Task<bool> Create(Notification notification)
        {
            if (notification == null)
            {
                return false;
            }
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            var notiCount = await _db.Notifications.Where(x => x.UserId == notification.UserId && x.IsRead == false).CountAsync();
            await _hub.Clients.User(notification.UserId).SendAsync("Noti-Receive", notiCount);

            return true;
        }

        public async Task<Notification> GetById(long id)
        {
            return await _db.Notifications.FindAsync(id);
        }

        public async Task<IEnumerable<Notification>> GetByUser(string userId)
        {
            return await _db.Notifications.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<bool> MarkAsRead(long id)
        {
            var noti = await _db.Notifications.FindAsync(id);
            if (noti != null) { 
                noti.IsRead = true;
                noti.DateRead = DateTime.UtcNow;
                _db.Notifications.Update(noti);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> Remove(long id)
        {
            var noti = await _db.Notifications.FindAsync(id);
            if (noti != null)
            {
                _db.Notifications.Remove(noti);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
