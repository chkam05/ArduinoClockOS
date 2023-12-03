using Newtonsoft.Json;

namespace ArduinoConnectWeb.Models.Weather
{
    public class WeatherDataModel
    {

        //  VARIABLES

        [JsonProperty("current_condition")]
        public List<WeatherCurrentCondition>? CurrentCondition { get; set; }

        [JsonProperty("nearest_area")]
        public List<WeatherNearestArea>? NearestArea { get; set; }

        [JsonProperty("request")]
        public List<WeatherRequest>? Request { get; set; }

        [JsonProperty("weather")]
        public List<WeatherData>? Weather { get; set; }

    }
}
