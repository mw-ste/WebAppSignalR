using System;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Shared
{
    public class NeverEndingRetryPolicy : IRetryPolicy
    {
        private readonly TimeSpan _delay;
        private readonly Action<string> _log;

        public NeverEndingRetryPolicy(TimeSpan delay)
        {
            _delay = delay;
            _log = Console.WriteLine;
        }

        public NeverEndingRetryPolicy(TimeSpan delay, ILogger logger)
        {
            _delay = delay;
            _log = message => logger.LogInformation(message);
        }

        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            LogRetry(retryContext);

            return retryContext.PreviousRetryCount switch
            {
                0 => TimeSpan.Zero,
                1 => TimeSpan.FromSeconds(2),
                _ => _delay
            };
        }

        private void LogRetry(RetryContext retryContext)
        {
            var message = $"Retry attempt {retryContext.PreviousRetryCount + 1}, " +
                          $"{_delay} delay before next retry. ";
            _log(message);
        }
    }
}