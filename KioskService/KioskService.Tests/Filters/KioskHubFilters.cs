using KioskService.Core.DTO;
using KioskService.Tests.Utils;
using KioskService.WEB.HubFilters;
using KioskService.WEB.HubInterfaces;
using KioskService.WEB.Hubs;
using KioskService.WEB.Interfaces;
using KioskService.WEB.Storages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace KioskService.Tests.Filters
{
    public class KioskHubFilters
    {
        KioskHubFilter filter;
        Mock<IHubCallerClients<IDesktopHub>> desktopHubClients = new();
        Mock<IHubCallerClients<IKioskHub>> kioskHubClients = new();
        Mock<IHubContext<DesktopHub, IDesktopHub>> desktopHub = new();

        Mock<ILogger<KioskHubFilter>> mockLogger = new();
        Mock<IDesktopHub> desktopHubEvents = new();
        Mock<IKioskHub> kioskHubEvents = new();

        IConnectionStorage connectionStorage = new InMemoryConnectionStorage();

        Hub<IKioskHub> hub;

        public KioskHubFilters()
        {
            desktopHubClients.Setup(x => x.All).Returns(desktopHubEvents.Object);

            desktopHub.Setup(x => x.Clients).Returns(desktopHubClients.Object);

            kioskHubClients.Setup(x => x.Caller).Returns(kioskHubEvents.Object);

            filter = new KioskHubFilter(desktopHub.Object, mockLogger.Object, connectionStorage);


            Mock<Hub<IKioskHub>> hubMock = new();
            hub = hubMock.Object;
            hub.Clients = kioskHubClients.Object;
        }

        [Fact]
        public async Task OnErrorShouldNotifyDesktopOnlyOnce()
        {
            HubInvocationContext context = new HubInvocationContext(new MockHubCallerContext().context,
                new Mock<IServiceProvider>().Object, hub, new Mock<MethodInfo>().Object, []);

            Mock<Func<HubInvocationContext, ValueTask<object?>>> mockNext
                = new Mock<Func<HubInvocationContext, ValueTask<object?>>>();

            mockNext.Setup(x => x.Invoke(context)).Throws<Exception>();

            await filter.InvokeMethodAsync(context, mockNext.Object);

            desktopHubClients.Verify(clients =>
                clients.All.KioskError(It.IsAny<Response>()), Times.Once());
        }

        [Fact]
        public async Task OnConnectionShouldAddNewIdInStorageOnlyOnce()
        {
            (HubCallerContext callerContext, string? id) = new MockHubCallerContext().GetContext();

            HubLifetimeContext context 
                = new HubLifetimeContext(callerContext, new Mock<IServiceProvider>().Object, hub);

            Mock<Func<HubLifetimeContext, Task>> mockNext = new Mock<Func<HubLifetimeContext, Task>>();

            mockNext.Setup(x => x.Invoke(context));

            int oldCount = connectionStorage.Count();

            await filter.OnConnectedAsync(context, mockNext.Object);

            Assert.True(connectionStorage.Count() == ++oldCount 
                && connectionStorage.FirstOrDefault(id) is not null);
        }

        [Fact]
        public async Task OnDisconnectShouldRemoveIdFromStorage()
        {
            (HubCallerContext callerContext, string? id) = new MockHubCallerContext().GetContext();

            HubLifetimeContext context
                = new HubLifetimeContext(callerContext, new Mock<IServiceProvider>().Object, hub);

            Mock<Func<HubLifetimeContext, Task>> mockNext = new Mock<Func<HubLifetimeContext, Task>>();
            Mock<Func<HubLifetimeContext, Exception?, Task>> mockNextOnDisconnect 
                = new Mock<Func<HubLifetimeContext, Exception?, Task>>();

            mockNext.Setup(x => x.Invoke(context));
            mockNextOnDisconnect.Setup(x => x.Invoke(context, null));

            int oldCount = connectionStorage.Count();

            await filter.OnConnectedAsync(context, mockNext.Object);
            await filter.OnDisconnectedAsync(context, null, mockNextOnDisconnect.Object);

            Assert.True(connectionStorage.Count() == oldCount
                && !connectionStorage.Contains(id));
        }

        [Fact]
        public async Task OnConnectWithWrongIdShouldNotifyCallerOnlyOnce()
        {
            HubCallerContext callerContext = new MockHubCallerContext(true).context;

            HubLifetimeContext context 
                = new HubLifetimeContext(callerContext, new Mock<IServiceProvider>().Object, hub);

            Mock<Func<HubLifetimeContext, Task>> mockNext
                = new Mock<Func<HubLifetimeContext, Task>>();

            mockNext.Setup(x => x.Invoke(context));

            await filter.OnConnectedAsync(context, mockNext.Object);

            kioskHubClients.Verify(clients =>
                clients.Caller.KioskConnectionError(It.IsAny<Response>()), Times.Once);
        }
    }
}
