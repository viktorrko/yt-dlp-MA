using ytdlpMA.Commands;
using ytdlpMA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        // CONSTRUCTOR
        public MainViewModel()
        {
            _video = new VideoViewModel();
        }
    }
}
