using KioskService.Core.Models;
using KioskService.Persistance.Entities;
using KioskService.Persistance.Interfaces;

namespace KioskService.Persistance.Mappers
{
    public class PaymentMapper : IDBMapper<Payment, PaymentEntity>
    {
        public Payment MapToCore(PaymentEntity entity)
        {
            return new Payment()
            {
                id = entity.Id,
                status = entity.Status,
                sum = entity.Sum,
                idemptencyENR = entity.BankId,
                organization = entity.Organization,
                utcTime = entity.TimeStamp,
                localTime = entity.TimeStamp.ToLocalTime(),
                paymentObjects = entity.PaymentObjects,
                paymentWay = entity.PaymentWay,
                check = File.ReadAllText($"{entity.Check}.txt")
            };
        }

        public PaymentEntity MapToDB(Payment entity)
        {
            return new PaymentEntity()
            {
                Status = entity.status,
                Sum = entity.sum,
                TimeStamp = entity.utcTime,
                BankId = entity.idemptencyENR,
                Organization = entity.organization,
                PaymentObjects = entity.paymentObjects,
                PaymentWay = entity.paymentWay
            };
        }
    }
}
