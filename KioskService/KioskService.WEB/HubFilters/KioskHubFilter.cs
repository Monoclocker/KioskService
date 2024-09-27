using KioskService.Core.DTO;
using KioskService.WEB.Hubs;
using KioskService.WEB.Utils;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace KioskService.WEB.HubFilters
{
    public class KioskHubFilter : IHubFilter
    {
        IHubContext<DesktopHub> desktopHub;
        public KioskHubFilter(IHubContext<DesktopHub> desktopHub)
        {
            this.desktopHub = desktopHub;
        }

        public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, 
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            string? deviceId = invocationContext.Context.UserIdentifier;
            try
            {
                if (deviceId == null)
                {
                    await invocationContext.Hub.Clients.Caller.SendAsync(KioskEventsNames.KioskConnectionErrorEvent);
                    throw new Exception();
                }

                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                string? stackTrace = ex.StackTrace;

                Response<object> response = new Response<object>()
                {
                    message = "Произошла ошибка при обработке события киоска",
                    stackTrace = stackTrace,
                    statusCode = 500,
                    data = ex.Data
                };

                string responseBody = JsonSerializer.Serialize(response);

                await desktopHub.Clients.All.SendAsync(DesktopEventsNames.KioskErrorEvent, responseBody);

                return null;
            }
        }
    }
}
