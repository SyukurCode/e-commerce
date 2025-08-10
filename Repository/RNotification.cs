using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Serilog;
namespace E_Commers_Adelia.Repository
{
    public class RNotification : INotification
    {
        private readonly ApplicationDbContext _db;
        public RNotification(ApplicationDbContext db) { 
            _db = db;
        }

        public async Task<bool> Create(Notification notification)
        {
            if (notification == null)
            {
                return false;
            }
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
            return false;
        }

        public async Task<Notification> GetById(int id)
        {
            return await _db.Notifications.FindAsync(id);
        }

        public async Task<IEnumerable<Notification>> GetByUser(string userId)
        {
            return await _db.Notifications.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<bool> MarkAsRead(int id)
        {
            var noti = await _db.Notifications.FindAsync(id);
            if (noti != null) { 
                noti.IsRead = true;
                _db.Notifications.Update(noti);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> Remove(int id)
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
