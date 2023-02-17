using chkam05.Tools.ControlsEx.Colors;
using chkam05.Tools.ControlsEx.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ArudinoConnect.Converters
{
    public class ButtonInteractionColorConverter : IValueConverter
    {

        //  METHODS

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(Color) && parameter.GetType() == typeof(int))
            {
                AHSLColor baseColor = AHSLColor.FromColor((Color)value);
                int lightness = (int)parameter;
                int saturation = baseColor.L == 0 ? 0 : baseColor.S;

                return new SolidColorBrush(ColorsUtilities.UpdateColor(baseColor, saturation: saturation, lightness: lightness).ToColor());
            }
            
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
