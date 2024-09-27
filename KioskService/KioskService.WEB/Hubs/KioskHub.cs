using KioskService.Core.DTO;
using KioskService.Persistance.Database;
using KioskService.WEB.Interfaces;
using KioskService.WEB.Utils;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB.Hubs
{
    public class KioskHub: Hub
    {
        IHubContext<DesktopHub> desktopHub;
        IConnectionStorage connections;
        ILogger<KioskHub> logger;
        UnitOfWork database;

        public KioskHub(IHubContext<DesktopHub> desktopHub,
            IConnectionStorage connections,
            ILogger<KioskHub> logger,
            UnitOfWork database
            ) 
        {
            this.desktopHub = desktopHub;
            this.connections = connections;
            this.logger = logger;
            this.database = database;
        }

        public override async Task OnConnectedAsync()
        {
            string deviceId = Context.UserIdentifier!;

            connections.Add(deviceId);

            await desktopHub.Clients
                .All
                .SendAsync(DesktopEventsNames.KioskConnectedEvent, deviceId);

            await base.OnConnectedAsync();
        }

        public async Task GetSettingsFromDB(Request<object> request)
        {
            object? settings = request.data;
            Response<object> response = new Response<object>();

            if (settings is null)
            {
                response.statusCode = 404;

                await Clients.User(request.deviceId).SendAsync(KioskEventsNames.GetSettingsEvent, response);
            }
            else
            {
                response.statusCode = 200;
                response.data = settings;

                await Clients.User(request.deviceId).SendAsync(KioskEventsNames.GetSettingsEvent, response);
            }
        }

        public async Task TransportKioskStateResponse(Response body)
        {
            await desktopHub.Clients
                .All
                .SendAsync(DesktopEventsNames.KioskSettingsResponseEvent, body);
        }

        public async Task TransportResultsResponse(Response body)
        {
            await desktopHub.Clients
                .All
                .SendAsync(DesktopEventsNames.KioskResultsResponseEvent, body);
        }

        public async Task ProceedRefund(Request body)
        {
            await desktopHub.Clients
                .All
                .SendAsync(DesktopEventsNames.KioskRefundRequestEvent, body);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string deviceId = Context.UserIdentifier!;

            connections.Delete(deviceId);

            await desktopHub.Clients.All.SendAsync(DesktopEventsNames.KioskDisconnectedEvent, deviceId);

            logger.LogInformation($"Киоск {deviceId} отключён");

            if (exception is not null)
            {
                logger.LogError($"Ошибка {exception.GetType()}: {exception.Message}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
