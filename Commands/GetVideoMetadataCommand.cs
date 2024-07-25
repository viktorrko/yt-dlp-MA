using ytdlpMA.Classes;
using Google.Apis.YouTube.v3.Data;
using ytdlpMA.Models;
using System.ComponentModel;
using System.Security.Policy;
using System.Windows;
using ytdlpMA.ViewModels;

namespace ytdlpMA.Commands
{
    internal class GetVideoMetadataCommand : AsyncCommandBase
    {
        private readonly VideoViewModel _videoViewModel;

        public override bool CanExecute(object? parameter)
        {
            bool temp = !string.IsNullOrEmpty(_videoViewModel.Url) && !_videoViewModel.HasErrors && base.CanExecute(parameter);
            return temp;
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VideoViewModel.Url))
            {
                OnCanExecuteChanged();
            }
        }

        // actual logic that gets executed
        protected override async Task ExecuteAsync(object? parameter)
        {
            _videoViewModel.LoadingBarVisible = Visibility.Visible;
            _videoViewModel.ThumbnailOpacity = 0.5f;
            string newId = Classes.Utilities.ExtractYouTubeVideoId(_videoViewModel.Url);
            

            if (await YouTubeAPI.Instance.IsValidVideoAsync(newId))
            {
                await YouTubeAPI.Instance.RetrieveVideoAsync(newId);
                _videoViewModel.Id = newId;
                _videoViewModel.Title = YouTubeAPI.Instance.GetTitle();
                _videoViewModel.Channel = YouTubeAPI.Instance.GetChannel();
                _videoViewModel.Duration = YouTubeAPI.Instance.GetDuration();
                _videoViewModel.Thumbnail = await YouTubeAPI.Instance.GetThumbnailAsync();
                _videoViewModel.QueuedUrl = _videoViewModel.Url;
            }
            else
            {
                _videoViewModel.Title = "Title";
                _videoViewModel.Channel = "Channel";
                _videoViewModel.Duration = TimeSpan.Zero;
                _videoViewModel.Thumbnail = Classes.Utilities.CreateBlankImage(4, 4);
                _videoViewModel.ErrorSnackbarMessageQueue.Enqueue("Video not found.");
                _videoViewModel.QueuedUrl = String.Empty;
            }

            _videoViewModel.LoadingBarVisible = Visibility.Hidden;
            _videoViewModel.ThumbnailOpacity = 1;

        }

        // CONSTRUCTOR
        public GetVideoMetadataCommand(VideoViewModel videoViewModel)
        {
            _videoViewModel = videoViewModel;
            _videoViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }
}
