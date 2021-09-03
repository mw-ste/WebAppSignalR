using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;

namespace CliClient
{
    public class SignalRClient : ISignalRClient
    {
        private readonly HubConnection _hubConnection;
        private readonly string _name;

        public SignalRClient(HubConnection hubConnection, string name)
        {
            _hubConnection = hubConnection;
            _name = name;
            SubscribeToHub();

            _hubConnection.Closed += OnClosed;
            _hubConnection.Reconnected += Reconnected;
            _hubConnection.Reconnecting += Reconnecting;
        }

        private static Task Reconnecting(Exception exception)
        {
            Console.WriteLine($"Reconnecting. Connection was interrupted because of \"{exception}\"");

            return Task.CompletedTask;
        }

        private async Task Reconnected(string newConnectionId)
        {
            Console.WriteLine(
                $"Hub reconnected with new id \"{newConnectionId}\", " +
                $"new connection state \"{_hubConnection.State}\"");

            await RegisterWithName();
        }

        private Task OnClosed(Exception exception)
        {
            Console.WriteLine(
                $"Hub connection \"{_hubConnection.ConnectionId}\" was closed!\n" +
                $"Reason: \"{exception}\"");

            return Task.CompletedTask;
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

        public async Task SendMessageToAllClients(string message)
        {
            await _hubConnection.SendCoreAsync("SendMessageToAllClients", new object[] { _name, message });
        }

        public async Task SendMessageToClient(string target, string message)
        {
            await _hubConnection.SendCoreAsync("SendMessageToClient", new object[] { _name, target, message });
        }

        public async Task RegisterWithName()
        {
            await _hubConnection.SendCoreAsync("RegisterWithName", new object[] { _name });
        }

        public async Task SendDisconnect()
        {
            await _hubConnection.SendCoreAsync("DisconnectMe", new object[0]);
        }
    }
}