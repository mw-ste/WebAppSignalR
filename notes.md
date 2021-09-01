# SignalR

## Hub
* Hubs are transient
* Don't store state in a property on the hub class. Every hub method call is executed on a new hub instance.
* Use await when calling asynchronous methods that depend on the hub staying alive. For example Clients.All.SendAsync(...)

## IHubContext
* The IHubContext is for sending notifications to clients, it is not used to call methods on the Hub