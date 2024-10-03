using Bogus;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace KioskService.Tests.Utils
{
    public class MockHubCallerContext
    {
        public HubCallerContext context;

        Faker faker = new Faker();

        public MockHubCallerContext(bool isNull = false)
        {
            Mock<HubCallerContext> mockContext = new Mock<HubCallerContext>();

            mockContext.Setup(x => x.UserIdentifier).Returns(isNull ? null : faker.Random.Int().ToString());

            context = mockContext.Object;
        }

        public (HubCallerContext context, string? id) GetContext()
        {
            return (this.context, this.context.UserIdentifier);
        } 
    }
}
