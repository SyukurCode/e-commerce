using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace E_Commers_Adelia.Hub
{
    public class ProviderId : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            //var httpContext = connection.GetHttpContext();
            //return httpContext?.Request.Query["userid"].FirstOrDefault();
            //return connection.User?.Identity?.Name;
            var u = connection.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return u;

        }
    }
}
