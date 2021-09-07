using System.Windows;

namespace GuiClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            DataContext = mainWindowViewModel;
            InitializeComponent();
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
    }
}
