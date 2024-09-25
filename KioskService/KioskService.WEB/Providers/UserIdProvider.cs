using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB.Providers
{
    public class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.GetHttpContext()?.Request.Query["deviceId"];
        }
    }
}
