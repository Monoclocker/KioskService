using KioskService.Core.Interfaces;
using KioskService.Persistance.Services;
using KioskService.Persistance.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KioskService.Persistance
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DbContext>(options => options.UseNpgsql(configuration["database"], 
                opt => opt.MigrationsAssembly("KioskService.WEB")));

            services.AddScoped<Utils.Mappers>();

            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        } 
    }
}
