using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace KioskService.Persistance
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMongoClient>(new MongoClient(configuration["database-host"]));
            services.AddDbContext<DbContext>(options => options.UseNpgsql(configuration["database"], 
                opt => opt.MigrationsAssembly("KioskService.WEB")));

            return services;
        } 
    }
}
