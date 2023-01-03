using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArudinoConnect.Mappers
{
    public static class WeatherDataMappers
    {
                                                                                        //  PL:
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

        private static Dictionary<string, int> _weatherArduinoCodeMap = new Dictionary<string, int>()
        {
            //
        };

        private static Dictionary<string, PackIconKind> _weatherIconMap = new Dictionary<string, PackIconKind>()
        {
            { WEATHER_NAME_SUNNY, PackIconKind.WeatherSunny },
            { WEATHER_NAME_PARTLY_CLOUDY, PackIconKind.WeatherPartlyCloudy },
            { WEATHER_NAME_CLOUDY, PackIconKind.WeatherCloudy },
            { WEATHER_NAME_VERY_CLOUDY, PackIconKind.WeatherCloudy },
            { WEATHER_NAME_FOG, PackIconKind.WeatherFog },
            { WEATHER_LIGHT_SHOWER, PackIconKind.WeatherPartlyRainy },
            { WEATHER_LIGHT_SLEET_SHOWER, PackIconKind.WeatherPartlySnowyRainy },
            { WEATHER_LIGHT_SLEET, PackIconKind.WeatherPartlySnowyRainy },
            { WEATHER_THUNDERY_SHOWER, PackIconKind.WeatherPartlyLightning },
            { WEATHER_LIGHT_SNOW, PackIconKind.WeatherSnowy },
            { WEATHER_HEAVY_SNOW, PackIconKind.WeatherSnowyHeavy },
            { WEATHER_LIGHT_RAIN, PackIconKind.WeatherRainy },
            { WEATHER_HEAVY_SHOWER, PackIconKind.WeatherPartlyRainy },
            { WEATHER_HEAVY_RAIN, PackIconKind.WeatherPouring },
            { WEATHER_LIGHT_SNOW_SHOWER, PackIconKind.WeatherPartlySnowy },
            { WEATHER_HEAVY_SNOW_SHOWER, PackIconKind.WeatherSnowyHeavy },
            { WEATHER_THUNDERY_HEAVY_RAIN, PackIconKind.WeatherThunderRainy },
            { WEATHER_THUNDERY_SNOW_SHOWER, PackIconKind.WeatherLightningRainy },
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

        public static int MapWeatherCodeToArduinoCode(int weatherCode)
        {
            return 0;
        }

        public static string MapWeatherCodeToName(int weatherCode)
        {
            return _weatherNameMap.TryGetValue(weatherCode, out string value)
                ? value : WEATHER_NAME_SUNNY;
        }

        public static PackIconKind MapWeatherCodeToPackIconKind(int weatherCode)
        {
            return _weatherIconMap.TryGetValue(MapWeatherCodeToName(weatherCode), out PackIconKind value)
                ? value : PackIconKind.WeatherSunny;
        }

    }
}
