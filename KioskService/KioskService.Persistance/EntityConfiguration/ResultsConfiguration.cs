using KioskService.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KioskService.Persistance.EntityConfiguration
{
    public class ResultsConfiguration : IEntityTypeConfiguration<ResultsEntity>
    {
        public void Configure(EntityTypeBuilder<ResultsEntity> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
