using ArudinoConnect.Data;
using ArudinoConnect.Events;
using ArudinoConnect.Static;
using ArudinoConnect.Utilities;
using chkam05.Tools.ControlsEx;
using chkam05.Tools.ControlsEx.Data;
using chkam05.Tools.ControlsEx.Events;
using chkam05.Tools.ControlsEx.InternalMessages;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArudinoConnect.Windows
{
    public partial class MainWindow : Window, INotifyPropertyChanged, IDisposable
    {

        //  CONST

        private static readonly int[] Month30 = { 4, 6, 9, 11 };

        private static readonly Dictionary<DayOfWeek, int> DayOfWeekMap = new Dictionary<DayOfWeek, int>()
        {
            { DayOfWeek.Monday, 1 },
            { DayOfWeek.Tuesday, 2 },
            { DayOfWeek.Wednesday, 3 },
            { DayOfWeek.Thursday, 4 },
            { DayOfWeek.Friday, 5 },
            { DayOfWeek.Saturday, 6 },
            { DayOfWeek.Sunday, 7 }
        };


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ObservableCollection<int> _baudRatesCollection;
        private ObservableCollection<ComPort> _devicesCollection;
        private ObservableCollection<WeatherTreeItem> _weatherTreeData;
        private ObservableCollection<WeatherViewItem> _weatherViewData;
        private int _baudRate;
        private ComPort _selectedDevice;
        private SerialPortConnection _serialPortConnection;
        private BackgroundWorker _timeUpdater;

        private string _console;
        private string _consoleMessage;

        private bool _dtAuto = true;
        private int _dtDayMax = 31;
        private string _dtDay = "1";
        private string _dtMonth = "1";
        private string _dtYear = "2000";
        private string _dtHour = "0";
        private string _dtMinute = "0";
        private string _dtSecond = "0";
        private string _alarmHour = "6";
        private string _alarmMinute = "30";
        private bool _alarmSet = false;
        private int _brightness = 0;
        private bool _brightnessAuto = true;
        private ObservableCollection<(int Value, string Title)> _hourBeepCollection;
        private (int Value, string Title) _hourBeep;


        //  GETTERS & SETTERS

        public ObservableCollection<int> BaudRatesCollection
        {
            get => _baudRatesCollection;
            set
            {
                _baudRatesCollection = value;
                _baudRatesCollection.CollectionChanged += (s, e) => OnPropertyChanged(nameof(BaudRatesCollection));
                OnPropertyChanged(nameof(BaudRatesCollection));
            }
        }
        
        public ObservableCollection<ComPort> DevicesCollection
        {
            get => _devicesCollection;
            set
            {
                _devicesCollection = value;
                _devicesCollection.CollectionChanged += (s, e) => OnPropertyChanged(nameof(DevicesCollection));
                OnPropertyChanged(nameof(DevicesCollection));
            }
        }

        public ObservableCollection<WeatherTreeItem> WeatherTreeData
        {
            get => _weatherTreeData;
            set
            {
                _weatherTreeData = value;
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
                _weatherViewData.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(WeatherViewData)); };
                OnPropertyChanged(nameof(WeatherViewData));
            }
        }

        public int BaudRate
        {
            get => _baudRate;
            set
            {
                _baudRate = BaudRatesCollection != null && BaudRatesCollection.Any() && BaudRatesCollection.Contains(value)
                    ? value
                    : SerialPortConnection.DEFAULT_BAUD_RATE;

                OnPropertyChanged(nameof(BaudRate));
            }
        }

        public ComPort SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
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

        public string Console
        {
            get => _console;
            set
            {
                _console = value;
                OnPropertyChanged(nameof(Console));
            }
        }

        public string ConsoleMessage
        {
            get => _consoleMessage;
            set
            {
                _consoleMessage = value;
                OnPropertyChanged(nameof(ConsoleMessage));
            }
        }

        public bool DtAuto
        {
            get => _dtAuto;
            set
            {
                _dtAuto = value;
                OnPropertyChanged(nameof(DtAuto));
            }
        }

        public int DtDayMax
        {
            get => _dtDayMax;
            set
            {
                _dtDayMax = Math.Max(28, Math.Min(31, value));
                OnPropertyChanged(nameof(DtDayMax));
            }
        }

        public string DtDay
        {
            get => _dtDay;
            set
            {
                _dtDay = value;
                OnPropertyChanged(nameof(DtDay));
            }
        }

        public string DtMonth
        {
            get => _dtMonth;
            set
            {
                _dtMonth = value;
                OnPropertyChanged(nameof(DtMonth));
            }
        }

        public string DtYear
        {
            get => _dtYear;
            set
            {
                _dtYear = value;
                OnPropertyChanged(nameof(DtYear));
            }
        }

        public string DtHour
        {
            get => _dtHour;
            set
            {
                _dtHour = value;
                OnPropertyChanged(nameof(DtHour));
            }
        }

        public string DtMinute
        {
            get => _dtMinute;
            set
            {
                _dtMinute = value;
                OnPropertyChanged(nameof(DtMinute));
            }
        }

        public string DtSecond
        {
            get => _dtSecond;
            set
            {
                _dtSecond = value;
                OnPropertyChanged(nameof(DtSecond));
            }
        }

        public string AlarmHour
        {
            get => _alarmHour;
            set
            {
                _alarmHour = value;
                OnPropertyChanged(nameof(AlarmHour));
            }
        }

        public string AlarmMinute
        {
            get => _alarmMinute;
            set
            {
                _alarmMinute = value;
                OnPropertyChanged(nameof(AlarmMinute));
            }
        }

        public bool AlarmSet
        {
            get => _alarmSet;
            set
            {
                _alarmSet = value;
                OnPropertyChanged(nameof(AlarmSet));
            }
        }

        public int Brightness
        {
            get => _brightness;
            set
            {
                _brightness = Math.Max(0, Math.Min(8, value));
                OnPropertyChanged(nameof(Brightness));
            }
        }

        public bool BrightnessAuto
        {
            get => _brightnessAuto;
            set
            {
                _brightnessAuto = value;
                OnPropertyChanged(nameof(BrightnessAuto));
            }
        }

        public ObservableCollection<(int Value, string Title)> HourBeepCollection
        {
            get => _hourBeepCollection;
            set
            {
                _hourBeepCollection = value;
                _hourBeepCollection.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(HourBeepCollection)); };
                OnPropertyChanged(nameof(HourBeepCollection));
            }
        }

        public (int Value, string Title) HourBeep
        {
            get => _hourBeep;
            set
            {
                _hourBeep = value;
                OnPropertyChanged(nameof(HourBeep));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MainWindow class constructor. </summary>
        public MainWindow()
        {
            SerialPortConnection = new SerialPortConnection();
            SerialPortConnection.ReceivedMessage += ReceiveMessage;

            _timeUpdater = new BackgroundWorker();
            _timeUpdater.WorkerReportsProgress = true;
            _timeUpdater.WorkerSupportsCancellation = true;
            _timeUpdater.DoWork += TimeUpdaterWork;
            _timeUpdater.ProgressChanged += TimeUpdaterUpdate;

            SetupDataCollections();
            InitializeComponent();

            _timeUpdater.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Shutting down application - disposing objects. </summary>
        public void Dispose()
        {
            if (SerialPortConnection != null && SerialPortConnection.IsConnected)
                SerialPortConnection.Disconnect();

            if (_timeUpdater.IsBusy)
                _timeUpdater.CancelAsync();
        }

        #endregion CLASS METHODS

        #region BASE INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on ConnectionButtonEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ConnectionButtonEx_Click(object sender, RoutedEventArgs e)
        {
            if (SerialPortConnection != null)
            {
                if (!SerialPortConnection.IsConnected)
                {
                    SerialPortConnection.PortCom = SelectedDevice.PortName;
                    SerialPortConnection.Connect();
                }
                else
                {
                    SerialPortConnection.Disconnect();
                }
            }
        }

        #endregion BASE INTERACTION METHODS

        #region CONFIGURATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on DownloadConfiguration. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void DownloadConfigurationButtonEx_Click(object sender, RoutedEventArgs e)
        {
            DownloadConfiguration();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on UploadDateTimeConfiguration. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UploadDateTimeConfigurationButtonEx_Click(object sender, RoutedEventArgs e)
        {
            string title = "Date & Time";
            string message = "Uploading date & time configuration...";
            string failMessage = "Time & Date cannot be configured. Please check configuration.";
            PackIconKind icon = PackIconKind.CalendarClock;

            if (DateTime.TryParseExact($"{DtYear}.{DtMonth}.{DtDay} {DtHour}:{DtMinute}:{DtSecond}", "yyyy.MM.dd HH:mm:ss", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            {
                int week = DayOfWeekMap[dt.DayOfWeek];

                var data = new List<ConfigCommandCarrier>()
                {
                    new ConfigCommandCarrier()
                    {
                        Command = $"/date set {DtDay}.{week}.{DtMonth}.{DtYear}",
                        CompleteMessage = "Date configuration updated.",
                        FailMessage = "Date configuration cannot be updated. Please check configuration.",
                    },

                    new ConfigCommandCarrier()
                    {
                        Command = $"/time set {DtHour}:{DtMinute}:{DtSecond}",
                        CompleteMessage = "Time configuration updated.",
                        FailMessage = "Date configuration cannot be updated. Please check configuration.",
                    }
                };

                UploadConfiguration(data, title, message, icon);
            }
            else
            {
                var errorMessage = new InternalMessageEx(InternalMessagesExContainer, title, failMessage, icon);
                InternalMessagesExContainer.ShowMessage(errorMessage);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on UploadAlarmConfiguration. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UploadAlarmConfigurationButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var data = new ConfigCommandCarrier()
            {
                Command = AlarmSet ? $"/alarm set {AlarmHour}:{AlarmMinute}" : "/alarm set disable",
                Title = "Alarm",
                Message = "Uploading alarm configuration...",
                CompleteMessage = "Alarm configuration updated.",
                FailMessage = "Alarm configuration cannot be updated. Please check configuration.",
                Icon = PackIconKind.Alarm,
            };

            UploadSingleConfiguration(data);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on UploadBrightnessConfiguration. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UploadBrightnessConfigurationButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var data = new ConfigCommandCarrier()
            {
                Command = BrightnessAuto ? "/brightness set auto" : $"/brightness set {Brightness}",
                Title = "Brightness",
                Message = "Uploading brightness configuration...",
                CompleteMessage = "Brightness configuration updated.",
                FailMessage = "Brightness configuration cannot be updated. Please check configuration.",
                Icon = PackIconKind.Alarm,
            };

            UploadSingleConfiguration(data);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on UploadHourBeepConfiguration. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UploadHourBeepConfigurationButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var data = new ConfigCommandCarrier()
            {
                Command = HourBeep.Value == 0 ? "/beep set disable" : $"/beep set {HourBeep.Value}",
                Title = "Beep hours",
                Message = "Uploading beep hours configuration...",
                CompleteMessage = "Beep hours configuration updated.",
                FailMessage = "Beep hours configuration cannot be updated. Please check configuration.",
                Icon = PackIconKind.Alarm,
            };

            UploadSingleConfiguration(data);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on UploadAllConfiguration. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UploadAllConfigurationButtonEx_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Download configuration from Arduino. </summary>
        private void DownloadConfiguration()
        {
            var bgDwonloader = new BackgroundWorker();
            bgDwonloader.WorkerReportsProgress = true;
            bgDwonloader.WorkerSupportsCancellation = true;

            var awaitMessage = new AwaitInternalMessageEx(InternalMessagesExContainer,
                "Downloading config", "Checking init state...", PackIconKind.Gear);

            awaitMessage.AllowCancel = true;
            awaitMessage.KeepFinishedOpen = true;

            awaitMessage.OnClose += (s, em) =>
            {
                if (em.Result == InternalMessageResult.Cancel && bgDwonloader.IsBusy)
                    bgDwonloader.CancelAsync();
            };

            var commands = new List<ConfigCommandCarrier>()
            {
                new ConfigCommandCarrier()
                {
                    Command = "/init",
                    CompleteMessage = "Downloading date configuration...",
                    FailMessage = "Could not download configuration. Device is not yet initialized.",
                },
                new ConfigCommandCarrier()
                {
                    Command = "/date get",
                    CompleteMessage = "Downloading time configuration...",
                    FailMessage = "Downloading date configuration failed.",
                },
                new ConfigCommandCarrier()
                {
                    Command = "/time get",
                    CompleteMessage = "Downloading alarm configuration...",
                    FailMessage = "Downloading time configuration failed.",
                },
                new ConfigCommandCarrier()
                {
                    Command = "/alarm get",
                    CompleteMessage = "Downloading beep hours configuration...",
                    FailMessage = "Downloading alarm configuration failed.",
                },
                new ConfigCommandCarrier()
                {
                    Command = "/beep get",
                    CompleteMessage = "Downloading brightness configuration...",
                    FailMessage = "Downloading beep hours configuration failed.",
                },
                new ConfigCommandCarrier()
                {
                    Command = "/beep get",
                    CompleteMessage = "",
                    FailMessage = "Downloading brightness configuration failed.",
                }
            };

            bgDwonloader.DoWork += (s, ew) =>
            {
                string resultMessage = string.Empty;

                foreach (var command in commands)
                {
                    if (ew.Cancel)
                    {
                        resultMessage += $"Downloading configuration cancelled.";
                        break;
                    }

                    var commandResult = ExecuteCommand(command.Command);

                    if (commandResult.Success)
                    {
                        bgDwonloader.ReportProgress(commands.IndexOf(command), new ConfigCommandResult()
                        {
                            Result = commandResult,
                            CompleteMessage = command.CompleteMessage
                        });
                    }
                    else
                    {
                        resultMessage += $"{command.FailMessage}{Environment.NewLine}";
                    }
                }

                if (string.IsNullOrWhiteSpace(resultMessage))
                    ew.Result = "Downloading configuration successed.";
                else
                    ew.Result = resultMessage;
            };

            bgDwonloader.ProgressChanged += (s, ep) =>
            {
                var data = ep.UserState as ConfigCommandResult;

                if (data != null)
                {
                    if (data.Result.Success)
                    {
                        switch (ep.ProgressPercentage)
                        {
                            case 1:
                                if (data.Result.Success)
                                {
                                    var dateParts = data.Result.Data.Split('.');
                                    DtAuto = false;
                                    DtDay = dateParts[1];
                                    DtMonth = dateParts[2];
                                    DtYear = dateParts[3];
                                }
                                break;

                            case 2:
                                if (data.Result.Success)
                                {
                                    var dateParts = data.Result.Data.Split(':');
                                    DtAuto = false;
                                    DtHour = dateParts[0];
                                    DtMinute = dateParts[1];
                                    DtSecond = dateParts[2];
                                }
                                break;

                            case 3:
                                if (data.Result.Success)
                                {
                                    var dateParts = data.Result.Data.Split(new string[] { ":", " " }, StringSplitOptions.RemoveEmptyEntries);
                                    AlarmHour = dateParts[0];
                                    AlarmMinute = dateParts[1];
                                    AlarmSet = dateParts[2].ToLower() == "on";
                                }
                                break;

                            case 4:
                                if (data.Result.Success)
                                {
                                    var dataParts = data.Result.Data.ToLower();

                                    HourBeep = int.TryParse(dataParts, out int beepValue)
                                        ? HourBeepCollection.Any(d => d.Value == beepValue)
                                            ? HourBeepCollection.First(d => d.Value == beepValue)
                                            : HourBeepCollection[0]
                                        : HourBeepCollection[0];
                                }
                                break;

                            case 5:
                                if (data.Result.Success)
                                {
                                    var dataParts = data.Result.Data.ToLower();

                                    BrightnessAuto = dataParts == "auto";
                                    Brightness = int.TryParse(dataParts, out int brightnessValue)
                                        ? brightnessValue : 0;
                                }
                                break;

                            default:
                                break;
                        }
                    }

                    awaitMessage.Message = data.CompleteMessage;
                }
            };

            bgDwonloader.RunWorkerCompleted += (s, ec) =>
            {
                awaitMessage.Message = (string)ec.Result;
                awaitMessage.InvokeFinsh();
            };

            InternalMessagesExContainer.ShowMessage(awaitMessage);
            bgDwonloader.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Execute multiple configuration uplaod command. </summary>
        /// <param name="data"> List of configuration command data carrier objects. </param>
        /// <param name="title"> Internal message startup title. </param>
        /// <param name="message"> Internal message startup message. </param>
        /// <param name="icon"> Internal message icon. </param>
        /// <param name="allowCancel"> Allow cancel process. </param>
        private void UploadConfiguration(List<ConfigCommandCarrier> data, 
            string title, string message, PackIconKind icon, bool allowCancel = false)
        {
            var bgSetter = new BackgroundWorker();
            bgSetter.WorkerReportsProgress = true;
            bgSetter.WorkerSupportsCancellation = allowCancel;

            var awaitMessage = new AwaitInternalMessageEx(InternalMessagesExContainer,
                title, message, icon);

            awaitMessage.AllowCancel = allowCancel;
            awaitMessage.KeepFinishedOpen = true;

            bgSetter.DoWork += (s, ew) =>
            {
                var result = new List<ConfigCommandResult>();

                foreach (var singleCommand in data)
                {
                    if (ew.Cancel)
                        break;

                    bgSetter.ReportProgress(data.IndexOf(singleCommand), singleCommand.Message);

                    var singleCommandResult = ExecuteCommand(singleCommand.Command);

                    result.Add(new ConfigCommandResult()
                    {
                        Result = singleCommandResult,
                        CompleteMessage = singleCommand.CompleteMessage,
                        FailMessage = singleCommand.FailMessage,
                    });
                }

                ew.Result = result;
            };

            bgSetter.ProgressChanged += (s, ep) =>
            {
                awaitMessage.Message = (string) ep.UserState;
            };

            bgSetter.RunWorkerCompleted += (s, ec) =>
            {
                var result = ec.Result as List<ConfigCommandResult>;
                string resultMessage = string.Empty;

                if (ec.Cancelled)
                    resultMessage = "Configuration updating process has been cancelled.";

                else if (result != null && result.Any())
                    foreach (var singleResult in result)
                    {
                        if (singleResult.Result.Success && singleResult.Result.Data == "OK")
                            resultMessage += $"{singleResult.CompleteMessage}{Environment.NewLine}";
                        else
                            resultMessage += $"{singleResult.FailMessage}{Environment.NewLine}";
                    }

                else
                    resultMessage = "Configuration cannot be updated. Please check configuration.";

                awaitMessage.Message = resultMessage;
                awaitMessage.InvokeFinsh();
            };

            InternalMessagesExContainer.ShowMessage(awaitMessage);
            bgSetter.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Execute single configuration uplaod command. </summary>
        /// <param name="data"> Configuration command data carrier object. </param>
        private void UploadSingleConfiguration(ConfigCommandCarrier data)
        {
            var bgSetter = new BackgroundWorker();

            var awaitMessage = new AwaitInternalMessageEx(InternalMessagesExContainer,
                data.Title, data.Message, data.Icon);

            awaitMessage.KeepFinishedOpen = true;

            bgSetter.DoWork += (s, ew) =>
            {
                var result = ExecuteCommand(data.Command);
                ew.Result = result;
            };

            bgSetter.RunWorkerCompleted += (s, ec) =>
            {
                var result = ec.Result as CommandResult;

                if (result != null && result.Success && result.Data == "OK")
                    awaitMessage.Message = data.CompleteMessage;
                else
                    awaitMessage.Message = data.FailMessage;

                awaitMessage.InvokeFinsh();
            };

            InternalMessagesExContainer.ShowMessage(awaitMessage);
            bgSetter.RunWorkerAsync();
        }

        #endregion CONFIGURATION METHODS

        #region CONSOLE INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on ConsoleSendButtonEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ConsoleSendButtonEx_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing any key in MessageTextBoxEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Key event arguments. </param>
        private void MessageTextBoxEx_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage();
        }

        #endregion CONSOLE INTERACTION METHODS

        #region DATE & TIME CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after modifying text in DtUpDownTextBoxEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Text modified event arguments. </param>
        private void DtUpDownTextBoxEx_TextModified(object sender, TextModifiedEventArgs e)
        {
            if (e.UserModified)
            {
                var upDownTextBoxEx = (UpDownTextBoxEx)sender;

                if (upDownTextBoxEx == DtYearUpDownTextBoxEx || upDownTextBoxEx == DtMonthUpDownTextBoxEx)
                {
                    if (int.TryParse(DtYear, out int year) && int.TryParse(DtMonth, out int month) && int.TryParse(DtDay, out int day))
                        UpdateDtDayMax(year, month, day);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update max day value. </summary>
        /// <param name="year"> Year. </param>
        /// <param name="month"> Month. </param>
        /// <param name="day"> Day. </param>
        private void UpdateDtDayMax(int year, int month, int day)
        {
            bool isLeap = (year % 4 == 0 && year % 100 != 0) || year % 400 == 0;

            DtDayMax = month == 2
                ? isLeap ? 29 : 28
                : Month30.Contains(month) ? 30 : 31;

            if (day > DtDayMax)
                DtDay = DtDayMax.ToString();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Background TimeUpdater work method. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Do work event arguments. </param>
        private void TimeUpdaterWork(object sender, DoWorkEventArgs e)
        {
            bool autoCheckpoint = DtAuto;
            DateTime checkpoint = DateTime.Now;

            while (!e.Cancel)
            {
                if (DtAuto)
                {
                    DateTime now = DateTime.Now;

                    bool updateDtDayMax = checkpoint.Month != now.Month || (autoCheckpoint != DtAuto);

                    _timeUpdater.ReportProgress(0, new object[] { now, updateDtDayMax });

                    autoCheckpoint = DtAuto;
                    checkpoint = now;
                }

                Thread.Sleep(1000);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Background TimeUpdater update method. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Do work event arguments. </param>
        private void TimeUpdaterUpdate(object sender, ProgressChangedEventArgs e)
        {
            if (MainTabControlEx.SelectedItem == SettingsTabItemEx)
            {
                var data = (object[])e.UserState;

                DateTime now = (DateTime)data[0];
                bool updateDtDayMax = (bool)data[1];

                DtHour = now.Hour.ToString();
                DtMinute = now.Minute.ToString();
                DtSecond = now.Second.ToString();
                DtYear = now.Year.ToString();
                DtMonth = now.Month.ToString();

                if (updateDtDayMax)
                    UpdateDtDayMax(now.Year, now.Month, now.Day);

                DtDay = now.Day.ToString();
            }
        }

        #endregion DATE & TIME CONTROL METHODS

        #region MESSAGING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Send message to serial port device. </summary>
        private void SendMessage()
        {
            if (!string.IsNullOrEmpty(ConsoleMessage) && SerialPortConnection?.IsConnected == true)
            {
                if (ConsoleMessage.ToLower() == "/help")
                {
                    Console += $"{Environment.NewLine}";
                    Console += $"--- HELP -------------------------------------------------------{Environment.NewLine}";
                    Console += $"/alarm get                              Get Alarm config. {Environment.NewLine}";
                    Console += $"/alarm set [off/disable] [hh:mm]        Set Alarm. {Environment.NewLine}";
                    Console += $"/beep get                               Get Beep hours config. {Environment.NewLine}";
                    Console += $"/beep set [off/disable] [0/1/3/6/12/24] Set beep hours. {Environment.NewLine}";
                    Console += $"/brightness get                         Get brightness config. {Environment.NewLine}";
                    Console += $"/brightness set [a/auto] [0..8]         Set brightness. {Environment.NewLine}";
                    Console += $"/date get                               Get date config. {Environment.NewLine}";
                    Console += $"/date set [dd.MM.yyyy / dd.w.mm.yyy]    Set date. {Environment.NewLine}";
                    Console += $"/init                                   Get init status. {Environment.NewLine}";
                    Console += $"/time get                               Get time config. {Environment.NewLine}";
                    Console += $"/time set [hh:mm:ss / hh:mm]            Set time. {Environment.NewLine}";
                    Console += $"----------------------------------------------------------------{Environment.NewLine}";
                    Console += $"{Environment.NewLine}";

                    consoleTextBoxEx.ScrollToEnd();
                    ConsoleMessage = string.Empty;

                    return;
                }

                SerialPortConnection.SendMessage(ConsoleMessage);
                consoleTextBoxEx.ScrollToEnd();
                ConsoleMessage = string.Empty;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Receive message from serial prot device. </summary>
        private void ReceiveMessage(object sender, SerialPortReceivedMessageEventArgs e)
        {
            if (e.HasMessage)
            {
                string tail = !e.Message.EndsWith(Environment.NewLine) ? Environment.NewLine : string.Empty;
                Console += $"{e.Message}{tail}";
                consoleTextBoxEx.ScrollToEnd();
            }
        }

        #endregion MESSAGING METHODS

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

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data collections. </summary>
        private void SetupDataCollections()
        {
            BaudRatesCollection = new ObservableCollection<int>(SerialPortConnection.BAUD_RATES);
            DevicesCollection = new ObservableCollection<ComPort>(SerialPortConnection.GetComDevices());

            HourBeepCollection = new ObservableCollection<(int Value, string Title)>()
            {
                (0, "Disabled"),
                (1, "Every hour"),
                (3, "Every 3 hours"),
                (6, "Every 6 hours"),
                (12, "Every 12 hours"),
                (24, "Every 24 hours"),
            };

            HourBeep = HourBeepCollection[0];

            if (BaudRatesCollection != null && BaudRatesCollection.Any())
                BaudRate = SerialPortConnection.DEFAULT_BAUD_RATE;

            if (DevicesCollection != null && DevicesCollection.Any())
                SelectedDevice = DevicesCollection.First();
        }

        #endregion SETUP METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Execute command with direct output. </summary>
        /// <param name="command"> Command to execute. </param>
        /// <param name="timeout"> Time waiting for response in miliseconds. </param>
        /// <returns> Tuple (bool, string) where first value idicates success or fail, 
        /// and second value contains received data. </returns>
        private CommandResult ExecuteCommand(string command, int timeout = 5000)
        {
            string message = null;
            bool successed = false;

            if (SerialPortConnection.IsConnected)
            {
                bool cancel = false;
                DateTime dtStart = DateTime.Now;

                var receiver = new EventHandler<SerialPortReceivedMessageEventArgs>((s, e) =>
                {
                    if (e.HasMessage)
                    {
                        switch (e.State)
                        {
                            case ReceivedMessageState.Error:
                            case ReceivedMessageState.System:
                                cancel = true;
                                break;

                            case ReceivedMessageState.Message:
                                message = e.Message.Replace(command, "").Replace(Environment.NewLine, "");
                                successed = true;
                                break;
                        }
                    }
                });

                SerialPortConnection.ReceivedMessage += receiver;
                SerialPortConnection.SendMessage(command);

                while (!cancel && string.IsNullOrEmpty(message) && dtStart.AddMilliseconds(timeout) > DateTime.Now) { };

                SerialPortConnection.ReceivedMessage -= receiver;
            }

            return new CommandResult(successed, message);
        }

        #endregion UTILITY METHODS

        #region WEATHER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on DownloadWeather. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void DownloadWeatherButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var downloader = new WeatherDownloader("Katowice");
            var bgDwonloader = new BackgroundWorker();

            bgDwonloader.WorkerSupportsCancellation = true;

            var awaitMessage = new AwaitInternalMessageEx(InternalMessagesExContainer, 
                "Weather", "Downloading weather...", PackIconKind.WeatherSunny);

            awaitMessage.AllowCancel = true;
            awaitMessage.KeepFinishedOpen = false;

            awaitMessage.OnClose += (s, ec) =>
            {
                if (ec.Result == InternalMessageResult.Cancel)
                    bgDwonloader.CancelAsync();
            };

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
                    WeatherViewData = new ObservableCollection<WeatherViewItem>(
                        response.Data.Weather.Select(w => new WeatherViewItem(w)));

                    WeatherTreeData = new ObservableCollection<WeatherTreeItem>()
                    {
                        response.Data.ToWeatherTreeItem()
                    };
                }
                else
                {
                    WeatherViewData = null;
                    WeatherTreeData = null;
                }

                awaitMessage.Close();
            };

            InternalMessagesExContainer.ShowMessage(awaitMessage);
            bgDwonloader.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on UploadWeather. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UploadWeatherButtonEx_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion WEATHER METHODS

    }
}
