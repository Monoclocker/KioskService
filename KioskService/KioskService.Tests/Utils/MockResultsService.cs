using Bogus;
using KioskService.Core.DTO;
using KioskService.Core.Interfaces;
using KioskService.Core.Models;
using Moq;

namespace KioskService.Tests.Utils
{
    public class MockResultsService
    {
        public IResultsService service;
        Faker faker = new Faker();
        public MockResultsService()
        {
            Mock<IResultsService> mockService = new Mock<IResultsService>();

            mockService.Setup(x => x.SaveResults(It.IsAny<Results>()))
                .ReturnsAsync(faker.Random.Int(min: 1));

            mockService.Setup(x => x.GetResults(It.IsAny<int>()))
                .Callback<int>((id) =>
                {
                    mockService.Setup(x => x.GetResults(id))
                        .ReturnsAsync(new MockResults());
                });

            mockService.Setup(x => x.GetPreviousResults(It.IsAny<string>(), It.IsAny<int>()))
                .Callback<string, int>((id, page) =>
                {
                    List<Results> list = new List<Results>();

                    for (int _ = 0; _ < 10; _++)
                    {
                        list.Add(new MockResults() { deviceId = id,  });
                    }

                    mockService.Setup(x => x.GetPreviousResults(id, page))
                    .ReturnsAsync(new PaginatedList<ResultsPreview>()
                    {
                        pagesCount = faker.Random.Int(),
                        results = list.Select(x => new ResultsPreview()
                        {
                            id = faker.Random.Int(min: 1)
                        }).ToList()
                    });
                });

            service = mockService.Object;
        }
    }
}
