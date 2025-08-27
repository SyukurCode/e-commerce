using E_Commers_Adelia.Models;

namespace E_Commers_Adelia.Service
{
    public interface IGeminiService
    {
        Task<ReceiptData> ExtractReceiptAsync(string imagePath, decimal amount, DateTime purchaseDate);
    }
}
