using KioskService.Core.Interfaces;
using KioskService.Persistance.Database;
using KioskService.Persistance.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KioskService.Persistance
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(configuration["database"], 
                opt => opt.MigrationsAssembly("KioskService.WEB")));

            services.AddScoped<Utils.Mappers>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IResultsService, ResultsService>();

            return services;
        } 
    }
}
