using KioskService.Core.DTO;
using KioskService.Core.Models;
using KioskService.Persistance.Database;
using KioskService.Persistance.Services;
using KioskService.Tests.Utils;
using Microsoft.EntityFrameworkCore;

namespace KioskService.Tests.Services
{
    public class PaymentServiceTests
    {
        PaymentService service;
        DatabaseContext context;
        public PaymentServiceTests()
        {
            DbContextOptions<DatabaseContext> options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("test").Options;

            context = new DatabaseContext(options);

            service = new PaymentService(context, new Persistance.Utils.Mappers());
        }

        [Fact]
        public async Task AddPaymentShouldBeSuccess()
        {
            Payment payment = new MockPayment();

            int newId = 0;

            Exception? ex = await Record.ExceptionAsync(async () =>
            {
                newId = await service.SavePayment(payment);
            });

            Assert.True(ex is null && newId != 0);
        }

        [Fact]
        public async Task ProceedRefundShouldBeSuccess()
        {
            Payment payment = new MockPayment();

            int newId = await service.SavePayment(payment);
            bool isCompleted = false;

            Exception? ex = await Record.ExceptionAsync(async () =>
            {
                await service.ProceedRefund(newId);

                isCompleted = context.Payment.First(x => x.Id == newId).IsValid == false;

            });

            Assert.True(ex is null && isCompleted);
        }

        [Fact]
        public async Task DoubleRefundShouldThrowException()
        {
            Payment payment = new MockPayment();

            int newId = await service.SavePayment(payment);

            Exception? ex = await Record.ExceptionAsync(async () =>
            {
                await service.ProceedRefund(newId);
                await service.ProceedRefund(newId);
            });

            Assert.NotNull(ex);
        }

        [Fact]
        public async Task GetPaymentShouldNotBeNull()
        {
            Payment payment = new MockPayment();

            int newId = await service.SavePayment(payment);

            Payment? entity = await service.GetPayment(newId);

            Assert.NotNull(entity);
        }

        [Fact]
        public async Task GetUnexistingPaymentShouleBeNull()
        {
            int unexistingId = int.MinValue;

            Payment? entity = await service.GetPayment(unexistingId);

            Assert.Null(entity);
        }

        [Fact]
        public async Task GetPaymentsPageShouldReturnSomeResults()
        {
            for (int _ = 0; _ < 10; _++)
            {
                Payment newPayment = new MockPayment() { deviceId = "1" };
                await service.SavePayment(newPayment);
            }

            PaginatedList<PaymentPreview> page = await service.GetPreviousTransactions("1", 1);

            Assert.Equal(10, page.results.Count);
        }

        [Fact]
        public async Task PageItemsCountShouldBeAlwaysLessThan10()
        {
            for (int _ = 0; _ < 12; _++)
            {
                Payment newPayment = new MockPayment() { deviceId = "1" };
                await service.SavePayment(newPayment);
            }

            PaginatedList<PaymentPreview> page = await service.GetPreviousTransactions("1", 1);

            Assert.Equal(10, page.results.Count);
        }

        [Fact]
        public async Task PageWithIndexLessThan1ShouldReturnFirstPage()
        {
            for (int _ = 0; _ < 10; _++)
            {
                Payment newPayment = new MockPayment() { deviceId = "1" };
                await service.SavePayment(newPayment);
            }


            PaginatedList<PaymentPreview> wrongPage = await service.GetPreviousTransactions("1", -1);
            PaginatedList<PaymentPreview> newPage = await service.GetPreviousTransactions("1", 1);

            Assert.Equal(newPage, wrongPage, comparer:new PaymentPaginatedListComparer());
        }

    }
}
