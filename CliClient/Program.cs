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
            Console.WriteLine("Enter your name: ");
            var name = Console.ReadLine();

            var connection = new HubConnectionBuilder()
                .WithUrl(new Uri("http://localhost:5000/communicationsignalrhub"))
                .WithAutomaticReconnect(new NeverEndingRetryPolicy(TimeSpan.FromSeconds(10)))
                .Build();

            var client = new SignalRClient(connection, name);
            await connection.StartAsync();
            await client.RegisterWithName();

            Console.WriteLine($"my id: {connection.ConnectionId}");

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                Console.WriteLine("Enter receiver: ");
                var receiver = Console.ReadLine();

                Console.WriteLine("Enter message: ");
                var message = Console.ReadLine();

                if (string.IsNullOrEmpty(receiver))
                {
                    await client.SendMessageToAllClients(message);
                }
                else
                {
                    await client.SendMessageToClient(receiver, message);
                }

            }
        }
    }
}
