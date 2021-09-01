namespace Backend
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    public class CommunicationSignalRHub : Hub
    {
        public async Task SendMessageToAllClients(string sender, string message)
        {
            await Clients
                .All
                .SendAsync("ReceiveMessage", sender, message);
        }
    }
}