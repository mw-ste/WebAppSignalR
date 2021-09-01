namespace Backend
{
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
    }
}