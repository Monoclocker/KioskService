using KioskService.Core.DTO;
using KioskService.Tests.Utils;
using KioskService.WEB.HubFilters;
using KioskService.WEB.HubInterfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace KioskService.Tests.Filters
{
    public class DesktopFiltersTests
    {
        DesktopHubFilter filter;
        Mock<IHubCallerClients<IDesktopHub>> desktopHubClients;
        Mock<ILogger<DesktopHubFilter>> mockLogger = new Mock<ILogger<DesktopHubFilter>>();
        Mock<IDesktopHub> desktopHubEvents = new Mock<IDesktopHub>();
        public DesktopFiltersTests()
        {
            desktopHubClients = new Mock<IHubCallerClients<IDesktopHub>>();

            desktopHubClients.Setup(x => x.Caller).Returns(desktopHubEvents.Object);

            filter = new DesktopHubFilter(mockLogger.Object);
        }

        [Fact]
        public async Task OnErrorShouldNotifyCallerOnlyOnce()
        {
            Mock<Hub<IDesktopHub>> hubMock = new();

            Hub<IDesktopHub> hub = hubMock.Object;
            hub.Clients = desktopHubClients.Object;

            HubInvocationContext context = new HubInvocationContext(new MockHubCallerContext().context,
                new Mock<IServiceProvider>().Object, hub, new Mock<MethodInfo>().Object, []);

            Mock<Func<HubInvocationContext, ValueTask<object?>>> mockNext
                = new Mock<Func<HubInvocationContext, ValueTask<object?>>>();

            mockNext.Setup(x => x.Invoke(context)).Throws<Exception>();

            await filter.InvokeMethodAsync(context, mockNext.Object);

            desktopHubClients.Verify(clients =>
                clients.Caller.Error(It.IsAny<Response>()), Times.Once);

        }

    }
}
