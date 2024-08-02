using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ytdlpMA.ViewModels;

namespace ytdlpMA.Views
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            // FFmpegCheck();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ConsoleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConsoleTextBox.ScrollToEnd();
        }

        private void CloseButton_Click(Object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ToggleDrawer_Click(object sender, RoutedEventArgs e)
        {
            DrawerHost.IsRightDrawerOpen = !DrawerHost.IsRightDrawerOpen;
        }

        
    }
}