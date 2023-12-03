using Newtonsoft.Json;

namespace ArduinoConnectWeb.Models.Weather
{
    public class WeatherCurrentCondition
    {

        //  VARIABLES

        [JsonProperty("FeelsLikeC")]
        public string? FeelsLikeC { get; set; }

        [JsonProperty("FeelsLikeF")]
        public string? FeelsLikeF { get; set; }

        [JsonProperty("cloudcover")]
        public string? CloudCover { get; set; }

        [JsonProperty("humidity")]
        public string? Humidity { get; set; }

        [JsonProperty("localObsDateTime")]
        public string? LocalObsDateTime { get; set; }

        [JsonProperty("observation_time")]
        public string? ObservationTime { get; set; }

        [JsonProperty("precipInches")]
        public string? PrecipInches { get; set; }

        [JsonProperty("PrecipMM")]
        public string? precipMM { get; set; }

        [JsonProperty("pressure")]
        public string? Pressure { get; set; }

        [JsonProperty("pressureInches")]
        public string? PressureInches { get; set; }

        [JsonProperty("temp_C")]
        public string? TempC { get; set; }

        [JsonProperty("temp_F")]
        public string? TempF { get; set; }

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

    }
}
