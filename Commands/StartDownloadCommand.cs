using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using ytdlpMA.ViewModels;

namespace ytdlpMA.Commands
{
    class StartDownloadCommand : AsyncCommandBase
    {
        // TODO: add stop button (kill process)
        private readonly VideoViewModel _videoViewModel;
        public static readonly string ytdlp = @"Resources\exe\yt-dlp.exe";
        private bool _running = false;

        public override bool CanExecute(object? parameter)
        {
            return _videoViewModel.QueuedUrl != String.Empty && _videoViewModel.FileDownloadPath != String.Empty && !_running && base.CanExecute(parameter);
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VideoViewModel.QueuedUrl) ||  e.PropertyName == nameof(VideoViewModel.FileDownloadPath))
            {
                OnCanExecuteChanged();
            }
        }

        private void ProcessOutputHandler(object sender, DataReceivedEventArgs line)
        {
            _videoViewModel.ConsoleText += line.Data + Environment.NewLine;

            if (line.Data != null)
            {
                if (line.Data.StartsWith("[download]"))
                {
                    _videoViewModel.DownloadProgressBarIndeterminate = false;
                    _videoViewModel.DownloadProgressBarValue = Classes.Utilities.ParseDownloadProgress(line.Data);
                }
                else
                {
                    _videoViewModel.DownloadProgressBarIndeterminate = true;
                    _videoViewModel.DownloadProgressBarValue = 0;
                }
            }

        }

        protected override async Task ExecuteAsync(object? parameter)
        {
            _running = true;
            OnCanExecuteChanged();
            _videoViewModel.ConsoleText = string.Empty;
            _videoViewModel.DownloadProgressBarValue = 0;
            _videoViewModel.DownloadProgressBarIndeterminate = true;

            ProcessStartInfo startInfo = new()
            {
                FileName = Path.GetFullPath(ytdlp),
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = String.Join(" ", _videoViewModel._video.BuildCommand()),

            };

            _videoViewModel.ConsoleText = "yt-dlp " + String.Join(" ", _videoViewModel._video.BuildCommand()) + Environment.NewLine + Environment.NewLine;

            using (var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true })
            {
                process.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputHandler);
                process.ErrorDataReceived += new DataReceivedEventHandler(ProcessOutputHandler);
                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                _videoViewModel.DownloadProgressBarValue = 0;
                _videoViewModel.DownloadProgressBarIndeterminate = false;

                _videoViewModel.ConsoleText += ("[PROCESS ENDED]" + Environment.NewLine);

                if (process.ExitCode == 0)
                {
                    _videoViewModel.SuccessSnackbarMessageQueue.Enqueue("Process exited without errors.");
                }
                else
                {
                    _videoViewModel.ErrorSnackbarMessageQueue.Enqueue("Process exited with errors.");
                }

                _running = false;
            }   
        }

        public StartDownloadCommand(VideoViewModel videoViewModel)
        {
            _videoViewModel = videoViewModel;
            _videoViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }
}
