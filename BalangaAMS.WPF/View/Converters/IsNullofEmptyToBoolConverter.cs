using System;
using System.Windows.Data;

namespace BalangaAMS.WPF.View.Converters
{
    class IsNullofEmptyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isNullorEmpty;

            if (value is String)
            {
                var str = value as String;
                isNullorEmpty = string.IsNullOrWhiteSpace(str);
            }

            else if (value is char[]){
                var charArray = value as char[];
                isNullorEmpty = charArray.Length == 0;
            }
            else
            {
                var date = value as DateTime?;
                isNullorEmpty = !date.HasValue;
            }
            return isNullorEmpty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
