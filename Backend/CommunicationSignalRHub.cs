namespace Backend
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    public class CommunicationSignalRHub : Hub
    {
        public async Task SendMessageToAllClients(string sender, string message)
        {
            await Clients
                .Caller
                .SendAsync("Acknowledge");

            await Clients
                .Others
                .SendAsync("ReceiveMessage", sender, message);

        }
    }
}