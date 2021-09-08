using System;
using Microsoft.Extensions.Logging;

namespace GuiClient
{
    public class GenericLogger : ILogger
    {
        private readonly Func<Action<string>> _getLogAction;

        public GenericLogger(Func<Action<string>> getLogAction)
        {
            _getLogAction = getLogAction;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var log = _getLogAction();
            log?.Invoke(formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}