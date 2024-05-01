using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using Classes;

namespace Utilities
{
    class Utils
    {
        public static bool IsValidUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, result: out Uri resultUri) && resultUri.Scheme == Uri.UriSchemeHttps)
                return true;
            
            return false;
        }

        public static string ExtractYouTubeVideoId(string url)
        {

            if (url.Contains("youtube.com/watch?v="))
            {
                return url.Split(new[] { "v=" }, StringSplitOptions.RemoveEmptyEntries)[1].Split('&')[0];
            }
            else if (url.Contains("youtu.be/"))
            {
                return url.Split(new[] { "youtu.be/" }, StringSplitOptions.RemoveEmptyEntries)[1].Split('&')[0];
            }
            else
            {
                return string.Empty;
            }
        }

        public static async Task<bool> IsValidYouTubeUrlAsync(string url)
        {
            if (IsValidUrl(url))
            {
                string Id = ExtractYouTubeVideoId(url);
                if (Id != String.Empty)
                {
                    if (await YouTubeAPIHandler.IsValidVideoAsync(Id))
                        return true;
                }
            }
            
            return false;
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] imageData)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream(imageData))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static int ParseDownloadProgress(string line)
        {
            string pattern = @"(\d+\.?\d*)%";

            // Create a Regex object
            Regex regex = new Regex(pattern);

            // Find matches
            Match match = regex.Match(line);

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value.Split('.')[0]);
            }
            else
            {
                return 0;
            }
        }
    }
}
