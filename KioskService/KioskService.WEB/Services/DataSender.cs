using KioskService.Core.DTO;
using KioskService.WEB.Hubs;
using KioskService.WEB.Interfaces;
using KioskService.WEB.Utils;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB.Services
{
    public class DataSender
    {
        IHubContext<KioskHub> kioskHub;
        IHubContext<DesktopHub> desktopHub;

        public DataSender(
            IHubContext<KioskHub> kioskHub,
            IHubContext<DesktopHub> desktopHub
        )
        {
            this.kioskHub = kioskHub;
            this.desktopHub = desktopHub;
        }

        public async Task NotifyDesktopAboutKioskConnect(string deviceId)
        {
            await desktopHub.Clients
                .All
                .SendAsync(DesktopEventsNames.KioskConnectedEvent, deviceId);
        }

        public async Task RequestKioskStateFromKiosk(Request request)
        {
           await kioskHub.Clients
                .User(request.deviceId)
                .SendAsync(KioskEventsNames.KioskGetStateRequestEvent);
        }

        public async Task SetKioskSettingsOnKiosk(Request request)
        {
            await kioskHub.Clients
                .User(request.deviceId)
                .SendAsync(KioskEventsNames.SetKioskSettingsEvent, request);
        }

        public async Task RequestKioskResultsOnKiosk(Request request)
        {
            await kioskHub.Clients
               .User(request.deviceId)
               .SendAsync(KioskEventsNames.KioskResultsRequestEvent, request);
        }

        public async Task TransportKioskStateToDesktop(Response response)
        {
            await desktopHub.Clients
                .All
                .SendAsync(DesktopEventsNames.KioskSettingsResponseEvent, response);
        }

        public async Task TransportResultsToDesktop(Response response)
        {
            await desktopHub.Clients
                .All
                .SendAsync(DesktopEventsNames.KioskResultsResponseEvent, response);
        }

        public async Task ProceedRefundToDesktop(Request request)
        {
            await desktopHub.Clients
               .All
               .SendAsync(DesktopEventsNames.KioskRefundRequestEvent, request);
        }

        public async Task NotifyDesktopAboutKioskDisconnect(string deviceId)
        {
            await desktopHub.Clients.All.SendAsync(DesktopEventsNames.KioskDisconnectedEvent, deviceId);
        }
    }
}
