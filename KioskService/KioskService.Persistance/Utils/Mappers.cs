using KioskService.Core.Models;
using KioskService.Persistance.Entities;
using KioskService.Persistance.Interfaces;
using KioskService.Persistance.Mappers;

namespace KioskService.Persistance.Utils
{
    public class Mappers
    {
        public IDBMapper<Payment, PaymentEntity> paymentMapper =>
            new PaymentMapper();
    }
}
