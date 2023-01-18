using ArudinoConnect.Mappers;
using ArudinoConnect.Utilities;
using chkam05.Tools.ControlsEx.InternalMessages;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ArudinoConnect.Data
{
    public class DataController : INotifyPropertyChanged, IDisposable
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private static DataController _instance;
        private SerialPortConnection _serialPortConnection;

        private DispatcherTimer _weatherBgAutoSync;
        private DateTime _weatherAutoSyncLastUpdate = DateTime.MinValue;
        private long _weatherAutoSyncUpdateInterval = 86400;

        private bool _weatherAutoSync = false;
        private WeatherData _weatherData;
        private ObservableCollection<WeatherHourViewItem> _weatherCurrentDayData;
        private ObservableCollection<WeatherTreeItem> _weatherTreeData;
        private ObservableCollection<WeatherViewItem> _weatherViewData;


        //  GETTERS & SETTERS

        public static DataController Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataController();

                return _instance;
            }
        }

        public SerialPortConnection SerialPortConnection
        {
            get => _serialPortConnection;
            set
            {
                _serialPortConnection = value;
                OnPropertyChanged(nameof(SerialPortConnection));
            }
        }

        public bool WeatherAutoSync
        {
            get => _weatherAutoSync;
            set
            {
                _weatherAutoSync = value && SerialPortConnection.IsConnected;

                if (_weatherAutoSync)
                    RunWeatherAutoSync();
                else
                    StopWeatherAutoSync();

                OnPropertyChanged(nameof(WeatherAutoSync));
            }
        }

        public WeatherData WeatherData
        {
            get => _weatherData;
            set
            {
                _weatherData = value;
                OnPropertyChanged(nameof(WeatherData));
            }
        }

        public ObservableCollection<WeatherHourViewItem> WeatherCurrentDayData
        {
            get => _weatherCurrentDayData;
            set
            {
                _weatherCurrentDayData = value;
                if (_weatherCurrentDayData != null)
                    _weatherCurrentDayData.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(WeatherCurrentDayData)); };
                OnPropertyChanged(nameof(WeatherCurrentDayData));
            }
        }

        public ObservableCollection<WeatherTreeItem> WeatherTreeData
        {
            get => _weatherTreeData;
            set
            {
                _weatherTreeData = value;
                if (_weatherTreeData != null)
                    _weatherTreeData.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(WeatherTreeData)); };
                OnPropertyChanged(nameof(WeatherTreeData));
            }
        }

        public ObservableCollection<WeatherViewItem> WeatherViewData
        {
            get => _weatherViewData;
            set
            {
                _weatherViewData = value;
                if (_weatherViewData != null)
                    _weatherViewData.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(WeatherViewData)); };
                OnPropertyChanged(nameof(WeatherViewData));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> DataController class constructor. </summary>
        private DataController()
        {
            SerialPortConnection = new SerialPortConnection();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Destroying object - disposing objects. </summary>
        public void Dispose()
        {
            StopWeatherAutoSync();
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

        #region WEATHER DATA MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Download weather async. </summary>
        /// <param name="city"> City of weather. </param>
        /// <param name="autoStart"> Auto start async process. </param>
        /// <param name="runWorkerCompletedEvent"> Method that will be invoked after donwloading weather. </param>
        /// <returns> Async background downloader. </returns>
        public BackgroundWorker DownloadWeatherAsync(string city, bool autoStart = true, RunWorkerCompletedEventHandler runWorkerCompletedEvent = null)
        {
            var downloader = new WeatherDownloader(city);
            var bgDwonloader = new BackgroundWorker();

            bgDwonloader.WorkerSupportsCancellation = true;

            bgDwonloader.DoWork += (s, ew) =>
            {
                if (!ew.Cancel)
                    ew.Result = downloader.DownloadWeather();
            };

            bgDwonloader.RunWorkerCompleted += (s, ec) =>
            {
                var response = (WeahterResponse)ec.Result;

                if (!ec.Cancelled && response != null && response.Success && response.Data != null)
                {
                    WeatherData = response.Data;

                    WeatherTreeData = new ObservableCollection<WeatherTreeItem>()
                    {
                        WeatherData.ToWeatherTreeItem()
                    };

                    WeatherViewData = new ObservableCollection<WeatherViewItem>(
                        WeatherData.Weather.Select(w => new WeatherViewItem(w)));

                    var currentDayWeather = WeatherData.Weather.FirstOrDefault(w 
                        => w.GetDateTime().HasValue && w.GetDateTime().Value.Date == DateTime.Now.Date);

                    WeatherCurrentDayData = currentDayWeather != null
                        ? new ObservableCollection<WeatherHourViewItem>(currentDayWeather.Hourly.Select(h => new WeatherHourViewItem(h)))
                        : null;
                }
                else
                {
                    WeatherViewData = null;
                    WeatherTreeData = null;
                }

                runWorkerCompletedEvent?.Invoke(s, ec);
            };

            if (autoStart)
                bgDwonloader.RunWorkerAsync();

            return bgDwonloader;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Upload weather async. </summary>
        /// <param name="autoStart"> Auto start async process. </param>
        /// <param name="progressChangedEvent"> Method that will be invoked after uploading single weather day. </param>
        /// <param name="runWorkerCompletedEvent"> Method that will be invoked after uploading weather. </param>
        /// <returns> Async background uploader. </returns>
        public BackgroundWorker UploadWeatherAsync(bool autoStart = true,
            ProgressChangedEventHandler progressChangedEvent = null,
            RunWorkerCompletedEventHandler runWorkerCompletedEvent = null)
        {
            if (SerialPortConnection.IsConnected && WeatherViewData != null && WeatherViewData.Any())
            {
                List<ConfigCommandCarrier> commands = new List<ConfigCommandCarrier>
                {
                    new ConfigCommandCarrier()
                    {
                        Command = $"/weather clear",
                        CompleteMessage = $"Weather cleared.",
                        FailMessage = $"Failed to clear message.",
                        Message = $"Uploading weather data to Arduino..."
                    }
                };

                foreach (var weatherData in WeatherViewData)
                {
                    string dateTime = weatherData.Date.Replace("-", ".");

                    List<int> codes = weatherData.HourlyWeather
                        .Select(w => WeatherDataMappers.MapWeatherCodeToArduinoCode(w.WeatherCode))
                        .ToList();

                    commands.Add(new ConfigCommandCarrier()
                    {
                        Command = $"/weather add {dateTime} {codes.Count},{string.Join(",", codes)}",
                        CompleteMessage = $"Added weather for {dateTime}.",
                        FailMessage = $"Weather for {dateTime} cannot be added. Please check data.",
                        Message = $"Uploading weather for {dateTime}",
                    });
                }

                if (commands.Count > 1)
                {
                    var scm = new SerialCommander(SerialPortConnection);
                    return scm.UploadConfigurationAsync(commands, autoStart, false, progressChangedEvent, runWorkerCompletedEvent);
                }
            }

            return null;
        }

        #endregion WEATHER DATA MANAGEMENT

        #region WEATHER BACKGROUND AUTO SYNCHRONISER

        //  --------------------------------------------------------------------------------
        /// <summary> Check if weather auto synchroniser is working. </summary>
        /// <returns> True - wehater auto synchroniser is working; False - otherwise. </returns>
        private bool IsWeatherAutoSyncRunning()
        {
            return _weatherBgAutoSync != null
                && _weatherBgAutoSync.IsEnabled;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Run weather auto synchroniser. </summary>
        private void RunWeatherAutoSync()
        {
            StopWeatherAutoSync();

            _weatherAutoSyncLastUpdate = DateTime.MinValue;

            if (_weatherBgAutoSync == null)
                _weatherBgAutoSync = new DispatcherTimer();

            _weatherBgAutoSync.Tick += WeatherAutoSync_Tick;
            _weatherBgAutoSync.Interval = new TimeSpan(1, 0, 0);
            _weatherBgAutoSync.Start();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Weather auto syncrhoniser work method. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Event arguments. </param>
        private void WeatherAutoSync_Tick(object sender, EventArgs e)
        {
            if (!_weatherAutoSync)
            {
                StopWeatherAutoSync();
                return;
            }

            else if (!SerialPortConnection.IsConnected)
            {
                WeatherAutoSync = false;
                StopWeatherAutoSync();
                return;
            }

            else if (DateTime.Now.Date >= _weatherAutoSyncLastUpdate)
            {
                _weatherAutoSyncLastUpdate = DateTime.Now
                    .AddSeconds(_weatherAutoSyncUpdateInterval);

                DownloadWeatherAsync("Katowice", true, (s,ec) =>
                {
                    var response = (WeahterResponse)ec.Result;
                    if (!ec.Cancelled && response != null && response.Success && response.Data != null)
                        UploadWeatherAsync();
                });
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop weather auto synchroniser. </summary>
        private void StopWeatherAutoSync()
        {
            if (IsWeatherAutoSyncRunning())
                _weatherBgAutoSync.Stop();
        }

        #endregion WEATHER BACKGROUND AUTO SYNCHRONISER

    }
}
