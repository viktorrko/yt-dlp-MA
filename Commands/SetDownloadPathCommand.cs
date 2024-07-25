using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ytdlpMA.ViewModels;

namespace ytdlpMA.Commands
{
    internal class SetDownloadPathCommand : CommandBase
    {
        private readonly VideoViewModel _videoViewModel;

        /*private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VideoViewModel.FileDownloadPath))
            {
                OnCanExecuteChanged();
            }
        }*/

        public override void Execute(object? parameter)
        {
            using (CommonOpenFileDialog dialog = new())
            {
                dialog.IsFolderPicker = true;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileName != null)
                    _videoViewModel.FileDownloadPath = dialog.FileName;
                else
                    _videoViewModel.FileDownloadPath = String.Empty;
            }
        }

        public SetDownloadPathCommand(VideoViewModel videoViewModel)
        {
            _videoViewModel = videoViewModel;
            // _videoViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }
}
