using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace CliClient
{
    public static class HubConnectionExtensions
    {
        public static async Task StartSafelyAsync(this HubConnection connection, TimeSpan retryDelay)
        {
            var connectionAttempt = 0;

            while (true)
            {
                try
                {
                    connectionAttempt++;
                    await connection.StartAsync();
                    Console.WriteLine($"Hub connection {connection.ConnectionId} established");
                    return;
                }
                catch(Exception exception)
                {
                    Console.WriteLine($"Failed to start connection, attempt {connectionAttempt}, retry in {retryDelay}...");
                    Console.WriteLine(exception.Message);
                    await Task.Delay(retryDelay);
                }
            }
        }
    }
}