using ArduinoConnectWeb.Mappers;
using ArduinoConnectWeb.Services.Weather;

namespace ArduinoConnectWeb.Models.Weather.ResponseModels
{
    public class HourlyWeatherItemResponseModel
    {

        //  VARIABLES

        public string? Hour { get; set; }
        public string? CloudCover { get; set; }
        public string? Humidity { get; set; }
        public string? Precipitation { get; set; }
        public string? Pressure { get; set; }
        public string? Temperature { get; set; }
        public string? TemperatureDewPoint { get; set; }
        public string? TemperatureFeelsLike { get; set; }
        public string? TemperatureHeatIndex { get; set; }
        public string? TemperatureWindChill { get; set; }
        public int? UvIndex { get; set; }
        public string? UvRisk { get; set; }
        public string? Visibility { get; set; }
        public int? WeatherArduinoCode { get; set; }
        public int? WeatherCode { get; set; }
        public string? WeatherName { get; set; }
        public string? WindDirection { get; set; }
        public string? WindSpeedKmph { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> HourlyWeatherResponseModel class constructor. </summary>
        /// <param name="weatherHourly"> Weather hourly data. </param>
        public HourlyWeatherItemResponseModel(WeatherHourly weatherHourly)
        {
            if (weatherHourly != null)
            {
                int? uvIndex = int.TryParse(weatherHourly.UvIndex, out int uvIndexTemp) ? uvIndexTemp : null;
                int weatherCode = weatherHourly.GetWeatherCode();

                Hour = weatherHourly.GetTime()?.ToString(WeatherServiceConfig.TIME_FORMAT);
                CloudCover = string.Format(WeatherServiceConfig.PERCENT_FORMAT, weatherHourly.CloudCover);
                Humidity = string.Format(WeatherServiceConfig.PERCENT_FORMAT, weatherHourly.Humidity);
                Precipitation = string.Format(WeatherServiceConfig.PRECIPITATION_FORMAT, weatherHourly.PrecipMM);
                Pressure = string.Format(WeatherServiceConfig.PRESSURE_FORMAT, weatherHourly.Pressure);
                Temperature = string.Format(WeatherServiceConfig.TEMPERATURE_FORMAT, weatherHourly.TempC);
                TemperatureDewPoint = string.Format(WeatherServiceConfig.TEMPERATURE_FORMAT, weatherHourly.DewPointC);
                TemperatureFeelsLike = string.Format(WeatherServiceConfig.TEMPERATURE_FORMAT, weatherHourly.FeelsLikeC);
                TemperatureHeatIndex = string.Format(WeatherServiceConfig.TEMPERATURE_FORMAT, weatherHourly.HeatIndexC);
                TemperatureWindChill = string.Format(WeatherServiceConfig.TEMPERATURE_FORMAT, weatherHourly.WindChillC);
                UvIndex = uvIndex;
                UvRisk = uvIndex.HasValue ? WeatherMapper.MapUvIndexToRiskName(uvIndex.Value) : null;
                Visibility = string.Format(WeatherServiceConfig.VISIBILITY_FORMAT, weatherHourly.Visibility);
                WeatherArduinoCode = WeatherMapper.MapWeatherCodeToArduinoCode(weatherCode);
                WeatherCode = weatherCode;
                WeatherName = WeatherMapper.MapWeatherCodeToName(weatherCode);
                WindDirection = string.Format(WeatherServiceConfig.WIND_DIRECTION_FORMAT, weatherHourly.WinddirDegree, weatherHourly.Winddir16Point);
                WindSpeedKmph = string.Format(WeatherServiceConfig.WIND_SPEED_FORMAT, weatherHourly.WindspeedKmph);
            }
        }

        #endregion CLASS METHODS

    }
}
