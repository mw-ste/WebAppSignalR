using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;

namespace GuiClient
{
    public class SignalRClient : ISignalRClient
    {
        private readonly HubConnection _hubConnection;

        public SignalRClient(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public Task ReceiveMessage(string sender, string message)
        {
            throw new System.NotImplementedException();
        }

        public Task Acknowledge()
        {
            throw new System.NotImplementedException();
        }

        public Task NotifyUserAdded(string user)
        {
            throw new System.NotImplementedException();
        }

        public Task NotifyUserLeft(string user)
        {
            throw new System.NotImplementedException();
        }
    }
}