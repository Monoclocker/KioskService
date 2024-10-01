using KioskService.Core.DTO;
using KioskService.Core.Exceptions;
using KioskService.Core.Interfaces;
using KioskService.Core.Models;
using KioskService.Persistance.Database;
using KioskService.Persistance.Entities;
using Microsoft.EntityFrameworkCore;

namespace KioskService.Persistance.Services
{
    public class PaymentService : IPaymentService
    {
        DatabaseContext context;
        Utils.Mappers mappers;
        public PaymentService(DatabaseContext context, Utils.Mappers mappers)
        {
            this.context = context;
            this.mappers = mappers;
        }

        public async Task<Payment?> GetPayment(int TransactionId)
        {
            PaymentEntity? entity = await context.Payment
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == TransactionId);

            if (entity != null)
            {
                return mappers.paymentMapper.MapToCore(entity);
            }

            return null;
        }

        public async Task<PaginatedList<PaymentPreview>> GetPreviousTransactions(string deviceId, int page)
        {
            PaginatedList<PaymentPreview> paymentPage = new PaginatedList<PaymentPreview>();

            paymentPage.pagesCount = (await context.Payment.CountAsync()) / 10 + 1;

            paymentPage.results = await context.Payment
                .AsNoTracking()
                .Where(x => x.IsValid && x.DeviceId == deviceId)
                .OrderByDescending(x => x.TimeStamp)
                .Skip(10 * (page - 1))
                .Take(10)
                .Select(x => new PaymentPreview()
                {
                    id = x.Id,
                    sum = x.Sum,
                    localDate = x.TimeStamp.ToLocalTime()
                })
                .ToListAsync();

            return paymentPage;
        }

        public async Task ProceedRefund(int TransactionId)
        {
            PaymentEntity? entity = await context.Payment
                .FirstOrDefaultAsync(x => x.Id == TransactionId);

            if (entity is null || !entity.IsValid)
            {
                throw new InvalidPaymentTranscation();
            }

            entity.IsValid = false;

            context.Payment.Update(entity);

            await context.SaveChangesAsync();
        }

        public async Task<int> SavePayment(Payment payment)
        {
            PaymentEntity entity = mappers
                .paymentMapper.MapToDB(payment);

            await context.Payment.AddAsync(entity);

            await context.SaveChangesAsync();

            return await Task.FromResult(entity.Id);
        }
    }
}
