
namespace KioskService.WEB.Loggers
{
    public class FileLogger : ILogger
    {
        string path;
        object locker = new object();

        public FileLogger(string path)
        {
            this.path = path;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            lock (locker)
            {
                File.AppendAllText(path, $"{DateTime.Now.ToString()}: {formatter(state, exception)}" + Environment.NewLine);
            }
            
        }
    }
}
