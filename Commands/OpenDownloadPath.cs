using ytdlpMA.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ytdlpMA.ViewModels;

namespace ytdlpMA.Commands
{
    internal class OpenDownloadPath : CommandBase
    {
        private readonly VideoViewModel _videoViewModel;

        public override bool CanExecute(object? parameter)
        {
            return _videoViewModel.FileDownloadPath != String.Empty && !_videoViewModel.HasErrors && base.CanExecute(parameter);
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VideoViewModel.FileDownloadPath))
            {
                OnCanExecuteChanged();
            }
        }

        public override void Execute(object? parameter)
        {
            if (_videoViewModel.FileDownloadPath != String.Empty)
                Process.Start("explorer.exe", _videoViewModel.FileDownloadPath);
            else
                _videoViewModel.ErrorSnackbarMessageQueue.Enqueue("No download path set.");
        }

        public OpenDownloadPath(VideoViewModel videoViewModel)
        {
            _videoViewModel = videoViewModel;
            _videoViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }
}
