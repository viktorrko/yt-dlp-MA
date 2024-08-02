using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ytdlpMA.Utilities
{
    internal class FFmpegToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
            {
                if (boolean)
                    return Brushes.Green;
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B00020"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
