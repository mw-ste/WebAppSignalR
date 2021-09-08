using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Shared
{
    public static class HubConnectionExtensions
    {
        public static async Task StartSafelyAsync(this HubConnection connection, TimeSpan retryDelay, ILogger logger = null)
        {
            var connectionAttempt = 0;

            while (true)
            {
                try
                {
                    connectionAttempt++;
                    await connection.StartAsync();
                    logger?.LogInformation($"Hub connection {connection.ConnectionId} established");
                    return;
                }
                catch(Exception exception)
                {
                    logger?.LogInformation(
                        $"Failed to start connection, attempt {connectionAttempt}, retry in {retryDelay}..." +
                        $"\n{exception.Message}");
                    await Task.Delay(retryDelay);
                }
            }
        }
    }
}