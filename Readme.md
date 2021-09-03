# SignalR

- This is a simple project to learn more about SignalR.
- Specifically about:
  - IHubContext vs HubConnection
  - Azure SignalR service
  - reconnection behavior

## Project

- **Backend**

  - simple ASP.Net api server
  - swagger is configured at `http://localhost:5000/index.html`
  - configured to run a SignalR hub, using an Azure SignalR service

- **CliClient**
  - a very simple (bear with me) cli tool to send and receive messages sent via SignalR
  - run several instances to "chat" with each other
  - on startup you need to provide a unique "user name"
  - then you can use the user names of other instances to send messages to them
  - leave the user name empty to message all clients

## Learnings

### SignalR Hub

- hubs are transient
- don't store state in a property on the hub class. Every hub method call is executed on a new hub instance.
- use await when calling asynchronous methods that depend on the hub staying alive. For example `Clients.All.SendAsync(...)`

### IHubContext

- the IHubContext is for sending notifications to clients
- it is not used to call methods on the Hub

### Adding App Secrets for the Azure SignalR service

- `cd <project dir>`
- `dotnet user-secrets init`
- `dotnet user-secrets set Azure:SignalR:ConnectionString "Endpoint=https://<signalr name>.service.signalr.net;AccessKey=<access key>=;Version=1.0;"`
- stored in: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- in `Startup.cs` in `ConfigureServices`: `services.AddSignalR().AddAzureSignalR();`

### Groups

- group membership isn't preserved when a connection reconnects
- the connection needs to rejoin the group when it's re-established.


### Reconnecting a disconnected HubConnection

* when the connection was disconnected by the hub calling `Context.Abort();` &rarr; hub connection can be reconnected by the client
* when the connection was disconnected by the client calling `_hubConnection.StopAsync();` &rarr; hub connection can be reconnected by the client
* when the connection was disconnected by the client calling `_hubConnection.DisposeAsync();` &rarr; hub connection can **not** be reconnected by the client