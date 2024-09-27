using KioskService.Core.DTO;
using KioskService.Persistance.Database;
using KioskService.WEB.Interfaces;
using KioskService.WEB.Services;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB.Hubs
{
    public class KioskHub: Hub
    {
        DataSender dataSender;
        ILogger<KioskHub> logger;
        IConnectionStorage connectionStorage;
        DatabaseContext context;

        public KioskHub(
            DataSender dataSender,
            ILogger<KioskHub> logger,
            IConnectionStorage connectionStorage,
            DatabaseContext context
        ) 
        {
            this.dataSender = dataSender;
            this.logger = logger;
            this.connectionStorage = connectionStorage;
            this.context = context;
        }

        public override async Task OnConnectedAsync()
        {
            string deviceId = Context.UserIdentifier!;

            await dataSender.NotifyDesktopAboutKioskConnect(deviceId);

            await base.OnConnectedAsync();
        }

        public async Task TransportKioskStateResponse(Response body)
        {
            await dataSender.TransportKioskStateToDesktop(body);
        }

        public async Task TransportResultsResponse(Response body)
        {
            await dataSender.TransportResultsToDesktop(body);
        }

        public async Task ProceedRefund(Request body)
        {
           await dataSender.ProceedRefundToDesktop(body);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string deviceId = Context.UserIdentifier!;

            connectionStorage.Delete(deviceId);

            await dataSender.NotifyDesktopAboutKioskDisconnect(deviceId);

            logger.LogInformation($"Киоск {deviceId} отключён");

            if (exception is not null)
            {
                logger.LogError($"Ошибка {exception.GetType()}: {exception.Message}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
