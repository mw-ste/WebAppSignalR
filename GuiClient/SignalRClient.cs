using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Shared;

namespace GuiClient
{
    public class SignalRClient : ISignalRClient
    {
        private readonly HubConnection _hubConnection;
        private readonly ILogger<SignalRClient> _logger;
        private string _name;

        public event Action<string, string> MessageReceived;
        public event Action MessageSent;
        public event Action<string> UserJoined;
        public event Action<string> UserConnected;
        public event Action<string> UserDisconnected;

        public SignalRClient(HubConnection hubConnection, ILogger<SignalRClient> logger)
        {
            _hubConnection = hubConnection;
            _logger = logger;
            SubscribeToHub();
        }

        private void SubscribeToHub()
        {
            _hubConnection.Closed += OnClosed;
            _hubConnection.Reconnected += Reconnected;
            _hubConnection.Reconnecting += Reconnecting;

            _hubConnection.On<string, string>(nameof(ReceiveMessage), ReceiveMessage);
            _hubConnection.On(nameof(Acknowledge), Acknowledge);
            _hubConnection.On<string>(nameof(NotifyUserConnected), NotifyUserConnected);
            _hubConnection.On<string>(nameof(NotifyUserDisconnected), NotifyUserDisconnected);
        }

        public async Task Stop()
        {
            _hubConnection.Closed -= OnClosed;
            _hubConnection.Reconnected -= Reconnected;
            _hubConnection.Reconnecting -= Reconnecting;

            _hubConnection.Remove(nameof(ReceiveMessage));
            _hubConnection.Remove(nameof(Acknowledge));
            _hubConnection.Remove(nameof(NotifyUserConnected));
            _hubConnection.Remove(nameof(NotifyUserDisconnected));

            await _hubConnection.StopAsync();
        }

        private async Task StartConnection()
        {
            await _hubConnection.StartSafelyAsync(TimeSpan.FromSeconds(10), _logger);
        }

        private Task Reconnecting(Exception exception)
        {
            return Task.CompletedTask;
        }

        private async Task Reconnected(string newConnectionId)
        {
            await RegisterWithName();
        }

        private async Task OnClosed(Exception exception)
        {
            await StartConnection();
            await RegisterWithName();
        }

        public async Task Register(string name)
        {
            _name = name;
            await StartConnection();
            await RegisterWithName();
        }

        public Task ReceiveMessage(string sender, string message)
        {
            MessageReceived?.Invoke(sender, message);
            return Task.CompletedTask;
        }

        public Task Acknowledge()
        {
            MessageSent?.Invoke();
            return Task.CompletedTask;
        }

        public Task NotifyUserRegistered(string userName)
        {
            UserJoined?.Invoke(userName);
            return Task.CompletedTask;
        }

        public Task NotifyUserConnected(string userId)
        {
            UserConnected?.Invoke(userId);
            return Task.CompletedTask;
        }

        public Task NotifyUserDisconnected(string userId)
        {
            UserDisconnected?.Invoke(userId);
            return Task.CompletedTask;
        }

        public async Task RegisterWithName()
        {
            EnsureConnected();
            await _hubConnection.SendCoreAsync("RegisterWithName", new object[] { _name });
        }

        public async Task SendMessageToAllClients(string message)
        {
            EnsureConnected();
            await _hubConnection.SendCoreAsync("SendMessageToAllClients", new object[] { _name, message });
        }

        public async Task SendMessageToClient(string target, string message)
        {
            EnsureConnected();
            await _hubConnection.SendCoreAsync("SendMessageToClient", new object[] { _name, target, message });
        }

        public async Task DisconnectMe()
        {
            EnsureConnected();
            await _hubConnection.SendCoreAsync("DisconnectMe", Array.Empty<object>());
        }

        private void EnsureConnected()
        {
            if (string.IsNullOrEmpty(_name) || _hubConnection.State != HubConnectionState.Connected)
            {
                throw new Exception(
                    $"Trying to invoke hub method while name is \"{_name}\" " +
                    $"and connection state is \"{_hubConnection.State}\"");
            }
        }
    }
}