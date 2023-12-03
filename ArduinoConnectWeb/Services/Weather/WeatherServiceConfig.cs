namespace ArduinoConnectWeb.Services.Weather
{
    public class WeatherServiceConfig
    {

        //  CONST

        public const string DATE_FORMAT = "yyyy-MM-dd";
        public const string MEDIA_TYPE = "application/json";
        public const string TIME_FORMAT = "HH:mm";
        public const string TEMPERATURE_FORMAT = "{0}°C";
        public const string PERCENT_FORMAT = "{0}%";
        public const string PRECIPITATION_FORMAT = "{0} mm";
        public const string PRESSURE_FORMAT = "{0} hPa";
        public const string SNOW_FORMAT = "{0} cm";
        public const string URL = "https://wttr.in/{0}?format=j1";
        public const string VISIBILITY_FORMAT = "{0} km";
        public const string WIND_DIRECTION_FORMAT = "{0}° {1}";
        public const string WIND_SPEED_FORMAT = "{0} Km/h";


        //  VARIABLES

        public TimeSpan TimeOut { get; set; }
		
		
		//	METHODS
		
		#region CLASS METHODS
		
		//  --------------------------------------------------------------------------------
        /// <summary> WeatherServiceConfig class constructor. </summary>
        public WeatherServiceConfig()
        {
            //
        }
		
		#endregion CLASS METHODS

    }
}
