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

            Log.TextChanged += (_, _) => Log.ScrollToEnd();
            ReceivedMessages.TextChanged += (_, _) => ReceivedMessages.ScrollToEnd();
        }
    }
}
