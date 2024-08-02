using Microsoft.WindowsAPICodePack.Dialogs;
using ytdlpMA.ViewModels;

namespace ytdlpMA.Commands
{
    internal class SetDownloadPathCommand : CommandBase
    {
        private readonly VideoViewModel _videoViewModel;

        public override void Execute(object? parameter)
        {
            using (CommonOpenFileDialog dialog = new())
            {
                dialog.IsFolderPicker = true;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileName != null)
                    _videoViewModel.FileDownloadPath = dialog.FileName;
            }
        }

        public SetDownloadPathCommand(VideoViewModel videoViewModel)
        {
            _videoViewModel = videoViewModel;
        }
    }
}
