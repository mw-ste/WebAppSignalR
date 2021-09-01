using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace CliClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var connection = new HubConnectionBuilder()
                .WithUrl(new Uri("http://localhost:5000/communicationsignalrhub"))
                .WithAutomaticReconnect(new NeverEndingRetryPolicy(TimeSpan.FromSeconds(10)))
                .Build();

            var client = new SignalRClient(connection);
            await connection.StartAsync();

            connection.Reconnected += _ =>
            {
                Console.WriteLine($"Reconnected, new connection state {connection.State}");
                return Task.CompletedTask;
            };

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                Console.WriteLine("Enter message: ");
                var message = Console.ReadLine();
                await client.SendMessage(message);
            }
        }
    }
}
