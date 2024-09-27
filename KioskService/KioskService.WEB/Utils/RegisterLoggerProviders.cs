using KioskService.WEB.Providers;

namespace KioskService.WEB.Utils
{
    public static class RegisterLoggerProviders
    {
        public static ILoggingBuilder RegisterCustomProviders(this ILoggingBuilder builder, IConfiguration configuration)
        {
            builder.AddProvider(new FileLoggerProvider(configuration));
            return builder;
        }
    }
}
