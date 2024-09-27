using KioskService.WEB.Loggers;

namespace KioskService.WEB.Providers
{
    public class FileLoggerProvider : ILoggerProvider
    {
        string? filePath;
        public FileLoggerProvider(IConfiguration configuration)
        {
            filePath = configuration["logFile"];
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(filePath ?? "logs.txt");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
