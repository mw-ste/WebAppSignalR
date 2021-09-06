using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;

namespace GuiClient
{
    public class Program
    {
        // https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
        // https://intellitect.com/getting-started-model-view-viewmodel-mvvm-pattern-using-windows-presentation-framework-wpf/

        static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            return host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(new Uri("http://localhost:5000/communicationsignalrhub"))
                .WithAutomaticReconnect(new NeverEndingRetryPolicy(TimeSpan.FromSeconds(10)))
                .Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services
                        .AddSingleton(connection)
                        .AddSingleton<ISignalRClient, SignalRClient>()
                        .AddSingleton<MainWindow>()
                        .AddSingleton<MainWindowViewModel>());
        }
    }
}