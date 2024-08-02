using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace ytdlpMA.Utilities
{
    internal class ConvertFormatIsEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<DownloadToggleOption> downloadToggleOptions)
            {
                foreach (var downloadToggleOption in downloadToggleOptions)
                {
                    if (downloadToggleOption.DisplayOption == "Convert" && downloadToggleOption.Value)
                        return true;
                }                
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
