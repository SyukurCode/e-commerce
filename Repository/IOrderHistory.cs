using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace E_Commers_Adelia.Repository
{
    public interface IOrderHistory
    {
        Task<IEnumerable<OrderHistory>> GetHistoryAsync(string orderNo);
        Task<IActionResult> CreateAsync(string orderNo, string text, int statusId);
        Task<IActionResult> RemoveAsync(string orderNo);
    }
}
