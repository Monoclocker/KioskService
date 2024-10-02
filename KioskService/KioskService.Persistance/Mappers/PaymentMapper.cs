using KioskService.Core.Models;
using KioskService.Persistance.Entities;
using KioskService.Persistance.Interfaces;
using System.Text;

namespace KioskService.Persistance.Mappers
{
    public class PaymentMapper : IDBMapper<Payment, PaymentEntity>
    {
        public Payment MapToCore(PaymentEntity entity)
        {
            string? checkB64 = null;

            if (entity.Check != null)
            {
                checkB64 = Encoding.UTF8.GetString
                    (Convert.FromBase64String(entity.Check));
            }

            return new Payment()
            {
                id = entity.Id,
                status = entity.Status,
                deviceId = entity.DeviceId,
                sum = entity.Sum,
                idemptencyENR = entity.BankId,
                organization = entity.Organization,
                utcTime = entity.TimeStamp,
                localTime = entity.TimeStamp.ToLocalTime(),
                paymentObjects = entity.PaymentObjects,
                paymentWay = entity.PaymentWay,
                
                check = checkB64
            };
        }

        public PaymentEntity MapToDB(Payment entity)
        {
            string? checkOrigin = null;

            if (entity.check != null)
            {
                checkOrigin = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(entity.check));
            }

            return new PaymentEntity()
            {
                Status = entity.status,
                Sum = entity.sum,
                DeviceId = entity.deviceId,
                TimeStamp = entity.utcTime,
                BankId = entity.idemptencyENR,
                Organization = entity.organization,
                PaymentObjects = entity.paymentObjects,
                PaymentWay = entity.paymentWay,
                Check = checkOrigin
            };
        }
    }
}
