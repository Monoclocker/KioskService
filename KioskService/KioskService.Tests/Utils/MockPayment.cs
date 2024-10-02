using Bogus;
using KioskService.Core.Models;

namespace KioskService.Tests.Utils
{
    public class MockPayment : Payment
    {
        Faker faker = new Faker("ru");
        public MockPayment() 
        {
            this.deviceId = faker.Database.Random.Int().ToString();
            this.organization = faker.Company.CompanyName();
            this.sum = faker.Random.Double(max: double.MaxValue);
            this.status = faker.Finance.TransactionType();
            this.idemptencyENR = faker.Random.Guid().ToString();
            this.localTime = DateTime.Now;
            this.check = faker.Lorem.Paragraph();
            this.paymentWay = faker.Lorem.Lines(lineCount: 1);
            this.utcTime = DateTime.UtcNow;
            this.paymentObjects.AddRange(faker.Lorem.Words());
        }
    }
}
