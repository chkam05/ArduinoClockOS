using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArudinoConnect.Data
{

    //  MAIN CLASSES

    public class WeatherData
    {
        [JsonProperty("current_condition")]
        public List<CurrentCondition> CurrentCondition { get; set; }

        [JsonProperty("nearest_area")]
        public List<NearestArea> NearestArea { get; set; }

        [JsonProperty("request")]
        public List<Request> Request { get; set; }

        [JsonProperty("weather")]
        public List<Weather> Weather { get; set; }


        //  METHODS

        public WeatherTreeItem ToWeatherTreeItem()
        {
            var result = new WeatherTreeItem(this.GetType().Name);
            var currentCondition = new WeatherTreeItem(nameof(CurrentCondition));
            var nearestArea = new WeatherTreeItem(nameof(NearestArea));
            var request = new WeatherTreeItem(nameof(Request));
            var weather = new WeatherTreeItem(nameof(Weather));

            for (int i = 0; i < CurrentCondition.Count; i++)
                currentCondition.Subitems.Add(CurrentCondition[i].ToWeatherTreeItem($"[{i}]"));

            for (int i = 0; i < NearestArea.Count; i++)
                nearestArea.Subitems.Add(NearestArea[i].ToWeatherTreeItem($"[{i}]"));

            for (int i = 0; i < Request.Count; i++)
                request.Subitems.Add(Request[i].ToWeatherTreeItem($"[{i}]"));

            for (int i = 0; i < Weather.Count; i++)
                weather.Subitems.Add(Weather[i].ToWeatherTreeItem($"[{i}]"));

            result.Subitems.Add(currentCondition);
            result.Subitems.Add(nearestArea);
            result.Subitems.Add(request);
            result.Subitems.Add(weather);

            return result;
        }
    }

    public class CurrentCondition
    {
        [JsonProperty("FeelsLikeC")]
        public string FeelsLikeC { get; set; }

        [JsonProperty("FeelsLikeF")]
        public string FeelsLikeF { get; set; }

        [JsonProperty("cloudcover")]
        public string CloudCover { get; set; }

        [JsonProperty("humidity")]
        public string Humidity { get; set; }
        
        [JsonProperty("localObsDateTime")]
        public string LocalObsDateTime { get; set; }

        [JsonProperty("observation_time")]
        public string ObservationTime { get; set; }

        [JsonProperty("precipInches")]
        public string PrecipInches { get; set; }

        [JsonProperty("PrecipMM")]
        public string precipMM { get; set; }

        [JsonProperty("pressure")]
        public string Pressure { get; set; }

        [JsonProperty("pressureInches")]
        public string PressureInches { get; set; }

        [JsonProperty("temp_C")]
        public string TempC { get; set; }

        [JsonProperty("temp_F")]
        public string TempF { get; set; }

        [JsonProperty("uvIndex")]
        public string UvIndex { get; set; }

        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("visibilityMiles")]
        public string VisibilityMiles { get; set; }

        [JsonProperty("weatherCode")]
        public string WeatherCode { get; set; }

        [JsonProperty("weatherDesc")]
        public List<WeatherDesc> WeatherDesc { get; set; }

        [JsonProperty("weatherIconUrl")]
        public List<WeatherIconUrl> WeatherIconUrl { get; set; }

        [JsonProperty("winddir16Point")]
        public string Winddir16Point { get; set; }

        [JsonProperty("winddirDegree")]
        public string WinddirDegree { get; set; }

        [JsonProperty("windspeedKmph")]
        public string WindspeedKmph { get; set; }

        [JsonProperty("windspeedMiles")]
        public string WindspeedMiles { get; set; }


        //  METHODS

        public WeatherTreeItem ToWeatherTreeItem(string customHeader = null)
        {
            string header = !string.IsNullOrEmpty(customHeader) ? customHeader : this.GetType().Name;

            var result = new WeatherTreeItem(header);
            var weatherDesc = new WeatherTreeItem(nameof(WeatherDesc));
            var weatherIconUrl = new WeatherTreeItem(nameof(WeatherIconUrl));

            for (int i = 0; i < WeatherDesc.Count; i++)
                weatherDesc.Subitems.Add(WeatherDesc[i].ToWeatherTreeItem($"[{i}]"));

            for (int i = 0; i < WeatherIconUrl.Count; i++)
                weatherIconUrl.Subitems.Add(WeatherIconUrl[i].ToWeatherTreeItem($"[{i}]"));

            result.Subitems.Add(new WeatherTreeItem(nameof(FeelsLikeC), FeelsLikeC));
            result.Subitems.Add(new WeatherTreeItem(nameof(FeelsLikeF), FeelsLikeF));
            result.Subitems.Add(new WeatherTreeItem(nameof(CloudCover), CloudCover));
            result.Subitems.Add(new WeatherTreeItem(nameof(Humidity), Humidity));
            result.Subitems.Add(new WeatherTreeItem(nameof(LocalObsDateTime), LocalObsDateTime));
            result.Subitems.Add(new WeatherTreeItem(nameof(ObservationTime), ObservationTime));
            result.Subitems.Add(new WeatherTreeItem(nameof(PrecipInches), PrecipInches));
            result.Subitems.Add(new WeatherTreeItem(nameof(precipMM), precipMM));
            result.Subitems.Add(new WeatherTreeItem(nameof(Pressure), Pressure));
            result.Subitems.Add(new WeatherTreeItem(nameof(PressureInches), PressureInches));
            result.Subitems.Add(new WeatherTreeItem(nameof(TempC), TempC));
            result.Subitems.Add(new WeatherTreeItem(nameof(TempF), TempF));
            result.Subitems.Add(new WeatherTreeItem(nameof(UvIndex), UvIndex));
            result.Subitems.Add(new WeatherTreeItem(nameof(Visibility), Visibility));
            result.Subitems.Add(new WeatherTreeItem(nameof(VisibilityMiles), VisibilityMiles));
            result.Subitems.Add(new WeatherTreeItem(nameof(WeatherCode), WeatherCode));
            result.Subitems.Add(weatherDesc);
            result.Subitems.Add(weatherIconUrl);
            result.Subitems.Add(new WeatherTreeItem(nameof(Winddir16Point), Winddir16Point));
            result.Subitems.Add(new WeatherTreeItem(nameof(WinddirDegree), WinddirDegree));
            result.Subitems.Add(new WeatherTreeItem(nameof(WindspeedKmph), WindspeedKmph));
            result.Subitems.Add(new WeatherTreeItem(nameof(WindspeedMiles), WindspeedMiles));

            return result;
        }
    }

    public class NearestArea
    {
        [JsonProperty("areaName")]
        public List<AreaName> AreaName { get; set; }

        [JsonProperty("country")]
        public List<Country> Country { get; set; }
        
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        
        [JsonProperty("population")]
        public string Population { get; set; }
        
        [JsonProperty("region")]
        public List<Region> Region { get; set; }
        
        [JsonProperty("weatherUrl")]
        public List<WeatherUrl> WeatherUrl { get; set; }


        //  METHODS

        public WeatherTreeItem ToWeatherTreeItem(string customHeader = null)
        {
            string header = !string.IsNullOrEmpty(customHeader) ? customHeader : this.GetType().Name;

            var result = new WeatherTreeItem(header);
            var areaName = new WeatherTreeItem(nameof(AreaName));
            var country = new WeatherTreeItem(nameof(Country));
            var region = new WeatherTreeItem(nameof(Region));
            var weatherUrl = new WeatherTreeItem(nameof(WeatherUrl));

            for (int i = 0; i < AreaName.Count; i++)
                areaName.Subitems.Add(AreaName[i].ToWeatherTreeItem($"[{i}]"));

            for (int i = 0; i < Country.Count; i++)
                country.Subitems.Add(Country[i].ToWeatherTreeItem($"[{i}]"));

            for (int i = 0; i < Region.Count; i++)
                region.Subitems.Add(Region[i].ToWeatherTreeItem($"[{i}]"));

            for (int i = 0; i < WeatherUrl.Count; i++)
                weatherUrl.Subitems.Add(WeatherUrl[i].ToWeatherTreeItem($"[{i}]"));

            result.Subitems.Add(areaName);
            result.Subitems.Add(country);
            result.Subitems.Add(new WeatherTreeItem(nameof(Latitude), Latitude));
            result.Subitems.Add(new WeatherTreeItem(nameof(Longitude), Longitude));
            result.Subitems.Add(new WeatherTreeItem(nameof(Population), Population));
            result.Subitems.Add(region);
            weatherUrl.Subitems.Add(region);

            return result;
        }
    }

    public class Request
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }


        //  METHODS

        public WeatherTreeItem ToWeatherTreeItem(string customHeader = null)
        {
            string header = !string.IsNullOrEmpty(customHeader) ? customHeader : this.GetType().Name;

            var result = new WeatherTreeItem(header);

            result.Subitems.Add(new WeatherTreeItem(nameof(Query), Query));
            result.Subitems.Add(new WeatherTreeItem(nameof(Type), Type));

            return result;
        }
    }

    public class Weather
    {

        //  CONST

        private static readonly string DTFormat = "yyyy-MM-dd";
        private static readonly DateTimeStyles DTFormatStyle = DateTimeStyles.None;
        private static readonly IFormatProvider DTFormatProvider = CultureInfo.InvariantCulture;


        //  VARIABLES

        [JsonProperty("astronomy")]
        public List<Astronomy> Astronomy { get; set; }

        [JsonProperty("avgtempC")]
        public string AvgTempC { get; set; }
        
        [JsonProperty("avgtempF")]
        public string AvgTempF { get; set; }
        
        [JsonProperty("date")]
        public string Date { get; set; }
        
        [JsonProperty("hourly")]
        public List<HourlyWeather> Hourly { get; set; }


        //  METHODS

        public DateTime? GetDateTime()
        {
            if (!string.IsNullOrEmpty(Date) && DateTime.TryParseExact(Date, DTFormat, DTFormatProvider, DTFormatStyle, out DateTime result))
                return result;
            return null;
        }

        public WeatherTreeItem ToWeatherTreeItem(string customHeader = null)
        {
            string header = !string.IsNullOrEmpty(customHeader) ? customHeader : this.GetType().Name;

            var result = new WeatherTreeItem(header);
            var astronomy = new WeatherTreeItem(nameof(Astronomy));
            var hourly = new WeatherTreeItem(nameof(Hourly));

            for (int i = 0; i < Astronomy.Count; i++)
                astronomy.Subitems.Add(Astronomy[i].ToWeatherTreeItem($"[{i}]"));

            for (int i = 0; i < Hourly.Count; i++)
                hourly.Subitems.Add(Hourly[i].ToWeatherTreeItem($"[{i}]"));

            result.Subitems.Add(astronomy);
            result.Subitems.Add(new WeatherTreeItem(nameof(AvgTempC), AvgTempC));
            result.Subitems.Add(new WeatherTreeItem(nameof(AvgTempF), AvgTempF));
            result.Subitems.Add(new WeatherTreeItem(nameof(Date), Date));
            result.Subitems.Add(hourly);

            return result;
        }
    }


    //  COMPONENT CLASSES

    public class ValueObject
    {
        [JsonProperty("value")]
        public string Value { get; set; }


        //  METHODS

        public WeatherTreeItem ToWeatherTreeItem(string customHeader = null)
        {
            string header = !string.IsNullOrEmpty(customHeader) ? customHeader : "Value";
            return new WeatherTreeItem(header, Value);
        }
    }

    public class AreaName : ValueObject { }

    public class Astronomy
    {

        //  CONST

        private static readonly string DTFormat = "hh:mm tt";
        private static readonly DateTimeStyles DTFormatStyle = DateTimeStyles.None;
        private static readonly IFormatProvider DTFormatProvider = CultureInfo.InvariantCulture;
        
        
        //  VARIABLES

        [JsonProperty("moon_illumination")]
        public string MoonIllumination { get; set; }
        
        [JsonProperty("moon_phase")]
        public string MoonPhase { get; set; }
        
        [JsonProperty("moonrise")]
        public string Moonrise { get; set; }
        
        [JsonProperty("moonset")]
        public string Moonset { get; set; }
        
        [JsonProperty("sunrise")]
        public string Sunrise { get; set; }
        
        [JsonProperty("sunset")]
        public string Sunset { get; set; }


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

        public WeatherTreeItem ToWeatherTreeItem(string customHeader = null)
        {
            string header = !string.IsNullOrEmpty(customHeader) ? customHeader : this.GetType().Name;

            var result = new WeatherTreeItem(header);

            result.Subitems.Add(new WeatherTreeItem(nameof(MoonIllumination), MoonIllumination));
            result.Subitems.Add(new WeatherTreeItem(nameof(MoonPhase), MoonPhase));
            result.Subitems.Add(new WeatherTreeItem(nameof(Moonrise), Moonrise));
            result.Subitems.Add(new WeatherTreeItem(nameof(Moonset), Moonset));
            result.Subitems.Add(new WeatherTreeItem(nameof(Sunrise), Sunrise));
            result.Subitems.Add(new WeatherTreeItem(nameof(Sunset), Sunset));

            return result;
        }
    }

    public class Country : ValueObject { }

    public class HourlyWeather
    {
        [JsonProperty("DewPointC")]
        public string DewPointC { get; set; }
        
        [JsonProperty("DewPointF")]
        public string DewPointF { get; set; }
        
        [JsonProperty("FeelsLikeC")]
        public string FeelsLikeC { get; set; }
        
        [JsonProperty("FeelsLikeF")]
        public string FeelsLikeF { get; set; }
        
        [JsonProperty("HeatIndexC")]
        public string HeatIndexC { get; set; }
        
        [JsonProperty("HeatIndexF")]
        public string HeatIndexF { get; set; }
        
        [JsonProperty("WindChillC")]
        public string WindChillC { get; set; }
        
        [JsonProperty("WindChillF")]
        public string WindChillF { get; set; }
        
        [JsonProperty("WindGustKmph")]
        public string WindGustKmph { get; set; }
        
        [JsonProperty("WindGustMiles")]
        public string WindGustMiles { get; set; }
        
        [JsonProperty("chanceoffog")]
        public string ChanceOfFog { get; set; }
        
        [JsonProperty("chanceoffrost")]
        public string ChanceOfFrost { get; set; }
        
        [JsonProperty("chanceofhightemp")]
        public string ChanceOfHighTemp { get; set; }
        
        [JsonProperty("chanceofovercast")]
        public string ChanceOfOvercast { get; set; }
        
        [JsonProperty("chanceofrain")]
        public string ChanceOfRain { get; set; }
        
        [JsonProperty("chanceofremdry")]
        public string ChanceOfRemdry { get; set; }
        
        [JsonProperty("chanceofsnow")]
        public string ChanceOfSnow { get; set; }
        
        [JsonProperty("chanceofsunshine")]
        public string ChanceOfSunshine { get; set; }
        
        [JsonProperty("chanceofthunder")]
        public string ChanceOfThunder { get; set; }
        
        [JsonProperty("chanceofwindy")]
        public string ChanceOfWindy { get; set; }
        
        [JsonProperty("cloudcover")]
        public string CloudCover { get; set; }
        
        [JsonProperty("humidity")]
        public string Humidity { get; set; }
        
        [JsonProperty("precipInches")]
        public string PrecipInches { get; set; }
        
        [JsonProperty("precipMM")]
        public string PrecipMM { get; set; }
        
        [JsonProperty("pressure")]
        public string Pressure { get; set; }
        
        [JsonProperty("pressureInches")]
        public string PressureInches { get; set; }
        
        [JsonProperty("tempC")]
        public string TempC { get; set; }
        
        [JsonProperty("tempF")]
        public string TempF { get; set; }
        
        [JsonProperty("time")]
        public string Time { get; set; }
        
        [JsonProperty("uvIndex")]
        public string UvIndex { get; set; }
        
        [JsonProperty("visibility")]
        public string Visibility { get; set; }
        
        [JsonProperty("visibilityMiles")]
        public string VisibilityMiles { get; set; }
        
        [JsonProperty("weatherCode")]
        public string WeatherCode { get; set; }

        [JsonProperty("weatherDesc")]
        public List<WeatherDesc> WeatherDesc { get; set; }
        
        [JsonProperty("weatherIconUrl")]
        public List<WeatherIconUrl> WeatherIconUrl { get; set; }

        [JsonProperty("winddir16Point")]
        public string Winddir16Point { get; set; }

        [JsonProperty("winddirDegree")]
        public string WinddirDegree { get; set; }

        [JsonProperty("windspeedKmph")]
        public string WindspeedKmph { get; set; }

        [JsonProperty("windspeedMiles")]
        public string WindspeedMiles { get; set; }


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

        public WeatherTreeItem ToWeatherTreeItem(string customHeader = null)
        {
            string header = !string.IsNullOrEmpty(customHeader) ? customHeader : this.GetType().Name;

            var result = new WeatherTreeItem(header);
            var weatherDesc = new WeatherTreeItem(nameof(WeatherDesc));
            var weatherIconUrl = new WeatherTreeItem(nameof(WeatherIconUrl));

            for (int i = 0; i < WeatherDesc.Count; i++)
                weatherDesc.Subitems.Add(WeatherDesc[i].ToWeatherTreeItem($"[{i}]"));
            
            for (int i = 0; i < WeatherIconUrl.Count; i++)
                weatherIconUrl.Subitems.Add(WeatherIconUrl[i].ToWeatherTreeItem($"[{i}]"));

            result.Subitems.Add(new WeatherTreeItem(nameof(DewPointC), DewPointC));
            result.Subitems.Add(new WeatherTreeItem(nameof(DewPointF), DewPointF));
            result.Subitems.Add(new WeatherTreeItem(nameof(FeelsLikeC), FeelsLikeC));
            result.Subitems.Add(new WeatherTreeItem(nameof(FeelsLikeF), FeelsLikeF));
            result.Subitems.Add(new WeatherTreeItem(nameof(HeatIndexC), HeatIndexC));
            result.Subitems.Add(new WeatherTreeItem(nameof(HeatIndexF), HeatIndexF));
            result.Subitems.Add(new WeatherTreeItem(nameof(WindChillC), WindChillC));
            result.Subitems.Add(new WeatherTreeItem(nameof(WindChillF), WindChillF));
            result.Subitems.Add(new WeatherTreeItem(nameof(WindGustKmph), WindGustKmph));
            result.Subitems.Add(new WeatherTreeItem(nameof(WindGustMiles), WindGustMiles));
            result.Subitems.Add(new WeatherTreeItem(nameof(ChanceOfFog), ChanceOfFog));
            result.Subitems.Add(new WeatherTreeItem(nameof(ChanceOfFrost), ChanceOfFrost));
            result.Subitems.Add(new WeatherTreeItem(nameof(ChanceOfHighTemp), ChanceOfHighTemp));
            result.Subitems.Add(new WeatherTreeItem(nameof(ChanceOfOvercast), ChanceOfOvercast));
            result.Subitems.Add(new WeatherTreeItem(nameof(ChanceOfRain), ChanceOfRain));
            result.Subitems.Add(new WeatherTreeItem(nameof(ChanceOfRemdry), ChanceOfRemdry));
            result.Subitems.Add(new WeatherTreeItem(nameof(ChanceOfSnow), ChanceOfSnow));
            result.Subitems.Add(new WeatherTreeItem(nameof(ChanceOfSunshine), ChanceOfSunshine));
            result.Subitems.Add(new WeatherTreeItem(nameof(ChanceOfThunder), ChanceOfThunder));
            result.Subitems.Add(new WeatherTreeItem(nameof(ChanceOfWindy), ChanceOfWindy));
            result.Subitems.Add(new WeatherTreeItem(nameof(CloudCover), CloudCover));
            result.Subitems.Add(new WeatherTreeItem(nameof(Humidity), Humidity));
            result.Subitems.Add(new WeatherTreeItem(nameof(PrecipInches), PrecipInches));
            result.Subitems.Add(new WeatherTreeItem(nameof(PrecipMM), PrecipMM));
            result.Subitems.Add(new WeatherTreeItem(nameof(Pressure), Pressure));
            result.Subitems.Add(new WeatherTreeItem(nameof(PressureInches), PressureInches));
            result.Subitems.Add(new WeatherTreeItem(nameof(TempC), TempC));
            result.Subitems.Add(new WeatherTreeItem(nameof(TempF), TempF));
            result.Subitems.Add(new WeatherTreeItem(nameof(Time), Time));
            result.Subitems.Add(new WeatherTreeItem(nameof(UvIndex), UvIndex));
            result.Subitems.Add(new WeatherTreeItem(nameof(Visibility), Visibility));
            result.Subitems.Add(new WeatherTreeItem(nameof(VisibilityMiles), VisibilityMiles));
            result.Subitems.Add(new WeatherTreeItem(nameof(WeatherCode), WeatherCode));
            result.Subitems.Add(weatherDesc);
            result.Subitems.Add(weatherIconUrl);
            result.Subitems.Add(new WeatherTreeItem(nameof(Winddir16Point), Winddir16Point));
            result.Subitems.Add(new WeatherTreeItem(nameof(WinddirDegree), WinddirDegree));
            result.Subitems.Add(new WeatherTreeItem(nameof(WindspeedKmph), WindspeedKmph));
            result.Subitems.Add(new WeatherTreeItem(nameof(WindspeedMiles), WindspeedMiles));

            return result;
        }
    }

    public class Region : ValueObject { }

    public class WeatherDesc : ValueObject { }

    public class WeatherIconUrl : ValueObject { }

    public class WeatherUrl : ValueObject { }

}