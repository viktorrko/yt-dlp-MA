using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ytdlpMA.Classes
{
    class DownloadItem
    {
        public static readonly string FFmpeg = @"Resources\exe\ffmpeg.exe";
        public static readonly string ytdlp = @"Resources\exe\yt-dlp.exe";

        public string Url { get; set; }
        public string DestinationPath { get; set; }
        public string Extension { get; set; }
        public Dictionary<string, bool> Switches { get; set; }
        public string CustomArguments { get; set; }


        public DownloadItem()
        {
            this.Url = string.Empty;
            this.DestinationPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            this.Extension = string.Empty;
            Switches = new Dictionary<string, bool>
            {
                ["convert"] = true,
                ["mtime"] = false,
                ["thumbnail"] = false,
                ["metadata"] = false
            };
            CustomArguments = string.Empty;
        }

        public List<string> ArgumentsListBuilder()
        {
            List<string> list = [];
            list.Add($"--ffmpeg-location \"{Path.GetFullPath(FFmpeg)}\"");
            list.Add("--newline");
            list.Add($"-i {this.Url}");
            list.Add("-f bestaudio");
            list.Add($"-o \"{Path.Combine(this.DestinationPath, "%(title)s.%(ext)s")}\"");
            

            if (Switches["convert"])
            {
                list.Add("-x");
                list.Add($"--audio-format {Extension}");
                if (Extension.Equals("mp3"))
                    list.Add("--audio-quality 320k");
            }

            if (!Switches["mtime"])
                list.Add("--no-mtime");

            if (Switches["thumbnail"])
                list.Add("--embed-thumbnail");

            if (Switches["metadata"])
                list.Add("--embed-metadata");

            list.Add(CustomArguments.Trim());

            return list;
        }

        public static string SetDestinationPath()
        {
            using (CommonOpenFileDialog dialog = new())
            {
                dialog.IsFolderPicker = true;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileName != null)
                {
                    return dialog.FileName;
                }
                return String.Empty;
            }
        }
    }
}
