using Newtonsoft.Json;
using System.Globalization;

namespace ArduinoConnectWeb.Models.Weather
{
    public class WeatherAstronomy
    {

        //  CONST

        private static readonly string DTFormat = "hh:mm tt";
        private static readonly DateTimeStyles DTFormatStyle = DateTimeStyles.None;
        private static readonly IFormatProvider DTFormatProvider = CultureInfo.InvariantCulture;


        //  VARIABLES

        [JsonProperty("moon_illumination")]
        public string? MoonIllumination { get; set; }

        [JsonProperty("moon_phase")]
        public string? MoonPhase { get; set; }

        [JsonProperty("moonrise")]
        public string? Moonrise { get; set; }

        [JsonProperty("moonset")]
        public string? Moonset { get; set; }

        [JsonProperty("sunrise")]
        public string? Sunrise { get; set; }

        [JsonProperty("sunset")]
        public string? Sunset { get; set; }


        //  METHODS

        public DateTime? GetMoonriseDateTime()
        {
            if (!string.IsNullOrEmpty(Moonrise) && DateTime.TryParseExact(Moonrise, DTFormat, DTFormatProvider, DTFormatStyle, out DateTime result))
                return result;
            return null;
        }

        public DateTime? GetMoonsetDateTime()
        {
            if (!string.IsNullOrEmpty(Moonset) && DateTime.TryParseExact(Moonset, DTFormat, DTFormatProvider, DTFormatStyle, out DateTime result))
                return result;
            return null;
        }

        public DateTime? GetSunriseDateTime()
        {
            if (!string.IsNullOrEmpty(Sunrise) && DateTime.TryParseExact(Sunrise, DTFormat, DTFormatProvider, DTFormatStyle, out DateTime result))
                return result;
            return null;
        }

        public DateTime? GetSunsetDateTime()
        {
            if (!string.IsNullOrEmpty(Sunset) && DateTime.TryParseExact(Sunset, DTFormat, DTFormatProvider, DTFormatStyle, out DateTime result))
                return result;
            return null;
        }

    }
}
