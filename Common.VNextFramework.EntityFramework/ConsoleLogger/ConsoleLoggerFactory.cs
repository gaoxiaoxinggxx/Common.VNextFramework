using Microsoft.Extensions.Logging;

namespace Common.VNextFramework.EntityFramework
{
    public class ConsoleLoggerFactory : Microsoft.Extensions.Logging.ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }
}
