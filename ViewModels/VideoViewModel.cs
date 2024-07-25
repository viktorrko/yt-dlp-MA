using ytdlpMA.Classes;
using ytdlpMA.Commands;
using MaterialDesignThemes.Wpf;
using ytdlpMA.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ytdlpMA.ViewModels
{
    class VideoViewModel : ViewModelBase , INotifyDataErrorInfo
    {
        public readonly Video _video = new();

        // URL BINDING
        public string Url
        {
            get => _video.Url;
            set
            {
                if (_video.Url != value)
                {
                    _video.Url = value;
                    // URL validation
                    ValidateUrl();
                    UrlMatchesQueuedUrl();
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        // the URL that is actually passed to the yt-dlp command
        public string QueuedUrl
        {
            get => _video.QueuedUrl;
            set
            {
                if (_video.QueuedUrl != value)
                {
                    _video.QueuedUrl = value;
                    OnPropertyChanged(nameof(QueuedUrl));
                }
            }
        }

        // METADATA BINDINGS
        public string Id
        {
            get => _video.Id;
            set
            {
                if (_video.Id != value)
                {
                    _video.Id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Title
        {
            get => _video.Title;
            set
            {
                if( _video.Title != value)
                {
                    _video.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Channel
        {
            get => _video.Channel;
            set
            {
                if (_video.Channel != value)
                {
                    _video.Channel = value;
                    OnPropertyChanged(nameof(Channel));
                }
            }
        }

        public TimeSpan Duration
        {
            get => _video.Duration;
            set
            {
                if (_video.Duration != value)
                {
                    _video.Duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        public ImageSource Thumbnail
        {
            get => _video.Thumbnail;
            set
            {
                if (_video.Thumbnail != value)
                {
                    _video.Thumbnail = value;
                    OnPropertyChanged(nameof(Thumbnail));
                }
            }
        }

        private float _thumbnailOpacity = 1;
        public float ThumbnailOpacity
        {
            get => _thumbnailOpacity;
            set
            {
                if (_thumbnailOpacity != value)
                {
                    _thumbnailOpacity = value;
                    OnPropertyChanged(nameof(ThumbnailOpacity));

                }
            }
        }

        // DOWNLOAD OPTION BINDINGS
        // feeds the extension ComboBox from a list of extensions defined in 'Video' class
        public IEnumerable<string> FileExtensionList => _video.FileExtensionList;

        // collects the selected extension ComboBox value, and sets it in the class instance
        public string FileExtension
        {
            get => _video.FileExtension;
            set
            {
                if (_video.FileExtension != value)
                {
                    _video.FileExtension = value;
                    OnPropertyChanged(nameof(FileExtension));
                }
            }
        }

        public string CustomDownloadArguments
        {
            set
            {
                if (_video.CustomDownloadArguments != value)
                {
                    _video.CustomDownloadArguments = value;
                    OnPropertyChanged(nameof(CustomDownloadArguments));
                }
            }
        }

        public string CustomFileName
        {
            get => _video.CustomFileName;
            set
            {
                if (_video.CustomFileName != value)
                {
                    _video.CustomFileName = value;
                    OnPropertyChanged(nameof(CustomFileName));
                }
            }
        }

        // set is only used for filling the TextBox with user folder on startup
        public string FileDownloadPath
        {
            get => _video.FileDownloadPath;
            set
            {
                if (_video.FileDownloadPath != value)
                {
                    _video.FileDownloadPath = value;
                    ValidateDownloadPath();
                    OnPropertyChanged(nameof(FileDownloadPath));
                }
            }
        }

        // retrieves the chips setup in the constructor of the Video class and wraps them in ObservableCollection
        public ObservableCollection<DownloadToggleOption> DownloadToggleOptions => new(_video.DownloadToggleOptions);

        // DOWNLOAD PROGRESS OPTIONS
        
        // download progress bar value
        private int _downloadProgressBarValue;
        public int DownloadProgressBarValue
        {
            get => _downloadProgressBarValue;
            set
            {
                if (_downloadProgressBarValue != value)
                {
                    _downloadProgressBarValue = value;
                    OnPropertyChanged(nameof(DownloadProgressBarValue));
                }
            }
        }

        // if true, the progress bar loads (on converting, loading, etc)
        private bool _downloadProgressBarIndeterminate;
        public bool DownloadProgressBarIndeterminate
        {
            get => _downloadProgressBarIndeterminate;
            set
            {
                if (_downloadProgressBarIndeterminate != value)
                {
                    _downloadProgressBarIndeterminate = value;
                    OnPropertyChanged(nameof(DownloadProgressBarIndeterminate));
                }
            }
        }

        // holds the console output text
        private string _consoleText;
        public string ConsoleText
        {
            get => _consoleText;
            set
            {
                if (_consoleText != value)
                {
                    _consoleText = value;
                    OnPropertyChanged(nameof(ConsoleText));
                }
            }
        }

        // INPUT VALIDATION
        private Dictionary<string, List<string>> _errors = [];
        public bool HasErrors => _errors.Count != 0;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
                return null;

            return _errors[propertyName];
        }

        // URL validator logic
        private void ValidateUrl()
        {
            const string pattern = @"^(https?\:\/\/)?(www\.youtube\.com|youtu\.?be)\/.+$";
            Regex regex = new(pattern);

            if (string.IsNullOrWhiteSpace(Url))
            {
                _errors.Remove(nameof(Url));
            }
            else if (!regex.IsMatch(Url))
            {
                _errors[nameof(Url)] = ["Enter Valid YouTube URL."];
            }
            else
            {
                _errors.Remove(nameof(Url));
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Url)));
        }

        private void UrlMatchesQueuedUrl()
        {
            if (String.Compare(Url, QueuedUrl) == 0)
            {
                ThumbnailOpacity = 1;
            }
            else
            {
                ThumbnailOpacity = 0.5f;
            }
        }

        // download path validator logic
        private void ValidateDownloadPath()
        {
            if (!Directory.Exists(Path.GetFullPath(FileDownloadPath)))
                _errors[nameof(FileDownloadPath)] = ["Enter valid path."];
            else
                _errors.Remove(nameof(FileDownloadPath));

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Url)));
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

        // VIDEO METADATA LOADING BAR
        private Visibility _loadingBarVisible = Visibility.Hidden;
        public Visibility LoadingBarVisible
        {
            get => _loadingBarVisible;
            set
            {
                _loadingBarVisible = value;
                OnPropertyChanged(nameof(LoadingBarVisible));
            }
        }

        // COMMANDS
        public ICommand GetVideoMetadata { get; }
        public ICommand SetDownloadPath { get; }
        public ICommand OpenDownloadPath { get; }
        public ICommand StartDownload { get; }

        // CONSTRUCTOR
        public VideoViewModel()
        {
            // commands
            GetVideoMetadata = new GetVideoMetadataCommand(this);
            SetDownloadPath = new SetDownloadPathCommand(this);
            OpenDownloadPath = new OpenDownloadPath(this);
            StartDownload = new StartDownloadCommand(this);


            // initial values
            DownloadProgressBarValue = 0;
            DownloadProgressBarIndeterminate = false;
            _consoleText = string.Empty;
        }
    }
}