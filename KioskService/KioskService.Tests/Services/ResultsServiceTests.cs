using KioskService.Core.DTO;
using KioskService.Core.Models;
using KioskService.Persistance.Database;
using KioskService.Persistance.Services;
using KioskService.Tests.Utils;
using Microsoft.EntityFrameworkCore;

namespace KioskService.Tests.Services
{
    public class ResultsServiceTests
    {
        ResultsService service;
        DatabaseContext context;

        public ResultsServiceTests()
        {
            DbContextOptions<DatabaseContext> options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("test")
                .Options;

            context = new DatabaseContext(options);

            service = new ResultsService(new Persistance.Utils.Mappers(), context);
        }

        [Fact]
        public async Task CreateResultsShouldBeSuccess()
        {
            Results results = new MockResults();

            int newId = 0;

            Exception? ex = await Record.ExceptionAsync(async () =>
            {
                newId = await service.SaveResults(results);
            });

            Assert.True(ex is null && newId != 0);
        }

        [Fact]
        public async Task GetPageShouldReturnResults()
        {
            for (int _ = 0; _ < 10; _++)
            {
                await service.SaveResults(new MockResults() { deviceId = "1" });
            }

            PaginatedList<ResultsPreview> page = await service.GetPreviousResults("1");

            Assert.True(page.results.Count > 0);
        }

        [Fact]
        public async Task GetResultsShouldNotBeNull()
        {
            Results results = new MockResults();

            int newId = await service.SaveResults(results);

            Results? foundedResults = await service.GetResults(newId);

            Assert.NotNull(foundedResults);
        }

        [Fact]
        public async Task GetUnexistingResultsShouldBeNull()
        {
            int newId = -1;

            Results? foundedResults = await service.GetResults(newId);

            Assert.Null(foundedResults);
        }

        [Fact]
        public async Task PageItemsCountShouldBeAlwaysLessThan10()
        {
            for (int _ = 0; _ < 13; _++)
            {
                await service.SaveResults(new MockResults() { deviceId = "1" });
            }

            PaginatedList<ResultsPreview> page = await service.GetPreviousResults("1");

            Assert.True(page.results.Count == 10);
        }

        [Fact]
        public async Task PageWithIndexLessThan1ShouldReturnFirstPage()
        {
            for (int _ = 0; _ < 10; _++)
            {
                await service.SaveResults(new MockResults() { deviceId = "1" });
            }

            PaginatedList<ResultsPreview> truePage = await service.GetPreviousResults("1");
            PaginatedList<ResultsPreview> wrongPage = await service.GetPreviousResults("1", -1);

            Assert.Equal(truePage, wrongPage, comparer: new ResultsPaginatedListComparer());
        }
    }
}
