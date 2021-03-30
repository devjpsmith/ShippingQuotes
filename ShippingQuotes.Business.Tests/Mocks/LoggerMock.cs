using System;
using Microsoft.Extensions.Logging;

namespace ShippingQuotes.Business.Tests.Mocks
{
    public class LoggerMock<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return state as IDisposable;
        }

        public bool IsEnabled(LogLevel logLevel) => false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }
    }
}