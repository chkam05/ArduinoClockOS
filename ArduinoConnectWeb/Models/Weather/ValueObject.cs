using Newtonsoft.Json;

namespace ArduinoConnectWeb.Models.Weather
{
    public class ValueObject
    {

        //  VARIABLES

        [JsonProperty("value")]
        public string? Value { get; set; }

    }
}
