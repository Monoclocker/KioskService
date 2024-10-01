using KioskService.Persistance;
using KioskService.WEB.HubFilters;
using KioskService.WEB.Hubs;
using KioskService.WEB.Interfaces;
using KioskService.WEB.Providers;
using KioskService.WEB.Storages;
using KioskService.WEB.Utils;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddPersistance(builder.Configuration);

            builder.Services.AddSignalR(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromMinutes(int.MaxValue / 2);
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(int.MaxValue);
            })
            .AddHubOptions<KioskHub>(configurator =>
            {
                configurator.AddFilter<KioskHubFilter>();
            })
            .AddHubOptions<DesktopHub>(configurator =>
            {
                configurator.AddFilter<DesktopHubFilter>();
            });

            builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
            builder.Services.AddSingleton<IConnectionStorage, InMemoryConnectionStorage>();

            builder.Logging.RegisterCustomProviders(builder.Configuration);

            var app = builder.Build();
            
            app.MapHub<KioskHub>("/kiosk");
            app.MapHub<DesktopHub>("/desktop");
           
            app.Run();
        }
    }
}
