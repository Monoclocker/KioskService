using KioskService.Core.Models;

namespace KioskService.Core.Interfaces
{
    public interface IPaymentService
    {
        public Task<Payment?> GetPayment(Guid TransactionId);
        public Task<Guid> SavePayment(Payment payment);
        public Task ProceedRefund(Guid TransactionId);
    }
}
