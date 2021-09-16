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
  - enter "exit" as target name or message to receive a "connection closed" from the hub

- **GuiClient**

  - ~~There is a problem in the shutdown of the gui that I have not fixed yet!~~
  - simple WPF gui client
  - run several instances to "chat" with each other
  - register with a user name to start the SignalR connection
  - then you can use the user names of other instances to send messages to them
  - leave the user name empty to message all clients
  - press "DisconnectMe" to force a "connection closed" from the hub

## Learnings

### SignalR Hub

- hubs are transient!
- don't store state in a property on the hub class. Every hub method call is executed on a new hub instance.
- use await when calling asynchronous methods that depend on the hub staying alive. For example `Clients.All.SendAsync(...)`

### IHubContext

- the IHubContext is for sending notifications to clients
- it is not used to call methods on the hub

### Evil surveillance logger

- the backend opens a client connection to the hub provided by itself
- so the rout is something like: client -> SignalR service -> hub -> SignalR service -> hub -> evil surveillance logger
- this is a bit chatty
- so whenever possible, call stuff directly from the hub (have the dependencies injected)

### Adding App Secrets for the Azure SignalR service

- `cd <project dir>`
- `dotnet user-secrets init`
- `dotnet user-secrets set Azure:SignalR:ConnectionString "Endpoint=https://<signalr name>.service.signalr.net;AccessKey=<access key>=;Version=1.0;"`
- stored in: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- in `Startup.cs` in `ConfigureServices`: `services.AddSignalR().AddAzureSignalR();`

### Running without Azure

- if you get the following error, then your have not configured a connection string
- `Microsoft.Azure.SignalR.Common.AzureSignalRConfigurationNoEndpointException: No connection string was specified.`
- to run the SignalR server locally instead of the Azure SignalR service, remove `.AddAzureSignalR(...)` from the line `services.AddSignalR().AddAzureSignalR();` in the file `Startup.cs`

### Groups

- group membership isn't preserved when a connection reconnects
- the connection needs to rejoin the group when it's re-established.

### Reconnecting a disconnected HubConnection

- when the connection was disconnected by the hub calling `Context.Abort();` &rarr; hub connection can be reconnected by the client
- when the connection was disconnected by the client calling `_hubConnection.StopAsync();` &rarr; hub connection can be reconnected by the client
- when the connection was disconnected by the client calling `_hubConnection.DisposeAsync();` &rarr; hub connection can **not** be reconnected by the client

### IRetryPolicy

- a retry policy can be registered to handle lost connections
- if no retry policy is registered, or the retry policy "timed out" (if implemented in such a way) the `Closed` event of the hub connection will be fired
- you can subscribe to this event to "manually" try to open the connection again (`_hubConnection.StartAsync();`)
- if this is not successful, you will get an exception (`HttpRequestException`)
- the `Closed` event will not fire again
- as a workaround for temporary unavailable connections, you can try to open the connection again in a loop, like in `StartSafelyAsync` in `HubConnectionExtensions`
- this will also work for initially starting the connection

## SignalR, WPF, MVVM, DI

- https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
- https://intellitect.com/getting-started-model-view-viewmodel-mvvm-pattern-using-windows-presentation-framework-wpf/
- https://marcominerva.wordpress.com/2019/11/07/update-on-using-hostbuilder-dependency-injection-and-service-provider-with-net-core-3-0-wpf-applications/
