using Bogus;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace KioskService.Tests.Utils
{
    public class MockHubCallerContext
    {
        public HubCallerContext context;

        Faker faker = new Faker();

        public MockHubCallerContext()
        {
            Mock<HubCallerContext> mockContext = new Mock<HubCallerContext>();

            mockContext.Setup(x => x.UserIdentifier).Returns(faker.Random.Int().ToString());

            context = mockContext.Object;
        }

        public (HubCallerContext context, string? id) GetContext()
        {
            return (this.context, this.context.UserIdentifier);
        } 
    }
}
