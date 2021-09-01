using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;

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

            connection.Reconnected += _ =>
            {
                Console.WriteLine($"Reconnected, new connection state \"{connection.State}\", new id: {connection.ConnectionId}");
                return Task.CompletedTask;
            };

            await connection.StartAsync();
            Console.WriteLine($"my id: {connection.ConnectionId}");

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                Console.WriteLine("Enter message: ");
                var message = Console.ReadLine();
                await client.SendMessage(message);
            }
        }
    }
}
