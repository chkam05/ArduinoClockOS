namespace ArduinoConnectWeb.Mappers
{
    public static class WeatherMapper
    {

        //  CONST

        public static string UV_INDEX_LOW_RISK = "Low Risk";
        public static string UV_INDEX_MODERATE_RISK = "Moderate Risk";
        public static string UV_INDEX_RISK = "Risk";
        public static string UV_INDEX_DANGEROUS = "Dangerous";
        public static string UV_INDEX_EXTREME_DANGER = "Extreme Danger";

        public static string WEATHER_NAME_SUNNY = "Sunny";                              //  Słonecznie
        public static string WEATHER_NAME_PARTLY_CLOUDY = "Partly Cloudy";              //  Częściowe zachmurzenie
        public static string WEATHER_NAME_CLOUDY = "Cloudy";                            //  Pochmurnie
        public static string WEATHER_NAME_VERY_CLOUDY = "Very Cloudy";                  //  Bardzo pochmurno
        public static string WEATHER_NAME_FOG = "Fog";                                  //  Mgła
        public static string WEATHER_LIGHT_SHOWER = "Light Shower";                     //  Lekka mżawka
        public static string WEATHER_LIGHT_SLEET_SHOWER = "Light Sleet Shower";         //  Lekka mżawka z śniegiem
        public static string WEATHER_LIGHT_SLEET = "Light Sleet";                       //  Lekki deszcz z śniegiem
        public static string WEATHER_THUNDERY_SHOWER = "Thundery Shower";               //  Burza i mżawka
        public static string WEATHER_LIGHT_SNOW = "Light Snow";                         //  Lekki śnieg
        public static string WEATHER_HEAVY_SNOW = "Heavy Snow";                         //  Mocny śnieg
        public static string WEATHER_LIGHT_RAIN = "Light Rain";                         //  Lekki deszcz
        public static string WEATHER_HEAVY_SHOWER = "Heavy Shower";                     //  Mocna mżawka
        public static string WEATHER_HEAVY_RAIN = "Heavy Rain";                         //  Mocny deszcz
        public static string WEATHER_LIGHT_SNOW_SHOWER = "Light Snow Shower";           //  Lekkie pruszenie śniegiem
        public static string WEATHER_HEAVY_SNOW_SHOWER = "Heavy Snow Shower";           //  Mocny pruszenie śniegiem
        public static string WEATHER_THUNDERY_HEAVY_RAIN = "Thundery Heavy Rain";       //  Burza i mocny deszcz
        public static string WEATHER_THUNDERY_SNOW_SHOWER = "Thundery Snow Shower";     //  Burza z śniegiem

        public static int ARDUINO_SUNNY = 0;
        public static int ARDUINO_PART_CLOUDY = 1;
        public static int ARDUINO_CLOUDY = 2;
        public static int ARDUINO_RAIN = 3;
        public static int ARDUINO_SNOW = 4;
        public static int ARDUINO_FOG = 5;
        public static int ARDUINO_THUNDER = 6;


        //  MAPPING DICTIONARIES

        private static Dictionary<int, string> _uvIndexMap = new Dictionary<int, string>()
        {
            { 0, UV_INDEX_LOW_RISK },
            { 1, UV_INDEX_LOW_RISK },
            { 2, UV_INDEX_LOW_RISK },
            { 3, UV_INDEX_MODERATE_RISK },
            { 4, UV_INDEX_MODERATE_RISK },
            { 5, UV_INDEX_MODERATE_RISK },
            { 6, UV_INDEX_RISK },
            { 7, UV_INDEX_RISK },
            { 8, UV_INDEX_DANGEROUS },
            { 9, UV_INDEX_DANGEROUS },
            { 10, UV_INDEX_DANGEROUS },
            { 11, UV_INDEX_EXTREME_DANGER },
        };

        private static Dictionary<string, int> _weatherArduinoCodeMap = new Dictionary<string, int>()
        {
            { WEATHER_NAME_SUNNY, ARDUINO_SUNNY },
            { WEATHER_NAME_PARTLY_CLOUDY, ARDUINO_PART_CLOUDY },
            { WEATHER_NAME_CLOUDY, ARDUINO_CLOUDY },
            { WEATHER_NAME_VERY_CLOUDY, ARDUINO_CLOUDY },
            { WEATHER_NAME_FOG, ARDUINO_FOG },
            { WEATHER_LIGHT_SHOWER, ARDUINO_RAIN },
            { WEATHER_LIGHT_SLEET_SHOWER, ARDUINO_RAIN },
            { WEATHER_LIGHT_SLEET, ARDUINO_RAIN },
            { WEATHER_THUNDERY_SHOWER, ARDUINO_THUNDER },
            { WEATHER_LIGHT_SNOW, ARDUINO_SNOW },
            { WEATHER_HEAVY_SNOW, ARDUINO_SNOW },
            { WEATHER_LIGHT_RAIN, ARDUINO_RAIN },
            { WEATHER_HEAVY_SHOWER, ARDUINO_RAIN},
            { WEATHER_HEAVY_RAIN, ARDUINO_RAIN },
            { WEATHER_LIGHT_SNOW_SHOWER, ARDUINO_SNOW },
            { WEATHER_HEAVY_SNOW_SHOWER, ARDUINO_SNOW },
            { WEATHER_THUNDERY_HEAVY_RAIN, ARDUINO_THUNDER },
            { WEATHER_THUNDERY_SNOW_SHOWER, ARDUINO_THUNDER },
        };

        private static Dictionary<int, string> _weatherNameMap = new Dictionary<int, string>()
        {
            { 113, WEATHER_NAME_SUNNY },
            { 116, WEATHER_NAME_PARTLY_CLOUDY },
            { 119, WEATHER_NAME_CLOUDY },
            { 122, WEATHER_NAME_VERY_CLOUDY },
            { 143, WEATHER_NAME_FOG },
            { 176, WEATHER_LIGHT_SHOWER },
            { 179, WEATHER_LIGHT_SLEET_SHOWER },
            { 182, WEATHER_LIGHT_SLEET },
            { 185, WEATHER_LIGHT_SLEET },
            { 200, WEATHER_THUNDERY_SHOWER },
            { 227, WEATHER_LIGHT_SNOW },
            { 230, WEATHER_HEAVY_SNOW },
            { 248, WEATHER_NAME_FOG },
            { 260, WEATHER_NAME_FOG },
            { 263, WEATHER_LIGHT_SHOWER },
            { 266, WEATHER_LIGHT_RAIN },
            { 281, WEATHER_LIGHT_SLEET },
            { 284, WEATHER_LIGHT_SLEET },
            { 293, WEATHER_LIGHT_RAIN },
            { 296, WEATHER_LIGHT_RAIN },
            { 299, WEATHER_HEAVY_SHOWER },
            { 302, WEATHER_HEAVY_RAIN },
            { 305, WEATHER_HEAVY_SHOWER },
            { 308, WEATHER_HEAVY_RAIN },
            { 311, WEATHER_LIGHT_SLEET },
            { 314, WEATHER_LIGHT_SLEET },
            { 317, WEATHER_LIGHT_SLEET },
            { 320, WEATHER_LIGHT_SNOW },
            { 323, WEATHER_LIGHT_SNOW_SHOWER },
            { 326, WEATHER_LIGHT_SNOW_SHOWER },
            { 329, WEATHER_HEAVY_SNOW },
            { 332, WEATHER_HEAVY_SNOW },
            { 335, WEATHER_HEAVY_SNOW_SHOWER },
            { 338, WEATHER_HEAVY_SNOW },
            { 350, WEATHER_LIGHT_SLEET },
            { 353, WEATHER_LIGHT_SHOWER },
            { 356, WEATHER_HEAVY_SHOWER },
            { 359, WEATHER_HEAVY_RAIN },
            { 362, WEATHER_LIGHT_SLEET_SHOWER },
            { 365, WEATHER_LIGHT_SLEET_SHOWER },
            { 368, WEATHER_LIGHT_SNOW_SHOWER },
            { 371, WEATHER_HEAVY_SNOW_SHOWER },
            { 374, WEATHER_LIGHT_SLEET_SHOWER },
            { 377, WEATHER_LIGHT_SLEET },
            { 386, WEATHER_THUNDERY_SHOWER },
            { 389, WEATHER_THUNDERY_HEAVY_RAIN },
            { 392, WEATHER_THUNDERY_SNOW_SHOWER },
            { 395, WEATHER_HEAVY_SNOW_SHOWER },
        };


        //  METHODS

        #region MAPPING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Map UV index to risk name. </summary>
        /// <param name="uvIndex"> UV index. </param>
        /// <returns> Risk name. </returns>
        public static string MapUvIndexToRiskName(int uvIndex)
        {
            if (uvIndex < 0)
                return UV_INDEX_LOW_RISK;

            if (uvIndex > 11)
                return UV_INDEX_EXTREME_DANGER;

            return _uvIndexMap[uvIndex];
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Map weather code to name. </summary>
        /// <param name="weatherCode"> Weather code. </param>
        /// <returns> Weather name. </returns>
        public static string MapWeatherCodeToName(int weatherCode)
        {
            return _weatherNameMap.TryGetValue(weatherCode, out string? value)
                ? value : WEATHER_NAME_SUNNY;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Map weather code to arduino code. </summary>
        /// <param name="weatherCode"> Weather code. </param>
        /// <returns> Arduino code. </returns>
        public static int MapWeatherCodeToArduinoCode(int weatherCode)
        {
            string name = MapWeatherCodeToName(weatherCode);
            return _weatherArduinoCodeMap.TryGetValue(name, out int value)
                ? value : ARDUINO_SUNNY;
        }

        #endregion MAPPING METHODS

    }
}
