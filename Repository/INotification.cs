using E_Commers_Adelia.Models;

namespace E_Commers_Adelia.Repository
{
    public interface INotification
    {
        Task<bool> Create(Notification notification);
        Task<IEnumerable<Notification>> GetByUser(string userId);
        Task<Notification> GetById(int id);
        Task<bool> Remove(int id);
        Task<bool> MarkAsRead(int id);
    }
}
