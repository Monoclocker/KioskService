using Bogus;
using KioskService.Core.Models;

namespace KioskService.Tests.Utils
{
    public class MockResults : Results
    {
        Faker faker = new Faker("ru");
        public MockResults() 
        {
            this.check = faker.Lorem.Paragraph(4);
            this.sum = faker.Random.Double(max: double.MaxValue);
            this.deviceId = faker.Random.Int().ToString();
            this.utcTime = DateTime.UtcNow;
            this.localTime = DateTime.Now;
        }
    }
}
