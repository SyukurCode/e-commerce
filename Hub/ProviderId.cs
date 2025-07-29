using Microsoft.AspNetCore.SignalR;

namespace E_Commers_Adelia.Hub
{
    public class ProviderId : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var httpContext = connection.GetHttpContext();
            return httpContext?.Request.Query["userid"].FirstOrDefault();
        }
    }
}
