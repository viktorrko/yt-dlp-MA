<img src="https://github.com/user-attachments/assets/2c82ce83-578b-4ccf-bb7f-cd883daa0c44" alt="icon" width="200"/>

# yt-dlp-MA

A small utility for downloading audio files from YouTube using yt-dlp.

### Features
+ Downloads the highest available audio track from YouTube.
+ Modern UI created using *Material Design in XAML* Toolkit.
+ Retrieves and displays video metadata.
+ Easy switches for common used options in yt-dlp.
+ User can choose to convert to MP3 (320k), WAV or FLAC.
+ Ability to pass custom arguments for advanced users.
+ Console view for advanced users.
+ One-click FFmpeg install.

### Requirements
+ .NET Runtime 8.0+

### About the FFmpeg install feature
It is recommended that you have FFmpeg installed in your system PATH. Otherwise functions such as converting or embedding metadata won't work.

App will check if FFmpeg is available in PATH enviroment variable on startup. If FFmpeg is not available, you can use the *'Download FFmpeg'* button from the Settings menu. This will download the latest release essentials build from [gyan.dev](https://www.gyan.dev/ffmpeg/builds/) to a `%TEMP%` folder, extract the contents, and moves *ffmpeg.exe* to `/Resources/exe/`, which is added to the program PATH enviroment variable. As *ffmpeg.exe* remains in the program folder, you only have to do this once.

### How to build
+ Clone the GitHub repository.
+ Open *yt-dlp-MA.sln* solution in Visual Studio
+ Place your *yt-dlp.exe* executable into `/Resources/exe/` folder.
+ Build the solution.

### Screenshots
<img src="https://github.com/user-attachments/assets/adc9e204-675c-462a-98b0-2e6b3ee0c36f" alt="screenshots" height="600" />

### How to use
<img src="https://github.com/user-attachments/assets/c5e602c9-847e-4778-b5aa-f498a095d22b" alt="how to use gif" height="600" />

Any suggestions or feedback is welcome :)
