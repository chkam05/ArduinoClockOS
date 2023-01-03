using ArudinoConnect.Mappers;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArudinoConnect.Data
{
    public class WeatherViewItem : INotifyPropertyChanged
    {

        //  CONST

        private static readonly string DateFormat = "yyyy-MM-dd";
        private static readonly string TimeFormat = "HH:mm";
        private static readonly string TempFormat = "{0}°C";


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _avgTemp;
        private string _date;
        private string _moonrise;
        private string _moonset;
        private string _sunrise;
        private string _sunset;
        private ObservableCollection<WeatherHourViewItem> _hourlyWeather;


        //  GETTERS & SETTERS

        public string AvgTemp
        {
            get => _avgTemp;
            set
            {
                _avgTemp = value;
                OnPropertyChanged(nameof(AvgTemp));
            }
        }

        public string Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public string Moonrise
        {
            get => _moonrise;
            set
            {
                _moonrise = value;
                OnPropertyChanged(nameof(Moonrise));
            }
        }

        public string Moonset
        {
            get => _moonset;
            set
            {
                _moonset = value;
                OnPropertyChanged(nameof(Moonset));
            }
        }

        public string Sunrise
        {
            get => _sunrise;
            set
            {
                _sunrise = value;
                OnPropertyChanged(nameof(Sunrise));
            }
        }

        public string Sunset
        {
            get => _sunset;
            set
            {
                _sunset = value;
                OnPropertyChanged(nameof(Sunset));
            }
        }

        public ObservableCollection<WeatherHourViewItem> HourlyWeather
        {
            get => _hourlyWeather;
            set
            {
                _hourlyWeather = value;
                _hourlyWeather.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(HourlyWeather)); };
                OnPropertyChanged(nameof(HourlyWeather));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> WeatherViewItem class constructor. </summary>
        /// <param name="weatherData"> Weather data. </param>
        public WeatherViewItem(Weather weatherData)
        {
            if (weatherData.Astronomy != null && weatherData.Astronomy.Any())
            {
                var astronomy = weatherData.Astronomy[0];

                Moonrise = astronomy.GetMoonriseDateTime()?.ToString(TimeFormat);
                Moonset = astronomy.GetMoonsetDateTime()?.ToString(TimeFormat);
                Sunrise = astronomy.GetSunriseDateTime()?.ToString(TimeFormat);
                Sunset = astronomy.GetSunsetDateTime()?.ToString(TimeFormat);
            }

            AvgTemp = string.Format(TempFormat, weatherData.AvgTempC);
            Date = weatherData.GetDateTime()?.ToString(DateFormat);
            HourlyWeather = new ObservableCollection<WeatherHourViewItem>();

            if (weatherData.Hourly != null && weatherData.Hourly.Any())
                weatherData.Hourly.ForEach(d => HourlyWeather.Add(new WeatherHourViewItem(d)));
        }

        #endregion CLASS METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler event. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

    }

    public class WeatherHourViewItem : INotifyPropertyChanged
    {

        //  CONST

        private static readonly string TimeFormat = "HH:mm";
        private static readonly string TempFormat = "{0}°C";


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _hour;
        private string _temp;
        private int _weatherCode;
        private PackIconKind _weatherIcon;
        private string _weatherName;


        //  GETTERS & SETTERS

        public string Hour
        {
            get => _hour;
            set
            {
                _hour = value;
                OnPropertyChanged(nameof(Hour));
            }
        }

        public string Temp
        {
            get => _temp;
            set
            {
                _temp = value;
                OnPropertyChanged(nameof(Temp));
            }
        }

        public int WeatherCode
        {
            get => _weatherCode;
            set
            {
                _weatherCode = value;
                OnPropertyChanged(nameof(WeatherCode));
            }
        }

        public PackIconKind WeatherIcon
        {
            get => _weatherIcon;
            set
            {
                _weatherIcon = value;
                OnPropertyChanged(nameof(WeatherIcon));
            }
        }

        public string WeatherName
        {
            get => _weatherName;
            set
            {
                _weatherName = value;
                OnPropertyChanged(nameof(WeatherName));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> WeatherHourViewItem class constructor. </summary>
        /// <param name="weahterData"> Hourly weather data. </param>
        public WeatherHourViewItem(HourlyWeather weahterData)
        {
            Hour = weahterData.GetTime()?.ToString(TimeFormat);
            Temp = string.Format(TempFormat, weahterData.TempC);
            WeatherCode = weahterData.GetWeatherCode();
            WeatherIcon = WeatherDataMappers.MapWeatherCodeToPackIconKind(WeatherCode);
            WeatherName = WeatherDataMappers.MapWeatherCodeToName(WeatherCode);
        }

        #endregion CLASS METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler event. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

    }
}
