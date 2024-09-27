using KioskService.Core.DTO;
using KioskService.Core.Models;
using KioskService.WEB.Hubs;
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

        public async Task NotifyDesktopAboutKioskDisconnect(string deviceId)
        {
            await desktopHub.Clients.All.SendAsync(DesktopEventsNames.KioskDisconnectedEvent, deviceId);
        }

        public async Task SendPaymentSaveResultToDesktop(Response<Guid> response)
        {
            await desktopHub.Clients.All.SendAsync(DesktopEventsNames.SendSavePaymentResultEvent, response);
        }

        public async Task SendRefundMessageToKiosk(Request<Guid> request)
        {
            await kioskHub.Clients
                .User(request.deviceId)
                .SendAsync(KioskEventsNames.ProceedRefundEvent, request);
        }

        public async Task SendRefundResultToDesktop(Response<Guid> response)
        {
            await desktopHub.Clients
                .All
                .SendAsync(DesktopEventsNames.RefundResponseEvent, response);
        }

        public async Task SendSettingsToKiosk(Request<Settings> request)
        {
            await kioskHub.Clients.User(request.deviceId)
                .SendAsync(KioskEventsNames.SendSettingsEvent, request);
        }

        public async Task SendSetSettingsResultToDesktop(Response response)
        {
            await desktopHub.Clients.All
                .SendAsync(DesktopEventsNames.SetSettingsResponseEvent, response);
        }
    }
}
