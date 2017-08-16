using System;
using System.Windows.Data;
using BalangaAMS.Core.Domain.Enum;

namespace BalangaAMS.WPF.View.Schedule
{
    public class StringToEnum : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var strvalue = value as string;
            
            return (Gatherings) Enum.Parse(typeof (Gatherings), strvalue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
