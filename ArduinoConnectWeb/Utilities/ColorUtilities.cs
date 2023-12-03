using System.Drawing;

namespace ArduinoConnectWeb.Utilities
{
    public static class ColorUtilities
    {

        //  CONST

        public static readonly Color DEFAULT_COLOR = Color.Transparent;


        //  METHODS

        #region COLOR CONVERTERS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert hexadecimal color code to System.Drawing.Color. </summary>
        /// <param name="hexColorCode"> Hexadecimal color code. </param>
        /// <param name="defaultColor"> Default color if hexadecimal code code is not valid. </param>
        /// <returns> System.Drawing.Color. </returns>
        public static Color ColorFromHexCode(string? hexColorCode, Color? defaultColor = null)
        {
            if (string.IsNullOrEmpty(hexColorCode))
                return GetColorOrDefault(defaultColor);

            hexColorCode = hexColorCode.StartsWith("#") ? hexColorCode.Substring(1) : hexColorCode;
            bool hasAlpha = hexColorCode.Length == 8;

            try
            {
                int argb = Convert.ToInt32(hexColorCode, 16);
                return hasAlpha ? Color.FromArgb(argb) : Color.FromArgb(255, Color.FromArgb(argb));
            }
            catch (Exception)
            {
                return GetColorOrDefault(defaultColor);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Convert System.Drawing.Color to hexadecimal color code. </summary>
        /// <param name="color"> System.Drawing.Color. </param>
        /// <param name="includeAlpha"> Include alpha value. </param>
        /// <param name="useHashBefore"> Use "#" character before color code. </param>
        /// <returns> Hexadecimal color code. </returns>
        public static string HexCodeFromColor(Color color, bool includeAlpha = true, bool useHashBefore = true)
        {
            string alpha = includeAlpha ? $"{color.A:X2}" : string.Empty;
            string colorCode = $"{alpha}{color.R:X2}{color.G:X2}{color.B:X2}";
            return useHashBefore ? $"#{colorCode}" : colorCode;
        }

        #endregion COLOR CONVERTERS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get color or default if color has not value. </summary>
        /// <param name="color"> Nullable System.Drawing.Color. </param>
        /// <returns> System.Drawing.Color. </returns>
        private static Color GetColorOrDefault(Color? color)
        {
            return color.HasValue ? color.Value : DEFAULT_COLOR;
        }

        #endregion UTILITY METHODS

    }
}
