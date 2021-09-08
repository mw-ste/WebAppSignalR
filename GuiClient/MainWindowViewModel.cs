using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GuiClient
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly SignalRClient _signalRClient;

        private bool _registered;
        private string _messageLog;
        private string _infoLog;
        private string _userName;
        private string _message;
        private string _receiver;

        public MainWindowViewModel(SignalRClient signalRClient)
        {
            _signalRClient = signalRClient;

            RegisterCommand = new Command(
                async _ => await Register(),
                _ => !Registered && !string.IsNullOrEmpty(UserName));

            SendMessageCommand = new Command(
                async _ => await SendMessage(),
                _ => Registered && !string.IsNullOrEmpty(Message));

            Subscribe();
        }

        private void Subscribe()
        {
            _signalRClient.MessageReceived += (user, message) => MessageLog += $"{user}: {message}\n";

            _signalRClient.MessageSent += () => InfoLog += "Message sent\n";
            _signalRClient.UserJoined += user => InfoLog += $"User {user} joined\n";
            _signalRClient.UserLeft += user => InfoLog += $"User {user} left\n";
        }

        private async Task Register()
        {
            await _signalRClient.Register(UserName);

            Registered = true;

            RegisterCommand.NotifyCanExecuteChanged();
            SendMessageCommand.NotifyCanExecuteChanged();
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrEmpty(Receiver))
            {
                await _signalRClient.SendMessageToAllClients(Message);
            }
            else
            {
                await _signalRClient.SendMessageToClient(Receiver, Message);
            }

            MessageLog += $"{UserName}: {Message}\n";
            Message = string.Empty;
        }

        public Command RegisterCommand { get; }
        public Command SendMessageCommand { get; }

        public bool Registered
        {
            get => _registered;
            set => SetProperty(nameof(Registered), ref _registered, value);
        }

        public string MessageLog
        {
            get => _messageLog;
            set => SetProperty(nameof(MessageLog), ref _messageLog, value);
        }

        public string InfoLog
        {
            get => _infoLog;
            set => SetProperty(nameof(InfoLog), ref _infoLog, value);
        }

        public string UserName
        {
            get => _userName;
            set
            {
                SetProperty(nameof(UserName), ref _userName, value);
                RegisterCommand.NotifyCanExecuteChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                SetProperty(nameof(Message), ref _message, value);
                SendMessageCommand.NotifyCanExecuteChanged();
            }
        }

        public string Receiver
        {
            get => _receiver;
            set => SetProperty(nameof(Receiver), ref _receiver, value);
        }

        private void SetProperty<T>(string propertyName, ref T currentValue, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                return;
            }

            currentValue = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}