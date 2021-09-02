# SignalR

## Hub
* Hubs are transient
* Don't store state in a property on the hub class. Every hub method call is executed on a new hub instance.
* Use await when calling asynchronous methods that depend on the hub staying alive. For example Clients.All.SendAsync(...)

## IHubContext
* The IHubContext is for sending notifications to clients, it is not used to call methods on the Hub

## App Secrets
* `cd <project dir>`
* `dotnet user-secrets init`
* `dotnet user-secrets set Azure:SignalR:ConnectionString "Endpoint=https://<signalr name>.service.signalr.net;AccessKey=<access key>=;Version=1.0;"`
* stored in: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
* in `Startup.cs` in `ConfigureServices`: `services.AddSignalR().AddAzureSignalR();`

## Groups
* Group membership isn't preserved when a connection reconnects. The connection needs to rejoin the group when it's re-established.