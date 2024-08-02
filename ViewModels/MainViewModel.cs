using MaterialDesignThemes.Wpf;
using System.IO;
using System.Windows.Input;
using ytdlpMA.Commands;

namespace ytdlpMA.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        // Video ViewModel bindings
        private VideoViewModel _video;

        public VideoViewModel Video
        {
            get => _video;
            set
            {
                _video = value;
                OnPropertyChanged(nameof(Video));
            }
        }

        // SETTINGS BINDINGS
        private bool _showConsole = false;
        public bool ShowConsole
        {
            get => _showConsole;
            set
            {
                if (_showConsole != value)
                {
                    _showConsole = value;
                    OnPropertyChanged(nameof(ShowConsole));
                }
            }
        }

        private int _consoleHeight = 150;
        public int ConsoleHeight
        {
            get => _consoleHeight;
            set
            {
                if (_consoleHeight != value)
                {
                    _consoleHeight = value;
                    OnPropertyChanged(nameof(ConsoleHeight));
                }
            }
        }

        private bool _isFFmpegInstalled = false;
        public bool IsFFmpegInstalled
        {
            get => _isFFmpegInstalled;
            set
            {
                if (value != _isFFmpegInstalled)
                {
                    _isFFmpegInstalled = value;
                    OnPropertyChanged(nameof(IsFFmpegInstalled));
                }   
            }
        }

        private string _FFmpegInstallStatusMessage = "Download FFmpeg";
        public string FFmpegInstallStatusMessage
        {
            get => _FFmpegInstallStatusMessage;
            set
            {
                if (value != _FFmpegInstallStatusMessage)
                {
                    _FFmpegInstallStatusMessage = value;
                    OnPropertyChanged(nameof(FFmpegInstallStatusMessage));
                }
            }
        }

        private bool _showFFmpegInstallButtonIndicator = false;
        public bool ShowFFmpegInstallButtonIndicator
        {
            get => _showFFmpegInstallButtonIndicator;
            set
            {
                _showFFmpegInstallButtonIndicator = value;
                OnPropertyChanged(nameof(ShowFFmpegInstallButtonIndicator));
            }
        }

        // ERROR SNACKBAR QUEUE
        private SnackbarMessageQueue _errorSnackbarMessageQueue = new();
        public SnackbarMessageQueue ErrorSnackbarMessageQueue
        {
            get => _errorSnackbarMessageQueue;
            set
            {
                _errorSnackbarMessageQueue = value;
                OnPropertyChanged(nameof(SnackbarMessageQueue));
            }
        }

        private SnackbarMessageQueue _successSnackbarMessageQueue = new();
        public SnackbarMessageQueue SuccessSnackbarMessageQueue
        {
            get => _successSnackbarMessageQueue;
            set
            {
                _successSnackbarMessageQueue = value;
                OnPropertyChanged(nameof(SnackbarMessageQueue));
            }
        }

        // COMMANDS
        public ICommand CheckFFmpeg { get; }
        public ICommand DownloadFFmpeg { get; }
        
        // CONSTRUCTOR
        public MainViewModel()
        {
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process) + ";" + Path.Join(AppContext.BaseDirectory, @"Resources\exe"), EnvironmentVariableTarget.Process);

            DownloadFFmpeg = new DownloadFFmpegCommand(this);
            CheckFFmpeg = new CheckFFmpegCommand(this);
            CheckFFmpeg.Execute(null);
            
            _video = new VideoViewModel(this);
        }
    }
}
