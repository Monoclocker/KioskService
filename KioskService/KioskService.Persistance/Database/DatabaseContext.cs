using KioskService.Persistance.Entities;
using KioskService.Persistance.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace KioskService.Persistance.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<PaymentEntity> Payment { get; set; }
        public DbSet<ResultsEntity> Results { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new ResultsConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
