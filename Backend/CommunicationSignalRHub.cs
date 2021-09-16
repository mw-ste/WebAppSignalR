namespace Backend
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using Shared;

    // Hubs are transient !!!
    public class CommunicationSignalRHub : Hub<ISignalRClient>
    {
        private readonly ILogger<CommunicationSignalRHub> _logger;

        public CommunicationSignalRHub(ILogger<CommunicationSignalRHub> logger)
        {
            _logger = logger;
        }

        public async Task SendMessageToAllClients(string sender, string message)
        {
            await Clients
                .Others
                .ReceiveMessage(sender, message);

            await Clients
                .Caller
                .Acknowledge();
        }

        public async Task SendMessageToClient(string sender, string target, string message)
        {
            _logger.LogInformation($"SendMessageToClient from {sender} to {target}");

            await Clients
                .Group(target)
                .ReceiveMessage(sender, message);

            await Clients
                .Caller
                .Acknowledge();
        }

        public async Task RegisterWithName(string sender)
        {
            _logger.LogInformation($"RegisterWithName {sender} with id {Context.ConnectionId}");

            await Groups.AddToGroupAsync(Context.ConnectionId, sender);

            await Clients
                .Others
                .NotifyUserRegistered(sender);
        }

        public void DisconnectMe()
        {
            _logger.LogInformation($"Closing connection with id {Context.ConnectionId}");

            Context.Abort();
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            await Clients
                .Others
                .NotifyUserConnected(Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"OnDisconnectedAsync {Context.ConnectionId}");

            await Clients
                .Others
                .NotifyUserDisconnected(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}