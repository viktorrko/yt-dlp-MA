using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace Classes
{
    class YouTubeAPIHandler
    {
        YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyCanS2C0nomqmVWwclnE_m87R7rqC16yfQ",
            ApplicationName = "yt-dld-MA"
        });
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

        public async static Task<bool> IsValidVideoAsync(string videoId)
        {
            YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyCanS2C0nomqmVWwclnE_m87R7rqC16yfQ",
                ApplicationName = "yt-dld-MA"
            });

            var videoRequest = youtubeService.Videos.List("snippet,contentDetails");
            videoRequest.Id = videoId;
            var videoResponse = await videoRequest.ExecuteAsync();

            if (videoResponse.Items.Count > 0)
            {
                return true;
            }
            return false;
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
                string durationString = video.ContentDetails.Duration.TrimStart('P', 'T');

                int indexOfM = durationString.IndexOf('M');
                

                int minutes = int.Parse(durationString.Substring(0, indexOfM));
                durationString = durationString.Substring(indexOfM + 1);

                int indexOfS = durationString.IndexOf('S');
                int seconds = int.Parse(durationString.Substring(0, indexOfS));

                return TimeSpan.FromMinutes(minutes).Add(TimeSpan.FromSeconds(seconds));
            }

            return TimeSpan.Zero;
        }

        public async Task<byte[]> GetThumbnailAsync()
        {
            if (video != null)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(video.Snippet.Thumbnails.Maxres.Url);

                    if (imageBytes != null && imageBytes.Length > 0)
                        return imageBytes;
                    else
                        Console.WriteLine("Failed to download the image.");
                }
            }
            return [];
        }
    }
}
