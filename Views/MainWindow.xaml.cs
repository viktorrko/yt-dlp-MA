using Classes;
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
using Utilities;

namespace Views
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly VideoItem videoItem = new VideoItem();
        readonly DownloadItem downloadItem = new DownloadItem();

        public MainWindow()
        {
            InitializeComponent();
            DestinationTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void UrlReloadButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            ThumbnailLoadingProgressBar.Visibility = Visibility.Visible;
            ThumbnailBorder.Background.Opacity = 0.5;

            if (await Utils.IsValidYouTubeUrlAsync(LinkTextBox.Text))
            {
                await videoItem.UpdateUrlAsync(LinkTextBox.Text);
                downloadItem.Url = LinkTextBox.Text;
                TitleTextBlock.Text = videoItem.Title;
                ChannelTextBlock.Text = videoItem.Channel;
                DurationTextBlock.Text = videoItem.Duration.ToString();
                ThumbnailBorder.Background = new ImageBrush { ImageSource = Utils.ByteArrayToBitmapImage(videoItem.ThumbnailData), Stretch = Stretch.UniformToFill };
            }
            else
                ErrorSnackbar.MessageQueue.Enqueue("Invalid link.");

            ThumbnailLoadingProgressBar.Visibility = Visibility.Hidden;
            ThumbnailBorder.Background.Opacity = 1;
        }

        private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string newDestination = DownloadItem.SetDestinationPath();
            if (newDestination != String.Empty)
            {
                DestinationTextBox.Text = newDestination;
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = String.Empty;
            DownloadProgressBar.IsIndeterminate = true;
            DownloadProgressBar.Value = 0;

            downloadItem.Extension = ((ComboBoxItem)ExtensionComboBox.SelectedItem).Content.ToString();
            
            downloadItem.DestinationPath = DestinationTextBox.Text;
            downloadItem.CustomArguments = CustomArgumentsTextBox.Text;
            downloadItem.Switches["convert"] = ConvertChip.IsSelected;
            downloadItem.Switches["mtime"] = LastModifiedChip.IsSelected;
            downloadItem.Switches["thumbnail"] = EmbedThumbnailChip.IsSelected;
            downloadItem.Switches["metadata"] = EmbedMetadataChip.IsSelected;

            ProcessStartInfo startInfo = new()
            {
                FileName = Path.GetFullPath(DownloadItem.ytdlp),
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                Arguments = String.Join(" ", downloadItem.ArgumentsListBuilder()),

            };

            OutputTextBox.AppendText("yt-dlp " + startInfo.Arguments + Environment.NewLine + Environment.NewLine);
            
            Process process = new Process();
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputHandler);
            process.Exited += new EventHandler(ProcessExitHandler);
            process.Start();
            process.BeginOutputReadLine();
        }

        private void ProcessOutputHandler(object sender, DataReceivedEventArgs line)
        {
            Dispatcher.Invoke(() =>
            {
                LinkTab.IsEnabled = false;
                SettingsTab.IsEnabled = false;
                DownloadButtonsStackPanel.IsEnabled = false;
                OutputTextBox.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 153, 255));
                
                OutputTextBox.AppendText(line.Data + Environment.NewLine);
                OutputTextBox.ScrollToEnd();

                if (line.Data != null)
                {
                    if (line.Data.StartsWith("[download]"))
                    {
                        DownloadProgressBar.IsIndeterminate = false;
                        DownloadProgressBar.Value = Utils.ParseDownloadProgress(line.Data);
                    }
                    else
                    {
                        DownloadProgressBar.IsIndeterminate = true;
                        DownloadProgressBar.Value = 0;
                    }  
                }
            });
        }

        private void ProcessExitHandler(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DownloadProgressBar.Value = 0;
                DownloadProgressBar.IsIndeterminate = false;
                OutputTextBox.BorderBrush = null;

                LinkTab.IsEnabled = true;
                SettingsTab.IsEnabled = true;
                DownloadButtonsStackPanel.IsEnabled = true;

                OutputTextBox.AppendText("[PROCESS ENDED]" + Environment.NewLine);
            });
        }

        private void OpenDownloadFolderButton_Click(object sender, EventArgs e)
        {
            if (downloadItem.DestinationPath != String.Empty)
                Process.Start("explorer.exe", downloadItem.DestinationPath);
        }

        private void CloseButton_Click(Object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}