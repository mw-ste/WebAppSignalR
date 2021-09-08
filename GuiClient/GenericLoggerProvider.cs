using System;
using Microsoft.Extensions.Logging;

namespace GuiClient
{
    public class GenericLoggerProvider : ILoggerProvider
    {
        private Action<string> _log;

        public void SetLogMethod(Action<string> log)
        {
            _log = log;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new GenericLogger(() => _log);
        }
    }
}