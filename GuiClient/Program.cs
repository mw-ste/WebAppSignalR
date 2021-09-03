using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;

namespace GuiClient
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var mainWindow = new MainWindow();

            var connection = new HubConnectionBuilder()
                .WithUrl(new Uri("http://localhost:5000/communicationsignalrhub"))
                .WithAutomaticReconnect(new NeverEndingRetryPolicy(TimeSpan.FromSeconds(10), mainWindow.LogInfo))
                .Build();

            var signalRClient = new SignalRClient(connection, mainWindow.LogMessage, mainWindow.LogInfo);

            try
            {
                signalRClient.StartConnection().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show(
                    e.ToString(), 
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            mainWindow.SetSignalRClient(signalRClient);
            var application = new System.Windows.Application();
            application.Run(mainWindow);
        }
    }
}