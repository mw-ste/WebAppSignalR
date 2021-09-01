namespace Backend
{
    using System;
    using Shared;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    public class CommunicationSignalRHub : Hub<ISignalRClient>
    {
        public async Task SendMessageToAllClients(string sender, string message)
        {
            await Clients
                .Caller
                .Acknowledge();

            await Clients
                .Others
                .ReceiveMessage(sender, message);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.All.NotifyUserAdded(Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.NotifyUserLeft(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}