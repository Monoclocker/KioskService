using KioskService.Core.DTO;
using KioskService.Core.Models;
using KioskService.Persistance.Database;
using KioskService.WEB.Interfaces;
using KioskService.WEB.Utils;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB.Hubs
{
    public class DesktopHub : Hub
    {
        IHubContext<KioskHub> kioskHub;
        IConnectionStorage connections;
        ILogger<DesktopHub> logger;
        UnitOfWork database;

        public DesktopHub(IHubContext<KioskHub> kioskHub, 
            IConnectionStorage connections,
            ILogger<DesktopHub> logger,
            UnitOfWork database)
        {
            this.kioskHub = kioskHub;
            this.connections = connections;
            this.logger = logger;
            this.database = database;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync(DesktopEventsNames.ConnectedToServiceEvent, connections);
            await base.OnConnectedAsync();
        }

        public async Task SetKioskSettings(Request body)
        {
            await database.Settings.Add((body.data as Settings)!);

            await kioskHub.Clients
                .User(body.deviceId)
                .SendAsync(KioskEventsNames.SetKioskSettingsEvent, body);
        }

        public async Task CheckKioskState(Request dto)
        {
            await kioskHub.Clients
                .User(dto.deviceId)
                .SendAsync(KioskEventsNames.KioskGetStateRequestEvent);
        }

        public async Task CheckKioskResults(Request body)
        {
            await kioskHub.Clients
                .User(body.deviceId)
                .SendAsync(KioskEventsNames.KioskResultsRequestEvent, body);
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
