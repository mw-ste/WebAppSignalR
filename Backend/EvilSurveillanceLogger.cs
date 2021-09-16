using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;

namespace Backend
{
    public class EvilSurveillanceBackgroundService : BackgroundService
    {
        private readonly EvilSurveillanceLogger _evilSurveillanceLogger;

        public EvilSurveillanceBackgroundService(EvilSurveillanceLogger evilSurveillanceLogger)
        {
            _evilSurveillanceLogger = evilSurveillanceLogger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _evilSurveillanceLogger.Start();
        }
    }

    public class EvilSurveillanceLogger
    {
        private readonly ILogger _logger;
        private readonly HubConnection _connection;

        public EvilSurveillanceLogger(ILogger<EvilSurveillanceLogger> logger)
        {
            _logger = logger;

            _connection = new HubConnectionBuilder()
                .WithUrl(new Uri("http://localhost:5000/communicationsignalrhub"))
                .WithAutomaticReconnect(new NeverEndingRetryPolicy(TimeSpan.FromSeconds(10)))
                .Build();

            _connection.On<string, string>("ReceiveMessage", LogMessage);
            _connection.On<string>("NotifyUserRegistered", RegisterAsUser);

            _connection.Closed += OnClosed;
            _connection.Reconnected += Reconnected;
        }

        private async Task RegisterAsUser(string userName)
        {
            _logger.LogInformation(
                $"Registering as user \"{userName}\"... muhahahaha");

            await _connection.SendCoreAsync("RegisterWithName", new object[] { userName });
        }

        private Task Reconnected(string newConnectionId)
        {
            _logger.LogInformation(
                $"Hub reconnected with new id {newConnectionId}, " +
                $"new connection state \"{_connection.State}\"");

            return Task.CompletedTask;
        }

        private Task OnClosed(Exception exception)
        {
            _logger.LogInformation(
                $"Hub connection {_connection.ConnectionId} was closed!\n" +
                $"Reason: {exception}");

            return Task.CompletedTask;
        }

        private void LogMessage(string sender, string message)
        {
            _logger.LogInformation($"User \"{sender}\" published message \"{message}\"");
        }

        public async Task Start()
        {
            await _connection.StartAsync();
            _logger.LogInformation("Surveillance started");
        }
    }
}