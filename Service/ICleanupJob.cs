using E_Commers_Adelia.Models;

namespace E_Commers_Adelia.Service
{
    public interface ICleanupJob
    {
        Task RunCleaningAsync();
        Task<List<CleaningItem>> ListItemToCleanAsync();
    }
}
