using KioskService.Core.DTO;
using KioskService.WEB.Hubs;
using KioskService.WEB.Utils;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace KioskService.WEB.HubFilters
{
    public class DesktopHubFilter : IHubFilter
    {
        public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            try
            {
                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                string? stackTrace = ex.StackTrace;

                Response response = new Response()
                {
                    message = "Произошла ошибка при обработке события клиента",
                    stackTrace = stackTrace,
                    statusCode = 500,
                    data = ex.Data
                };

                string responseBody = JsonSerializer.Serialize(response);

                await invocationContext.Hub.Clients.All.SendAsync(DesktopEventsNames.KioskErrorEvent, responseBody);

                return null;
            }
        }
    }
}
