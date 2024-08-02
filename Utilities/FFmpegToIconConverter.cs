using System.Globalization;
using System.Windows.Data;

namespace ytdlpMA.Utilities
{
    internal class FFmpegToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
            {
                if (boolean)
                    return MaterialDesignThemes.Wpf.PackIconKind.Check;
            }
            return MaterialDesignThemes.Wpf.PackIconKind.Close;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
