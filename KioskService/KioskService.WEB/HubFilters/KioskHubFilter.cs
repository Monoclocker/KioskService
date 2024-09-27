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
        ILogger<KioskHubFilter> logger;
        public KioskHubFilter(IHubContext<DesktopHub> desktopHub, ILogger<KioskHubFilter> logger)
        {
            this.desktopHub = desktopHub;
            this.logger = logger;
        }

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

                logger.LogError($"При вызове метода {invocationContext.HubMethodName} произошла ошибка");
                logger.LogError(ex.GetType().ToString());
                logger.LogError(ex.StackTrace);

                Response<object> response = new Response<object>()
                {
                    message = "Произошла ошибка при обработке события киоска",
                    stackTrace = stackTrace,
                    statusCode = 500,
                    data = ex.Data
                };

                string responseBody = JsonSerializer.Serialize(response);

                Console.WriteLine(responseBody);

                Console.WriteLine(invocationContext.HubMethod);

                await desktopHub.Clients.All.SendAsync(DesktopEventsNames.KioskErrorEvent, responseBody);

                return null;
            }
        }

        public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            string? deviceId = context.Context.UserIdentifier;
            try
            {
                if (deviceId == null)
                {
                    await context.Hub.Clients.Caller.SendAsync(KioskEventsNames.KioskConnectionErrorEvent);
                    throw new Exception();
                }
                await next(context);
            }
            catch(Exception ex)
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

                Console.WriteLine(responseBody);


                await desktopHub.Clients.All.SendAsync(DesktopEventsNames.KioskErrorEvent, responseBody);

                await Task.CompletedTask;
            }

        }
    }
}
