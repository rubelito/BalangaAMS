using System;
using System.Windows.Data;

namespace BalangaAMS.WPF.View.Converters
{
    public class ReverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var boolean = value as bool?;
            return !boolean;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var boolean = value as bool?;
            return !boolean;
        }
    }
}
