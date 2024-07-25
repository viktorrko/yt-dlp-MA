using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Iso8601Duration;

namespace ytdlpMA.Classes
{
    class YouTubeAPI
    {
        private static readonly Lazy<YouTubeAPI> _instance = new(() => new YouTubeAPI());

        readonly YouTubeService youtubeService = new(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyCanS2C0nomqmVWwclnE_m87R7rqC16yfQ",
            ApplicationName = "yt-dlp-MA"
        });

        public static YouTubeAPI Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        // private singleton constructor
        private YouTubeAPI() { }

        Google.Apis.YouTube.v3.Data.Video? video;
        
        public async Task RetrieveVideoAsync(string videoId)
        {
            var videoRequest = youtubeService.Videos.List("snippet,contentDetails");
            videoRequest.Id = videoId;
            var videoResponse = await videoRequest.ExecuteAsync();

            if (videoResponse.Items.Count > 0)
            {
                video = videoResponse.Items[0];
            }
        }

        public async Task<bool> IsValidVideoAsync(string videoId)
        {
            try
            {
                var videoRequest = youtubeService.Videos.List("snippet,contentDetails");
                videoRequest.Id = videoId;
                var videoResponse = await videoRequest.ExecuteAsync();

                if (videoResponse.Items.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
            
        }

        public string GetTitle()
        {
            if (video != null)
                return video.Snippet.Title;
            return String.Empty;
        }

        public string GetChannel()
        {
            if (video != null)
                return video.Snippet.ChannelTitle;
            return String.Empty;
        }

        public TimeSpan GetDuration()
        {
            if (video != null)
            {
                var periodBuilder = new PeriodBuilder();
                return periodBuilder.ToTimeSpan(video.ContentDetails.Duration);
            }

            return TimeSpan.Zero;
        }

        public async Task<ImageSource> GetThumbnailAsync()
        {
            if (video != null)
            {
                using (HttpClient httpClient = new())
                {
                    byte[] imageBytes = [];

                    try
                    {
                        imageBytes = await httpClient.GetByteArrayAsync(video.Snippet.Thumbnails.Maxres.Url);
                    }
                    catch
                    {
                        imageBytes = await httpClient.GetByteArrayAsync(video.Snippet.Thumbnails.Default__.Url);
                    }
                    
                    if (imageBytes != null && imageBytes.Length > 0)
                        return Utilities.ByteArrayToBitmapImage(imageBytes);
                    else
                        Console.WriteLine("Failed to download the image.");
                }
            }
            return Utilities.CreateBlankImage(4,4);
        }
    }
}
