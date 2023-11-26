namespace ArduinoConnectWeb.Models.Serial
{
    public static class BaudRates
    {

        //  CONST

        public const int Bps300 = 300;
        public const int Bps1200 = 1200;
        public const int Bps2400 = 2400;
        public const int Bps4800 = 4800;
        public const int Bps9600 = 9600;
        public const int Bps19200 = 19200;
        public const int Bps38400 = 38400;
        public const int Bps57600 = 57600;
        public const int Bps115200 = 115200;


        //  VARIABLES

        public static readonly List<int> BaudRatesList = new List<int>()
        {
            Bps300,
            Bps1200,
            Bps2400,
            Bps4800,
            Bps9600,
            Bps19200,
            Bps38400,
            Bps57600,
            Bps115200
        };


        //  METHODS

        #region VALIDATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if baud rate is correct. </summary>
        /// <param name="baudRate"> Buad rate. </param>
        /// <returns> True - baud rate is correct; False - otherwise. </returns>
        public static bool IsCorrectBaudRate(int baudRate)
        {
            return BaudRatesList.Contains(baudRate);
        }

        #endregion VALIDATION METHODS

    }
}
