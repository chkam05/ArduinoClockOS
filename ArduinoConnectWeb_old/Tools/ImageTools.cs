using System.Drawing;

namespace ArduinoConnectWeb.Tools
{
    public static class ImageTools
    {

        //  METHODS

        #region TEXT TO IMAGE METHODS

        //  --------------------------------------------------------------------------------
        public static byte[] GenerateImage(string text)
        {
            var backColor = Color.Cornsilk;
            var textColor = Color.DarkBlue;
            var font = new Font("Sans Serif", 15);
            var textSize = MeasureString(text, font);

            var image = new Bitmap((int)textSize.Width, (int)textSize.Height);
            var drawing = Graphics.FromImage(image);

            drawing.Clear(backColor);
            Brush textBrush = new SolidBrush(textColor);
            drawing.DrawString(text, font, textBrush, 0, 0);
            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            return memoryStream.ToArray();
        }

        //  --------------------------------------------------------------------------------
        private static SizeF MeasureString(string text, Font font)
        {
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            SizeF textSize = drawing.MeasureString(text, font);
            img.Dispose();
            drawing.Dispose();

            return textSize;
        }

        #endregion TEXT TO IMAGE METHODS

    }
}
