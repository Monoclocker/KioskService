using Bogus;
using KioskService.Core.DTO;
using KioskService.Core.Models;
using KioskService.Tests.Utils;
using KioskService.WEB.HubInterfaces;
using KioskService.WEB.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;

namespace KioskService.Tests.Hubs
{
    public class KioskHubTests
    {
        KioskHub hub;
        Mock<IHubCallerClients<IKioskHub>> mockKioskClients;
        Mock<IHubCallerClients<IDesktopHub>> mockDesktopClients;
        public KioskHubTests()
        {
            Mock<ILogger<KioskHub>> mockLogger = new Mock<ILogger<KioskHub>>();
            Mock<IKioskHub> mockKioskHubEvents = new Mock<IKioskHub>();
            Mock<IDesktopHub> mockDesktopHubEvents = new Mock<IDesktopHub>();
            Mock<IHubContext<DesktopHub, IDesktopHub>> mockDesktopHub
                = new Mock<IHubContext<DesktopHub, IDesktopHub>>();

            mockKioskClients = new Mock<IHubCallerClients<IKioskHub>>();
            mockDesktopClients = new Mock<IHubCallerClients<IDesktopHub>>();


            mockDesktopHub.Setup(x => x.Clients.All).Returns(mockDesktopHubEvents.Object);
            mockDesktopClients.Setup(x => x.All).Returns(mockDesktopHub.Object.Clients.All);
            mockKioskClients.Setup(x => x.Caller).Returns(mockKioskHubEvents.Object);
            mockDesktopHub.Setup(x => x.Clients).Returns(mockDesktopClients.Object);

            hub = new KioskHub(mockDesktopHub.Object, mockLogger.Object,
                new MockPaymentService().service, new MockResultsService().service)
            { 
                Clients = mockKioskClients.Object,
                Context = new MockHubCallerContext().GetContext().context
            };
        }

        [Fact]
        public async Task OnConnectionWithDeviceIdShouldSendMessageToDesktop()
        {
            await hub.OnConnectedAsync();

            mockDesktopClients.Verify(clients => 
                clients.All.KioskConnected(It.IsAny<Response<string>>()), Times.Once());
        }

        [Fact]
        public async Task OnPaymentShouldNotifyDesktopOnlyOnce()
        {
            await hub.SavePayment(new Request<Payment>()
            {
                deviceId = hub.Context.UserIdentifier!,
                kioskType = Core.Enums.KioskTypes.Default,
                data = new MockPayment()
            });

            mockDesktopClients.Verify(clients =>
                clients.All.SendSavePaymentResult(It.IsAny<Response<PaymentPreview>>()), Times.Once());
        }

        [Fact]
        public async Task OnResultsRequestShouldSendResponseOnlyOnce() 
        {
            Response<Results> mockResponse = new Response<Results>()
            {
                statusCode = 200,
                data = new MockResults(),
                deviceId = hub.Context.UserIdentifier!,
            };

            await hub.ResultsResponse(mockResponse);

            mockDesktopClients.Verify(clients => 
                clients.All.NewResults(It.IsAny<Response<ResultsPreview>>()), Times.Once());
        }

        [Fact]
        public async Task OnRefundShouldNotifyDesktopOnlyOnce()
        {
            Response<int> mockResponse = new Response<int>()
            {
                statusCode = 200,
                data = 1,
                deviceId = hub.Context.UserIdentifier!,
            };

            await hub.RefundResult(mockResponse);

            mockDesktopClients.Verify(clients =>
                clients.All.RefundResponse(It.IsAny<Response<int>>()), Times.Once());
        }

        [Fact]
        public async Task OnSetSettingsShouldNotifyDesktopOnlyOnce()
        {
            await hub.SetSettingsResult(It.IsAny<Response<int>>());

            mockDesktopClients.Verify(clients =>
                clients.All.SetSettingsResponse(It.IsAny<Response>()), Times.Once());
        }
    }
}
