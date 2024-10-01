using KioskService.Core.DTO;
using KioskService.WEB.HubInterfaces;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB.HubFilters
{
    public class DesktopHubFilter : IHubFilter
    {
        ILogger<DesktopHubFilter> logger;
        public DesktopHubFilter(ILogger<DesktopHubFilter> logger)
        {
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

                Response response = new Response()
                {
                    message = $"Произошла ошибка при обработке события клиента: {ex.Message}",
                    stackTrace = stackTrace,
                    statusCode = 500
                };

                var hub = invocationContext.Hub as Hub<IDesktopHub>;

                if (hub != null)
                {
                    await hub.Clients.Caller.Error(response);
                }

                return null;
            }
        }
    }
}
