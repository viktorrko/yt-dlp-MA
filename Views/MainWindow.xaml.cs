using ytdlpMA.Classes;
using MaterialDesignThemes.Wpf;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ytdlpMA.ViewModels;
using MaterialDesignColors.Recommended;

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
            FFmpegCheck();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ConsoleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConsoleTextBox.ScrollToEnd();
        }

        private void FFmpegCheck()
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "ffmpeg",
                Arguments = "-h",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;

                try
                {
                    process.Start();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        FFmpegCheckButton.Background = Brushes.Green;
                        FFmpegCheckButton.Content = new PackIcon { Kind = PackIconKind.Check, Foreground = Brushes.White, Width=24, Height=24 };
                        Console.WriteLine("FFmpeg found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while executing the process: " + ex.Message);
                    Console.WriteLine("FFmpeg not found.");
                }
            }
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