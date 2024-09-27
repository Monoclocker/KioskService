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

        public async Task<Payment?> GetPayment(Guid TransactionId)
        {
            PaymentEntity? entity = await context.Payment
                .FirstOrDefaultAsync(x => x.Id == TransactionId);

            if (entity != null)
            {
                return mappers.paymentMapper.MapToCore(entity);
            }

            return null;
        }

        public async Task ProceedRefund(Guid TransactionId)
        {
            PaymentEntity? entity = await context.Payment
                .FirstOrDefaultAsync(x => x.Id == TransactionId);

            if (entity is null || !entity.IsValid)
            {
                throw new InvalidPaymentTranscation();
            }

            entity.IsValid = false;

            await Task.CompletedTask;
        }

        public async Task<Guid> SavePayment(Payment payment)
        {
            Guid newId = Guid.NewGuid();

            PaymentEntity entity = mappers
                .paymentMapper.MapToDB(payment);

            entity.Id = newId;

            string filePath = $"{newId}.txt";

            entity.Check = filePath;

            File.WriteAllText(filePath, payment.check);

            await context.Payment.AddAsync(entity);

            await context.SaveChangesAsync();

            return await Task.FromResult(newId);
        }
    }
}
