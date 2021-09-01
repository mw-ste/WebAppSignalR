using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;

namespace CliClient
{
    public class SignalRClient : ISignalRClient
    {
        private readonly HubConnection _hubConnection;

        public SignalRClient(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
            SubscribeToHub();
        }

        private void SubscribeToHub()
        {
            _hubConnection.On<string, string>(nameof(ReceiveMessage), ReceiveMessage);
            _hubConnection.On(nameof(Acknowledge), Acknowledge);
            _hubConnection.On<string>(nameof(NotifyUserAdded), NotifyUserAdded);
            _hubConnection.On<string>(nameof(NotifyUserLeft), NotifyUserLeft);
        }

        public Task ReceiveMessage(string sender, string message)
        {
            Console.WriteLine($"Received message from {sender}: {message}");
            return Task.CompletedTask;
        }

        public Task Acknowledge()
        {
            Console.WriteLine("Message successfully sent");
            return Task.CompletedTask;
        }

        public Task NotifyUserAdded(string user)
        {
            Console.WriteLine($"New user {user} joined the conversation");
            return Task.CompletedTask;
        }

        public Task NotifyUserLeft(string user)
        {
            Console.WriteLine($"User {user} left the conversation");
            return Task.CompletedTask;
        }

        public async Task SendMessage(string message)
        {
            await _hubConnection.SendCoreAsync("SendMessageToAllClients", new object[]{_hubConnection.ConnectionId, message});
        }
    }
}