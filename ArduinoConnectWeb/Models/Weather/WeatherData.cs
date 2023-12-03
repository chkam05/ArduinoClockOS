﻿using Newtonsoft.Json;
using System;
using System.Globalization;

namespace ArduinoConnectWeb.Models.Weather
{
    public class WeatherData
    {

        //  CONST

        private static readonly string DTFormat = "yyyy-MM-dd";
        private static readonly DateTimeStyles DTFormatStyle = DateTimeStyles.None;
        private static readonly IFormatProvider DTFormatProvider = CultureInfo.InvariantCulture;


        //  VARIABLES

        [JsonProperty("astronomy")]
        public List<WeatherAstronomy>? Astronomy { get; set; }

        [JsonProperty("avgtempC")]
        public string? AvgTempC { get; set; }

        [JsonProperty("avgtempF")]
        public string? AvgTempF { get; set; }

        [JsonProperty("date")]
        public string? Date { get; set; }

        [JsonProperty("hourly")]
        public List<WeatherHourly>? Hourly { get; set; }


        //  METHODS

        public DateTime? GetDateTime()
        {
            if (!string.IsNullOrEmpty(Date) && DateTime.TryParseExact(Date, DTFormat, DTFormatProvider, DTFormatStyle, out DateTime result))
                return result;
            return null;
        }

    }
}