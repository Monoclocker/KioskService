using KioskService.WEB.HubFilters;
using KioskService.WEB.Hubs;
using KioskService.WEB.Providers;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR(options =>
            {
                options.KeepAliveInterval = TimeSpan.MaxValue;
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

            var app = builder.Build();
            
            app.MapHub<KioskHub>("/kiosk");
            app.MapHub<DesktopHub>("/desktop");
           
            app.Run();
        }
    }
}
