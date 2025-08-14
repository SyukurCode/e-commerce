using E_Commers_Adelia.Models;

namespace E_Commers_Adelia.Service
{
    public interface IReceiptVerificationService
    {
        Task<VerificationResult> VerifyReceiptAsync(string dbimagePath, decimal expectedAmount, DateTime expectedDateTime);
    }
}
