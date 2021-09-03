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
            await client.ConnectToHub();
            await client.RegisterWithName();

            Console.WriteLine($"my id: {connection.ConnectionId}");
            Console.WriteLine("enter \"exit\" to disconnect");

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                Console.WriteLine("Enter receiver: ");
                var receiver = Console.ReadLine();
                if (receiver?.ToLower() == "exit")
                {
                    await Disconnect(client);
                    continue;
                }

                Console.WriteLine("Enter message: ");
                var message = Console.ReadLine();
                if (message?.ToLower() == "exit")
                {
                    await Disconnect(client);
                    continue;
                }

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

        private static async Task Disconnect(SignalRClient client)
        {
            await client.Disconnect();
            await client.SendDisconnect();
            //await client.DisposeConnection();
        }
    }
}
