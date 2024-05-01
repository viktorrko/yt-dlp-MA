using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Classes
{
    class VideoItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Channel {  get; set; }
        public TimeSpan Duration { get; set; }
        public byte[] ThumbnailData { get; set; }
        private YouTubeAPIHandler YouTubeAPI;

        public VideoItem()
        {
            this.Id = string.Empty;
            this.Title = string.Empty;
            this.Channel = string.Empty;
            this.Duration = TimeSpan.Zero;
            this.ThumbnailData = [];
            this.YouTubeAPI = new YouTubeAPIHandler();
        }

        public async Task UpdateUrlAsync(string url)
        {
            string newId = Utils.ExtractYouTubeVideoId(url);
            await YouTubeAPI.RetrieveVideoAsync(newId);

            if (YouTubeAPI.GetTitle() != String.Empty)
            {
                this.Id = newId;
                this.Title = YouTubeAPI.GetTitle();
                this.Channel = YouTubeAPI.GetChannel();
                this.Duration = YouTubeAPI.GetDuration();
                this.ThumbnailData = await YouTubeAPI.GetThumbnailAsync();
            }
        }
    }
}
