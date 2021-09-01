﻿using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace CliClient
{
    public class NeverEndingRetryPolicy : IRetryPolicy
    {
        private readonly TimeSpan _delay;

        public NeverEndingRetryPolicy(TimeSpan delay)
        {
            _delay = delay;
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
            Console.WriteLine(message);
        }
    }
}