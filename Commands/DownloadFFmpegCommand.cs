using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using ytdlpMA.ViewModels;
using System.Windows;

namespace ytdlpMA.Commands
{
    class DownloadFFmpegCommand : AsyncCommandBase
    {
        private readonly MainViewModel _mainViewModel;
        private readonly string url = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip";
        private readonly string destinationFilePath = Path.Join(Path.GetTempPath(), "ffmpeg.7z");
        private readonly string extractDestinationPath = Path.Join(Path.GetTempPath(), "ffmpeg");
        //private readonly string installPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "FFmpeg");
        private readonly string installPath = Path.Join(AppContext.BaseDirectory, @"Resources\exe\ffmpeg.exe");

        public override bool CanExecute(object? parameter)
        {
            return !_mainViewModel.IsFFmpegInstalled && base.CanExecute(parameter);
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.IsFFmpegInstalled))
            {
                OnCanExecuteChanged();
            }
        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            _mainViewModel.ShowFFmpegInstallButtonIndicator = true;
            _mainViewModel.FFmpegInstallStatusMessage = "Downloading FFmpeg...";

            if (Directory.Exists(extractDestinationPath))
            {
                Directory.Delete(extractDestinationPath, true);
            }

            // downloads FFmpeg from the provided static URL
            if (!File.Exists(destinationFilePath))
            {
                try
                {
                    using HttpClient httpClient = new();
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                           fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }
                catch
                {
                    _mainViewModel.ErrorSnackbarMessageQueue.Enqueue("Download failed.");
                    _mainViewModel.FFmpegInstallStatusMessage = "Failed to download FFmpeg";
                }
            }

            // extracts and moves the contents of the downloaded FFmpeg zip
            _mainViewModel.FFmpegInstallStatusMessage = "Installing FFmpeg...";
            try
            {
                _mainViewModel.FFmpegInstallStatusMessage = "Extracting the archive...";
                // TODO: File.Move freezes the UI for some reason
                await Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(destinationFilePath, extractDestinationPath);
                    File.Move(Path.Join(Directory.GetDirectories(extractDestinationPath)[0], @"bin\ffmpeg.exe"), installPath);
                });
            }
            catch (IOException)
            {
                _mainViewModel.ErrorSnackbarMessageQueue.Enqueue($"{installPath} already exists.");
                _mainViewModel.FFmpegInstallStatusMessage = "Failed to install FFMpeg";
            }

            // checks the installation
            await Application.Current.Dispatcher.InvokeAsync(() => _mainViewModel.CheckFFmpeg.Execute(null));
            
            if (await Utilities.Utilities.IsFFmpegInstalled())
                _mainViewModel.FFmpegInstallStatusMessage = "FFmpeg installed!";
            else
                _mainViewModel.FFmpegInstallStatusMessage = "Failed to install FFMpeg";

            _mainViewModel.ShowFFmpegInstallButtonIndicator = false;
        }

        private Task MoveFileAsync(string sourceFilePath, string destFilePath)
        {
            return Task.Run(() =>
            {
                File.Move(sourceFilePath, destFilePath);
            });
        }

        public DownloadFFmpegCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _mainViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }
}
