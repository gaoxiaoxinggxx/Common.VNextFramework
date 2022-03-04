using Microsoft.Extensions.Logging;
using System;

namespace Common.VNextFramework.EntityFramework
{
    public class ConsoleLogger : ILogger
    {
        private readonly string _CategoryName = null;
        public ConsoleLogger(string categoryName)
        {
            _CategoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            System.Diagnostics.Debug.WriteLine($"************************************************************");
            System.Diagnostics.Debug.WriteLine($"ConsoleLogger {_CategoryName} {logLevel} {eventId} {state} start");
            System.Diagnostics.Debug.WriteLine($"Exception：{exception?.Message}");
            System.Diagnostics.Debug.WriteLine($"Message：{formatter.Invoke(state, exception)}");
            System.Diagnostics.Debug.WriteLine($"ConsoleLogger {_CategoryName} {logLevel} {eventId} {state} end");
            System.Diagnostics.Debug.WriteLine($"************************************************************");


            System.Console.WriteLine($"************************************************************");
            System.Console.WriteLine($"ConsoleLogger {_CategoryName} {logLevel} {eventId} {state} start");
            System.Console.WriteLine($"Exception：{exception?.Message}");
            System.Console.WriteLine($"Message：{formatter.Invoke(state, exception)}");
            System.Console.WriteLine($"ConsoleLogger {_CategoryName} {logLevel} {eventId} {state} end");
            System.Console.WriteLine($"************************************************************");
        }
    }
}
