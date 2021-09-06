using Shared;

namespace GuiClient
{
    public class MainWindowViewModel
    {
        private readonly ISignalRClient _signalRClient;

        public MainWindowViewModel(ISignalRClient signalRClient)
        {
            _signalRClient = signalRClient;
        }
    }
}