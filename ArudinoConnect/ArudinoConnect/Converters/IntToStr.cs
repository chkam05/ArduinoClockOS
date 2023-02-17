using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ArudinoConnect.Converters
{
    public class IntToStr : IValueConverter
    {

        //  METHODS

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(int))
                return ((int)value).ToString();

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(string))
            {
                if (int.TryParse((string)value, out int intValue))
                    return intValue;
            }

            return 0;
        }

    }
}
