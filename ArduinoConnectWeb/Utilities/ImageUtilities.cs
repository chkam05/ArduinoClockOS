using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using static System.Net.Mime.MediaTypeNames;

namespace ArduinoConnectWeb.Utilities
{
    [SupportedOSPlatform("windows")]
    public static class ImageUtilities
    {

        //  VARIABLES

        private const string FONT_FAMILY_NAME = "Sans Serif";
        private const float FONT_SIZE = 15f;


        //  METHODS

        #region TEXT TO IMAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Generate image from string. </summary>
        /// <param name="text"> String text. </param>
        /// <param name="foregroundColorCode"> Foreground hexadecimal color code. </param>
        /// <param name="backgroundColorCode"> Background hexadecimal color code. </param>
        /// <param name="fontFamilyName"> Font family name. </param>
        /// <param name="fontSize"> Font size. </param>
        /// <returns> Image as stream byte array. </returns>
        public static byte[] GenerateImageFromString(string text,
            string? foregroundColorCode = "#FF000000",
            string? backgroundColorCode = "#00FFFFFF",
            string? fontFamilyName = FONT_FAMILY_NAME,
            float? fontSize = FONT_SIZE)
        {
            var backColor = ColorUtilities.ColorFromHexCode(backgroundColorCode, Color.Transparent);
            var textColor = ColorUtilities.ColorFromHexCode(foregroundColorCode, Color.Black);
            var font = new Font(fontFamilyName ?? FONT_FAMILY_NAME, fontSize ?? FONT_SIZE);
            var textSize = MeasureString(text, font);

            int xShift = 5;
            int yShift = 5;

            MemoryStream memoryStream = new MemoryStream();

            using (var bitmap = new Bitmap((int)textSize.Width + (xShift * 2), (int)textSize.Height + (yShift * 2)))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(backColor);

                    using (var textBrush = new SolidBrush(textColor))
                    {
                        graphics.DrawString(text, font, textBrush, xShift, yShift);
                        graphics.Save();
                    }
                }

                bitmap.Save(memoryStream, ImageFormat.Png);
            }

            return memoryStream.ToArray();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Measure text size. </summary>
        /// <param name="text"> String text. </param>
        /// <param name="font"> Text font. </param>
        /// <returns> Text size. </returns>
        private static SizeF MeasureString(string text, Font font)
        {
            SizeF textSize = new SizeF();

            using (var bitmap = new Bitmap(1, 1))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    textSize = graphics.MeasureString(text, font);
                }
            }

            return textSize;
        }

        #endregion TEXT TO IMAGE METHODS

    }
}
