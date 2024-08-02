using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ytdlpMA.Utilities
{
    class Utilities
    {
        public static bool IsValidUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, result: out Uri? resultUri) && resultUri.Scheme == Uri.UriSchemeHttps)
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
                return url.Split(new[] { "youtu.be/" }, StringSplitOptions.RemoveEmptyEntries)[1].Split('?')[0];
            }
            else
            {
                return string.Empty;
            }
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] imageData)
        {
            BitmapImage bitmapImage = new();
            using (MemoryStream memoryStream = new(imageData))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static BitmapImage ImageToBitmapImage(Image image)
        {
            Bitmap bitmap = (Bitmap)image;
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memStream = new MemoryStream())
            {
                bitmap.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);
                memStream.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = null;
                bitmapImage.StreamSource = memStream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static ImageSource CreateBlankImage(int width, int height)
        {
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];

            // Fill the array with black pixels (ARGB format)
            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = 0;   // Blue
                pixels[i + 1] = 0; // Green
                pixels[i + 2] = 0; // Red
                pixels[i + 3] = 255; // Alpha
            }

            BitmapSource bitmapSource = BitmapSource.Create(
                width, height, 96, 96, PixelFormats.Bgra32, null, pixels, stride);

            return bitmapSource;
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

        public static async Task<bool> IsFFmpegInstalled()
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "ffmpeg",
                Arguments = "-h",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;

                try
                {
                    process.Start();
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0)
                        return true;
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
