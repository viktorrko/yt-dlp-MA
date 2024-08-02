namespace ytdlpMA.Utilities
{
    // class that hold information for the individual filter chips
    public class DownloadToggleOption(string option, string displayOption, bool value)
    {
        // argument that will get passed to the final yt-dlp command
        public string Option { get; } = option;

        // string that will show in UI
        public string DisplayOption { get; } = displayOption;

        // hold toggle value (if false, 'Option' wont be added to the final argument list)
        public bool Value { get; set; } = value;
    }
}
