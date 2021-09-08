using System;
using System.Windows;
using System.Windows.Threading;
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

        public App()
        {
            _host = CreateHostBuilder();
        }

        private static IHost CreateHostBuilder()
        {
            var windowLoggerProvider = new GenericLoggerProvider();

            return Host
                .CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddProvider(windowLoggerProvider);
                })
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton(CreateHubConnection);
                    services.AddSingleton(windowLoggerProvider);
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<SignalRClient>();
                })
                .Build();
        }

        private static HubConnection CreateHubConnection(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<NeverEndingRetryPolicy>>();

            return new HubConnectionBuilder()
                .WithUrl(new Uri("http://localhost:5000/communicationsignalrhub"))
                .WithAutomaticReconnect(new NeverEndingRetryPolicy(TimeSpan.FromSeconds(10), logger))
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs startupEventArgs)
        {
            await _host.StartAsync();

            ConfigureLogToMainWindow();

            _host.Services.GetRequiredService<MainWindow>().Show();

            base.OnStartup(startupEventArgs);
        }

        private void ConfigureLogToMainWindow()
        {
            var windowLoggerProvider = _host.Services.GetRequiredService<GenericLoggerProvider>();
            var windowViewModel = _host.Services.GetRequiredService<MainWindowViewModel>();
            windowLoggerProvider.SetLogMethod(windowViewModel.WriteToLog);
        }

        protected override async void OnExit(ExitEventArgs exitEventArgs)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(exitEventArgs);
        }

        private void HandleUnhandledExceptions(object sender, DispatcherUnhandledExceptionEventArgs eventArgs)
        {
            MessageBox.Show(
                eventArgs.Exception.ToString(),
                "Unhandled Exception",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}