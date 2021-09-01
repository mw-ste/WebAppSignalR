namespace Backend
{
    using System;
    using Shared;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    // Hubs are transient !!!
    public class CommunicationSignalRHub : Hub<ISignalRClient>
    {
        public async Task SendMessageToAllClients(string sender, string message)
        {
            await Clients
                .Others
                .ReceiveMessage(sender, message);

            await Clients
                .Caller
                .Acknowledge();
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            await Clients
                .Others
                .NotifyUserAdded(Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients
                .Others
                .NotifyUserLeft(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}