using KioskService.Core.DTO;
using KioskService.WEB.HubInterfaces;
using KioskService.WEB.Hubs;
using KioskService.WEB.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB.HubFilters
{
    public class KioskHubFilter : IHubFilter
    {
        IHubContext<DesktopHub, IDesktopHub> desktopHub;
        ILogger<KioskHubFilter> logger;
        IConnectionStorage connectionStorage;

        public KioskHubFilter(IHubContext<DesktopHub, IDesktopHub> desktopHub, 
            ILogger<KioskHubFilter> logger,
            IConnectionStorage connectionStorage)
        {
            this.desktopHub = desktopHub;
            this.logger = logger;
            this.connectionStorage = connectionStorage;
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

                Response response = new Response()
                {
                    message = $"Произошла ошибка при обработке события киоска {ex.Message}",
                    stackTrace = stackTrace,
                    statusCode = 500
                };


                await desktopHub.Clients.All.KioskError(response);

                return null;
            }
        }

        public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            string? deviceId = context.Context.UserIdentifier;

            Response response = new Response()
            {
                date = DateTime.UtcNow
            };

            try
            {
                if (deviceId == null)
                {
                    response.statusCode = 400;
                    response.message = "некорректное deviceId";

                    var hub = context.Hub as Hub<IKioskHub>;

                    if (hub == null)
                    {
                        logger.LogError("Ошибка при оповещении клиента");
                        return;
                    }

                    await hub.Clients.Caller.KioskConnectionError(response);

                    return;
                }

                connectionStorage.Add(deviceId);

                await next(context);

            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                string? stackTrace = ex.StackTrace;

                response.message = "Произошла ошибка при обработке события киоска";
                response.stackTrace = stackTrace;
                response.statusCode = 500;

                await desktopHub.Clients.All.KioskError(response);

                await Task.CompletedTask;
            }

        }

        public async Task OnDisconnectedAsync(HubLifetimeContext context, 
            Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)
        {

            string deviceId = context.Context.UserIdentifier!;

            connectionStorage.Delete(deviceId);

            logger.LogInformation($"Киоск {deviceId} отключён");

            await next(context, exception);
        }
    }
}
