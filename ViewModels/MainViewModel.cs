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

        // CONSTRUCTOR
        public MainViewModel()
        {
            _video = new VideoViewModel();
        }
    }
}
