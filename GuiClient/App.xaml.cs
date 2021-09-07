using System;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;

namespace GuiClient
{
    public partial class App : Application
    {
        private readonly IHost _host;
        private static HubConnection _connection;

        public App()
        {
            _connection = CreateHubConnection();
            _host = CreateHostBuilder().Build();
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host
                .CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton(_connection);
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<SignalRClient>();
                });
        }

        private static HubConnection CreateHubConnection()
        {
            return new HubConnectionBuilder()
                .WithUrl(new Uri("http://localhost:5000/communicationsignalrhub"))
                .WithAutomaticReconnect(new NeverEndingRetryPolicy(TimeSpan.FromSeconds(10)))
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs startupEventArgs)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(startupEventArgs);
        }

        protected override async void OnExit(ExitEventArgs exitEventArgs)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(exitEventArgs);
        }
    }
}