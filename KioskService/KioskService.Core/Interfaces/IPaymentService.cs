using KioskService.Core.DTO;
using KioskService.Core.Models;

namespace KioskService.Core.Interfaces
{
    public interface IPaymentService
    {
        public Task<Payment?> GetPayment(int TransactionId);
        public Task<int> SavePayment(Payment payment);
        public Task ProceedRefund(int TransactionId);
        public Task<PaginatedList<PaymentPreview>> GetPreviousTransactions(int page);
    }
}
