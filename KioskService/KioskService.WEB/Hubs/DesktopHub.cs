using KioskService.Core.DTO;
using KioskService.Persistance.Database;
using KioskService.WEB.Interfaces;
using KioskService.WEB.Services;
using KioskService.WEB.Utils;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB.Hubs
{
    public class DesktopHub : Hub
    {
        DataSender dataSender;
        ILogger<DesktopHub> logger;
        IConnectionStorage connectionStorage;
        DatabaseContext context;

        public DesktopHub(
            DataSender dataSender,
            IConnectionStorage connectionStorage,
            ILogger<DesktopHub> logger
,
            DatabaseContext context

        )
        {
            this.dataSender = dataSender;
            this.connectionStorage = connectionStorage;
            this.logger = logger;
            this.context = context;
        }

        public override async Task OnConnectedAsync()
        {
            
            await Clients.Caller.SendAsync(DesktopEventsNames.ConnectedToServiceEvent, connectionStorage);
            await base.OnConnectedAsync();
        }

        public async Task SetKioskSettings(Request<object> body)
        {
            await dataSender.SetKioskSettingsOnKiosk(body);
        }

        public async Task CheckKioskState(Request dto)
        {
            await dataSender.RequestKioskStateFromKiosk(dto);
        }

        public async Task CheckKioskResults(Request body)
        {
            await dataSender.RequestKioskResultsOnKiosk(body);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            logger.LogInformation($"Приложение {Context.ConnectionId} отключено");

            if (exception is not null)
            {
                logger.LogError($"Ошибка {exception.GetType()}: {exception.Message}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
