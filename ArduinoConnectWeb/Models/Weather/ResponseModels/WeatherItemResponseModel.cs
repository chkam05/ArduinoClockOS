using ArduinoConnectWeb.Services.Weather;
using Microsoft.VisualBasic;

namespace ArduinoConnectWeb.Models.Weather.ResponseModels
{
    public class WeatherItemResponseModel
    {

        //  VARIABLES

        public string? Date { get; set; }
        public List<HourlyWeatherItemResponseModel>? HourlyWeather { get; set; }
        public string? Moonrise { get; set; }
        public string? Moonset { get; set; }
        public string? Sunrise { get; set; }
        public string? Sunset { get; set; }
        public string? TemperatureAverage { get; set; }
        public string? TemperatureMax { get; set; }
        public string? TemperatureMin { get; set; }
        public string? TotalSnow { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> WeatherResponseModel class constructor. </summary>
        /// <param name="weatherData"> Weather data. </param>
        public WeatherItemResponseModel(WeatherData? weatherData)
        {
            if (weatherData != null)
            {
                Date = weatherData.GetDateTime()?.ToString(WeatherServiceConfig.DATE_FORMAT);
                HourlyWeather = weatherData.Hourly.Select(w => new HourlyWeatherItemResponseModel(w)).ToList();

                if (weatherData.Astronomy?.Any() ?? false)
                {
                    var astronomy = weatherData.Astronomy[0];

                    Moonrise = astronomy.GetMoonriseDateTime()?.ToString(WeatherServiceConfig.TIME_FORMAT);
                    Moonset = astronomy.GetMoonsetDateTime()?.ToString(WeatherServiceConfig.TIME_FORMAT);
                    Sunrise = astronomy.GetSunriseDateTime()?.ToString(WeatherServiceConfig.TIME_FORMAT);
                    Sunset = astronomy.GetSunsetDateTime()?.ToString(WeatherServiceConfig.TIME_FORMAT);
                }

                TemperatureAverage = string.Format(WeatherServiceConfig.TEMPERATURE_FORMAT, weatherData.AvgTempC);
                TemperatureMax = string.Format(WeatherServiceConfig.TEMPERATURE_FORMAT, weatherData.MaxTempC);
                TemperatureMin = string.Format(WeatherServiceConfig.TEMPERATURE_FORMAT, weatherData.MinTempC);
                TotalSnow = string.Format(WeatherServiceConfig.SNOW_FORMAT, weatherData.TotalSnowCm);
            }
        }

        #endregion CLASS METHODS

    }
}
