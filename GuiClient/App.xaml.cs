using System;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;

namespace GuiClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        static IHostBuilder CreateHostBuilder()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(new Uri("http://localhost:5000/communicationsignalrhub"))
                .WithAutomaticReconnect(new NeverEndingRetryPolicy(TimeSpan.FromSeconds(10)))
                .Build();

            return Host
                .CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton(connection);
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<SignalRClient>();
                });
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
            await _host.Services.GetRequiredService<HubConnection>().DisposeAsync();
            await _host.StopAsync();

            base.OnExit(exitEventArgs);
        }
    }
}