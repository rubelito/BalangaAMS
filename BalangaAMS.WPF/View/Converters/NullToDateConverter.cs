using System;
using System.Windows.Data;

namespace BalangaAMS.WPF.View.Converters
{
    public class NullToDateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var date = value as DateTime?;
            if (date != null)
                return date;
            return DateTime.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var date = value as DateTime?;
            return date;

        }
    }
}
