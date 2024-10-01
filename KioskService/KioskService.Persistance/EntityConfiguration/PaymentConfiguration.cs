using KioskService.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KioskService.Persistance.EntityConfiguration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<PaymentEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.IsValid)
                .HasDefaultValue(true);
            builder.Property(x => x.TimeStamp)
                .HasDefaultValue(DateTime.UtcNow);
        }
    }
}
