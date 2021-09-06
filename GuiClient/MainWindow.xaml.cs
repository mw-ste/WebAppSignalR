using System;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;

namespace GuiClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SignalRClient _signalRClient;

        public MainWindow()
        {
            //_viewModel = viewModel;
            InitializeComponent();
            SendMessageButton.IsEnabled = false;

        }

        public void SetSignalRClient(SignalRClient signalRClient)
        {
            _signalRClient = signalRClient;
        }

        public void LogInfo(string info)
        {
            Log.Text += info + "\r\n";
            Log.ScrollToEnd();
        }

        public void LogMessage(string message)
        {
            ReceivedMessages.Text += message + "\r\n";
            ReceivedMessages.ScrollToEnd();
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            var name = UserName.Text;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            RegisterButton.IsEnabled = false;
            SendMessageButton.IsEnabled = true;

            Dispatcher.InvokeAsync(async () =>
            {
                await _signalRClient.Register(name);
            });
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            var target = Receiver.Text;
            var message = Message.Text;
            Message.Text = "";

            if (string.IsNullOrEmpty(target))
            {
                Dispatcher.InvokeAsync(async () => await _signalRClient.SendMessageToAllClients(message));

            }
            else
            {
                Dispatcher.InvokeAsync(async () => await _signalRClient.SendMessageToClient(target, message));
            }
        }
    }
}
