using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commers_Adelia.Repository
{
    public interface IOrderHistory
    {
        Task<OrderHistory> GetHistoryAsync(string orderNo);
        Task<IActionResult> Create(string orderNo, string text, int statusId);
        Task<IActionResult> Remove(string orderNo);
    }
}
