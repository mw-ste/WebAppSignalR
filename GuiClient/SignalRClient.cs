using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;

namespace GuiClient
{
    public class SignalRClient : ISignalRClient
    {
        private readonly HubConnection _hubConnection;
        private readonly Action<string> _logMessage;
        private readonly Action<string> _logInfo;
        private string _name;

        public SignalRClient(
            HubConnection hubConnection,
            Action<string> logMessage,
            Action<string> logInfo)
        {
            _hubConnection = hubConnection;
            _logMessage = logMessage;
            _logInfo = logInfo;

            SubscribeToHub();
        }

        public async Task StartConnection()
        {
            await _hubConnection.StartAsync();
        }

        private void SubscribeToHub()
        {
            _hubConnection.Closed += OnClosed;
            _hubConnection.Reconnected += Reconnected;
            _hubConnection.Reconnecting += Reconnecting;

            _hubConnection.On<string, string>(nameof(ReceiveMessage), ReceiveMessage);
            _hubConnection.On(nameof(Acknowledge), Acknowledge);
            _hubConnection.On<string>(nameof(NotifyUserAdded), NotifyUserAdded);
            _hubConnection.On<string>(nameof(NotifyUserLeft), NotifyUserLeft);
        }

        private Task Reconnecting(Exception exception)
        {
            _logInfo($"Reconnecting. Connection was interrupted because of \"{exception}\"");
            return Task.CompletedTask;
        }

        private async Task Reconnected(string newConnectionId)
        {
            _logInfo(
                $"Hub reconnected with new id {newConnectionId}, " +
                $"new connection state \"{_hubConnection.State}\"");

            await RegisterWithName();
        }

        private Task OnClosed(Exception exception)
        {
            _logInfo(
                $"Hub connection {_hubConnection.ConnectionId} was closed!\n" +
                $"Reason: {exception}");

            return Task.CompletedTask;
        }

        public Task Register(string name)
        {
            _name = name;
            return RegisterWithName();
        }

        public Task ReceiveMessage(string sender, string message)
        {
            _logMessage($"{sender}: {message}");

            return Task.CompletedTask;
        }

        public Task Acknowledge()
        {
            _logInfo("Message successfully sent");

            return Task.CompletedTask;
        }

        public Task NotifyUserAdded(string user)
        {
            _logInfo($"{user} joined the conversation");

            return Task.CompletedTask;
        }

        public Task NotifyUserLeft(string user)
        {
            _logInfo($"{user} left the conversation");

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
    }
}