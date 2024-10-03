using Bogus;
using KioskService.Core.DTO;
using KioskService.Core.Enums;
using KioskService.Core.Models;
using KioskService.Tests.Utils;
using KioskService.WEB.HubInterfaces;
using KioskService.WEB.Hubs;
using KioskService.WEB.Storages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;

namespace KioskService.Tests.Hubs
{
    public class DesktopHubTests
    {
        DesktopHub hub;
        Mock<IHubCallerClients<IKioskHub>> mockKioskClients;
        Mock<IHubCallerClients<IDesktopHub>> mockDesktopClients;
        InMemoryConnectionStorage connectionStorage = new InMemoryConnectionStorage();
        Faker faker = new Faker();
        public DesktopHubTests()
        {
            Mock<ILogger<DesktopHub>> mockLogger = new Mock<ILogger<DesktopHub>>();
            Mock<IKioskHub> mockKioskHubEvents = new Mock<IKioskHub>();
            Mock<IDesktopHub> mockDesktopHubEvents = new Mock<IDesktopHub>();
            Mock<IHubContext<KioskHub, IKioskHub>> mockKioskHub
                = new Mock<IHubContext<KioskHub, IKioskHub>>();

            (HubCallerContext context, string? id) = new MockHubCallerContext().GetContext();


            mockKioskClients = new Mock<IHubCallerClients<IKioskHub>>();
            mockDesktopClients = new Mock<IHubCallerClients<IDesktopHub>>();

            mockDesktopClients.Setup(x => x.Caller).Returns(mockDesktopHubEvents.Object);
            mockKioskClients.Setup(x => x.User(It.Is<string>(x => x != null))).Returns(mockKioskHubEvents.Object);

            mockKioskHub.Setup(x => x.Clients).Returns(mockKioskClients.Object);
            mockKioskHub.Setup(x => x.Clients.User(id!)).Returns(mockKioskHubEvents.Object);


            hub = new DesktopHub(mockKioskHub.Object, connectionStorage,
                new MockPaymentService().service, new MockResultsService().service, 
                mockLogger.Object)
            {
                Clients = mockDesktopClients.Object,
                Context = new MockHubCallerContext().GetContext().context
            };
        }

        [Fact]
        public async Task OnConnectedShouldNotifyCallerAboutConnectionsOnlyOnce()
        {
            await hub.OnConnectedAsync();

            mockDesktopClients.Verify(clients =>
                clients.Caller.ConnectedToService(It.IsAny<Response<IEnumerable<string>>>()), Times.Once());
        }

        [Fact]
        public async Task OnResultsRequestShouldNotifyKioskOnlyOnce()
        {
            Request mockRequest = new Request()
            {
                deviceId = faker.Random.Int().ToString(),
                kioskType = faker.Random.Enum<KioskTypes>()
            };

            await hub.ResultsRequest(mockRequest);

            mockKioskClients.Verify(clients =>
                clients.User(mockRequest.deviceId).ResultsRequest(mockRequest), Times.Once());
        }

        [Fact]
        public async Task OnSendSettingsShouldNotifyOnlyOnce()
        {
            Request<object> mockRequest = new Request<object>()
            {
                deviceId = faker.Random.Int().ToString(),
                data = It.IsAny<object>()
            };

            await hub.SendSettings(mockRequest);

            mockKioskClients.Verify(clients =>
                clients.User(mockRequest.deviceId).SendSettings(mockRequest), Times.Once());
        }

        [Fact]
        public async Task OnGetPreviousTransactionsShouldNotifyCallerOnlyOnce()
        {
            Request<int> mockRequest = new Request<int>()
            {
                deviceId = faker.Random.Int().ToString(),
                data = faker.Random.Int()
            };

            await hub.GetPreviousPayment(mockRequest);

            mockDesktopClients.Verify(clients =>
                clients.Caller.SavedTransactions(It.IsAny<Response<PaginatedList<PaymentPreview>>>()), Times.Once());
        }

        [Fact]
        public async Task OnGetPreviousResultsShouldNotifyCallerOnlyOnce()
        {
            Request<int> mockRequest = new Request<int>()
            {
                deviceId = faker.Random.Int().ToString(),
                data = faker.Random.Int()
            };

            await hub.GetPreviousResults(mockRequest);

            mockDesktopClients.Verify(clients =>
                clients.Caller.SavedResults(It.IsAny<Response<PaginatedList<ResultsPreview>>>()), Times.Once());
        }

        [Fact]
        public async Task OnGetPaymentShouldNotifyCallerOnlyOnce()
        {
            Request<int> mockRequest = new Request<int>()
            {
                deviceId = faker.Random.Int().ToString(),
                data = faker.Random.Int()
            };

            await hub.GetPayment(mockRequest);

            mockDesktopClients.Verify(clients =>
                clients.Caller.GetPayment(It.IsAny<Response<Payment>>()), Times.Once());
        }

        [Fact]
        public async Task OnGetResultsShouldNotifyCallerOnlyOnce()
        {
            Request<int> mockRequest = new Request<int>()
            {
                deviceId = faker.Random.Int().ToString(),
                data = faker.Random.Int()
            };

            await hub.GetResults(mockRequest);

            mockDesktopClients.Verify(clients =>
                clients.Caller.GetResults(It.IsAny<Response<Results>>()), Times.Once());
        }
    }
}
