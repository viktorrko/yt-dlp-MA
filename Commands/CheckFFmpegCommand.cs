using ytdlpMA.ViewModels;

namespace ytdlpMA.Commands
{
    class CheckFFmpegCommand : AsyncCommandBase
    {
        private readonly MainViewModel _mainViewModel;
        
        protected override async Task ExecuteAsync(object? parameter)
        {
            if (await Utilities.Utilities.IsFFmpegInstalled())
            {
                _mainViewModel.IsFFmpegInstalled = true;
                _mainViewModel.FFmpegInstallStatusMessage = "FFmpeg installed!";
            }

            else
            {
                _mainViewModel.IsFFmpegInstalled = false;
                _mainViewModel.FFmpegInstallStatusMessage = "Download FFmpeg";
            }
                
        }

        public CheckFFmpegCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}
