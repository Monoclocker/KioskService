using KioskService.Persistance.Entities;
using Microsoft.EntityFrameworkCore;

namespace KioskService.Persistance.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<PaymentEntity> Payment { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
