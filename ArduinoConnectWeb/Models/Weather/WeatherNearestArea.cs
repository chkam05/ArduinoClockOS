using Newtonsoft.Json;

namespace ArduinoConnectWeb.Models.Weather
{
    public class WeatherNearestArea
    {

        //  VARIABLES

        [JsonProperty("areaName")]
        public List<WeatherAreaName>? AreaName { get; set; }

        [JsonProperty("country")]
        public List<WeatherCountry>? Country { get; set; }

        [JsonProperty("latitude")]
        public string? Latitude { get; set; }

        [JsonProperty("longitude")]
        public string? Longitude { get; set; }

        [JsonProperty("population")]
        public string? Population { get; set; }

        [JsonProperty("region")]
        public List<WeatherRegion>? Region { get; set; }

        [JsonProperty("weatherUrl")]
        public List<WeatherUrl>? WeatherUrl { get; set; }

    }
}
