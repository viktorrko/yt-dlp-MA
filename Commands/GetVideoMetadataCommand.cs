using YoutubeExplode;
using System.ComponentModel;
using System.Windows;
using ytdlpMA.ViewModels;
using YoutubeExplode.Common;
using System.Net.Http;
using WebP.Net;

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
            string newId = Utilities.Utilities.ExtractYouTubeVideoId(_videoViewModel.Url);

            var youtube = new YoutubeClient();
            try
            {
                var video = await youtube.Videos.GetAsync(newId);
                _videoViewModel.Title = video.Title;
                _videoViewModel.Channel = video.Author.ChannelTitle;
                if (video.Duration != null)
                    _videoViewModel.Duration = video.Duration.Value;
                else
                    _videoViewModel.Duration = TimeSpan.Zero;

                byte[] thumbnailBytes = [];

                using (HttpClient httpClient = new())
                    thumbnailBytes = await httpClient.GetByteArrayAsync(video.Thumbnails.GetWithHighestResolution().Url);

                if (video.Thumbnails.GetWithHighestResolution().Url.Contains("webp"))
                {
                    _videoViewModel.Thumbnail = Utilities.Utilities.ImageToBitmapImage(new WebPObject(thumbnailBytes).GetImage());
                }
                else
                {
                    _videoViewModel.Thumbnail = Utilities.Utilities.ByteArrayToBitmapImage(thumbnailBytes);
                }

                _videoViewModel.QueuedUrl = _videoViewModel.Url;
            }
            catch
            {
                _videoViewModel.Title = "Title";
                _videoViewModel.Channel = "Channel";
                _videoViewModel.Duration = TimeSpan.Zero;
                _videoViewModel.Thumbnail = Utilities.Utilities.CreateBlankImage(4, 4);
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
