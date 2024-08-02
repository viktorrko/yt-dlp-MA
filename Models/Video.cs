// class definiton for video metadata and download options
using ytdlpMA.Utilities;
using System.IO;
using System.Windows.Media;

namespace ytdlpMA.Models
{
    public class Video
    {
        public readonly string[] FileExtensionList = ["mp3", "wav", "flac"];
        
        // PARAMETERS
        public string Url { get; set; }
        public string QueuedUrl { get; set; }
        public string Title { get; set; }
        public string Channel {  get; set; }
        public TimeSpan Duration { get; set; }
        public ImageSource Thumbnail { get; set; }
        public string FileDownloadPath { get; set; }
        public string FileExtension { get; set; }
        public List<DownloadToggleOption> DownloadToggleOptions { get; set; }
        public string CustomDownloadArguments { get; set; }
        public string CustomFileName { get; set; }

        // METHODS
        public List<string> BuildCommand()
        {
            List<string> command = [];
            // command.Add($"--ffmpeg-location \"{Path.GetFullPath(FFmpeg)}\"");
            command.Add("--newline");
            command.Add($"-i {this.QueuedUrl.Split("&")[0]}");
            command.Add("-f bestaudio");
            if (CustomFileName == string.Empty || CustomFileName == null)
                CustomFileName = "%(title)s.%(ext)s";
            command.Add($"-o \"{Path.Combine(this.FileDownloadPath, CustomFileName)}\"");
            command.Add("--force-overwrites");

            // checks for options
            foreach (var option in DownloadToggleOptions)
            {
                if (option.Value)
                    command.Add(option.Option);
            }

            // special actions if convert is checked
            if (command.Contains("-x"))
            {
                command.Add($"--audio-format {FileExtension}");

                if (FileExtension == "mp3")
                    command.Add("--audio-quality 320k");
            }
            
            // custom user arguments
            command.Add(CustomDownloadArguments.Trim());
            
            return command;
        }

        // CONSTRUCTOR
        public Video()
        {
            Url = string.Empty;
            QueuedUrl = string.Empty;
            Title = "Title";
            Channel = "Channel";
            Duration = TimeSpan.Zero;
            Thumbnail = Utilities.Utilities.CreateBlankImage(4, 4);
            FileDownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            FileExtension = FileExtensionList[0];
            DownloadToggleOptions =
            [
                new DownloadToggleOption("-x", "Convert", true),
                new DownloadToggleOption("--no-mtime", "Do not use last modified date", true),
                new DownloadToggleOption("--embed-metadata", "Embed metadata", false),
                new DownloadToggleOption("--embed-thumbnail", "Embed thumbnail", false)
            ];
            CustomDownloadArguments = string.Empty;
            CustomFileName = "%(title)s.%(ext)s";
        }
    }
}
