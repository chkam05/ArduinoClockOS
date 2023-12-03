using Newtonsoft.Json;

namespace ArduinoConnectWeb.Models.Weather
{
    public class WeatherHourly
    {

        //  VARIABLES

        [JsonProperty("DewPointC")]
        public string? DewPointC { get; set; }

        [JsonProperty("DewPointF")]
        public string? DewPointF { get; set; }

        [JsonProperty("FeelsLikeC")]
        public string? FeelsLikeC { get; set; }

        [JsonProperty("FeelsLikeF")]
        public string? FeelsLikeF { get; set; }

        [JsonProperty("HeatIndexC")]
        public string? HeatIndexC { get; set; }

        [JsonProperty("HeatIndexF")]
        public string? HeatIndexF { get; set; }

        [JsonProperty("WindChillC")]
        public string? WindChillC { get; set; }

        [JsonProperty("WindChillF")]
        public string? WindChillF { get; set; }

        [JsonProperty("WindGustKmph")]
        public string? WindGustKmph { get; set; }

        [JsonProperty("WindGustMiles")]
        public string? WindGustMiles { get; set; }

        [JsonProperty("chanceoffog")]
        public string? ChanceOfFog { get; set; }

        [JsonProperty("chanceoffrost")]
        public string? ChanceOfFrost { get; set; }

        [JsonProperty("chanceofhightemp")]
        public string? ChanceOfHighTemp { get; set; }

        [JsonProperty("chanceofovercast")]
        public string? ChanceOfOvercast { get; set; }

        [JsonProperty("chanceofrain")]
        public string? ChanceOfRain { get; set; }

        [JsonProperty("chanceofremdry")]
        public string? ChanceOfRemdry { get; set; }

        [JsonProperty("chanceofsnow")]
        public string? ChanceOfSnow { get; set; }

        [JsonProperty("chanceofsunshine")]
        public string? ChanceOfSunshine { get; set; }

        [JsonProperty("chanceofthunder")]
        public string? ChanceOfThunder { get; set; }

        [JsonProperty("chanceofwindy")]
        public string? ChanceOfWindy { get; set; }

        [JsonProperty("cloudcover")]
        public string? CloudCover { get; set; }

        [JsonProperty("humidity")]
        public string? Humidity { get; set; }

        [JsonProperty("precipInches")]
        public string? PrecipInches { get; set; }

        [JsonProperty("precipMM")]
        public string? PrecipMM { get; set; }

        [JsonProperty("pressure")]
        public string? Pressure { get; set; }

        [JsonProperty("pressureInches")]
        public string? PressureInches { get; set; }

        [JsonProperty("tempC")]
        public string? TempC { get; set; }

        [JsonProperty("tempF")]
        public string? TempF { get; set; }

        [JsonProperty("time")]
        public string? Time { get; set; }

        [JsonProperty("uvIndex")]
        public string? UvIndex { get; set; }

        [JsonProperty("visibility")]
        public string? Visibility { get; set; }

        [JsonProperty("visibilityMiles")]
        public string? VisibilityMiles { get; set; }

        [JsonProperty("weatherCode")]
        public string? WeatherCode { get; set; }

        [JsonProperty("weatherDesc")]
        public List<WeatherDesc>? WeatherDesc { get; set; }

        [JsonProperty("weatherIconUrl")]
        public List<WeatherIconUrl>? WeatherIconUrl { get; set; }

        [JsonProperty("winddir16Point")]
        public string? Winddir16Point { get; set; }

        [JsonProperty("winddirDegree")]
        public string? WinddirDegree { get; set; }

        [JsonProperty("windspeedKmph")]
        public string? WindspeedKmph { get; set; }

        [JsonProperty("windspeedMiles")]
        public string? WindspeedMiles { get; set; }


        //  METHODS

        public DateTime? GetTime()
        {
            if (!string.IsNullOrEmpty(Time))
            {
                var hour = Time.Length > 3
                    ? Time.Substring(0, 2)
                    : Time.Length > 2
                        ? Time.Substring(0, 1)
                        : "0";

                if (!string.IsNullOrEmpty(hour) && int.TryParse(hour, out int value))
                    return DateTime.MinValue.AddHours(value);
            }

            return null;
        }

        public int GetWeatherCode()
        {
            if (!string.IsNullOrEmpty(WeatherCode) && int.TryParse(WeatherCode, out int value))
                return value;
            return -1;
        }

    }
}
