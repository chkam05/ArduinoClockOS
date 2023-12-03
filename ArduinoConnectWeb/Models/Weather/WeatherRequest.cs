using Newtonsoft.Json;

namespace ArduinoConnectWeb.Models.Weather
{
    public class WeatherRequest
    {

        //  VARIABLES

        [JsonProperty("query")]
        public string? Query { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

    }
}
