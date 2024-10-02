using Bogus;
using KioskService.Core.DTO;
using KioskService.Core.Exceptions;
using KioskService.Core.Interfaces;
using KioskService.Core.Models;
using Moq;

namespace KioskService.Tests.Utils
{
    public class MockPaymentService
    {
        public IPaymentService service { get; }

        Faker faker = new Faker();

        Dictionary<int, int> callCounts = new Dictionary<int, int>();


        public MockPaymentService()
        {
            Mock<IPaymentService> mockService = new Mock<IPaymentService>();

            mockService.Setup(x => x.SavePayment(It.IsAny<Payment>()))
                .ReturnsAsync(faker.Random.Int(min: 1));

            mockService.Setup(x => x.ProceedRefund(It.IsAny<int>()))
                .Callback<int>((arg) =>
                {
                    if (!callCounts.ContainsKey(arg))
                    {
                        callCounts.Add(arg, 0);
                        return;
                    }

                    if (callCounts[arg] == 1)
                        throw new InvalidPaymentTranscation();

                    callCounts[arg]++;
                    return;
                });

            mockService.Setup(x => x.GetPreviousTransactions(It.IsAny<string>(), It.IsAny<int>()))
                .Callback<string, int>((id, page) =>
                {
                    List<Payment> list = new List<Payment>();

                    for (int _ = 0; _ < 10; _++)
                    {
                        list.Add(new MockPayment() { deviceId = id, id = faker.Random.Int(min: 1)});
                    }

                    mockService.Setup(x => x.GetPreviousTransactions(id, page))
                    .ReturnsAsync(new PaginatedList<PaymentPreview>()
                    {
                        pagesCount = faker.Random.Int(min: 1),
                        results = list.Select(x => new PaymentPreview()
                        { id = x.id, localDate = x.localTime, sum = x.sum }).ToList()
                    });
                });

            mockService.Setup(x => x.GetPayment(It.IsAny<int>()))
                .Callback<int>((arg) =>
                {
                    mockService.Setup(x => x.GetPayment(arg))
                    .ReturnsAsync(new MockPayment() { id = arg });
                });

            service = mockService.Object;

        }

    }
}
