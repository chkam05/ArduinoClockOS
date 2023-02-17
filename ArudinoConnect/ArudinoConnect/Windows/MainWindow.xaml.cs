using ArudinoConnect.Data;
using ArudinoConnect.Events;
using ArudinoConnect.InternalMessages;
using ArudinoConnect.Mappers;
using ArudinoConnect.Static;
using ArudinoConnect.Utilities;
using chkam05.Tools.ControlsEx;
using chkam05.Tools.ControlsEx.Data;
using chkam05.Tools.ControlsEx.Events;
using chkam05.Tools.ControlsEx.InternalMessages;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Windows.Foundation.Collections;
using static chkam05.Tools.ControlsEx.Events.Delegates;
using Border = System.Windows.Controls.Border;
using Point = System.Windows.Point;

namespace ArudinoConnect.Windows
{
    public partial class MainWindow : Window, INotifyPropertyChanged
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

        private static readonly Dictionary<string, int> PianoButtonsMap = new Dictionary<string, int>()
        {
            { "PianoKeyC1", 1 },
            { "PianoKeyC_1", 2 },
            { "PianoKeyD1", 3 },
            { "PianoKeyD_1", 4 },
            { "PianoKeyE1", 5 },
            { "PianoKeyF1", 6 },
            { "PianoKeyF_1", 7 },
            { "PianoKeyG1", 8 },
            { "PianoKeyG_1", 9 },
            { "PianoKeyA1", 10 },
            { "PianoKeyA_1", 11 },
            { "PianoKeyH1", 12 },
            { "PianoKeyC2", 13 },
            { "PianoKeyC_2", 14 },
            { "PianoKeyD2", 15 },
            { "PianoKeyD_2", 16 },
            { "PianoKeyE2", 17 },
            { "PianoKeyF2", 18 },
            { "PianoKeyF_2", 19 },
            { "PianoKeyG2", 20 },
            { "PianoKeyG_2", 21 },
            { "PianoKeyA2", 22 },
            { "PianoKeyA_2", 23 },
            { "PianoKeyH2", 24 },
            { "PianoKeyC3", 25 },
            { "PianoKeyC_3", 26 },
            { "PianoKeyD3", 27 },
            { "PianoKeyD_3", 28 },
            { "PianoKeyE3", 29 },
        };

        private static readonly Dictionary<Key, int> PianoKeyMap = new Dictionary<Key, int>()
        {
            { Key.Z, 1 },
            { Key.S, 2 },
            { Key.X, 3 },
            { Key.D, 4 },
            { Key.C, 5 },
            { Key.V, 6 },
            { Key.G, 7 },
            { Key.B, 8 },
            { Key.H, 9 },
            { Key.N, 10 },
            { Key.J, 11 },
            { Key.M, 12 },
            { Key.OemComma , 13 },
            { Key.L, 14 },
            { Key.OemPeriod, 15 },
            { Key.OemSemicolon, 16 },
            { Key.OemQuestion, 17 },
            { Key.Q , 13 },
            { Key.D2, 14 },
            { Key.W, 15 },
            { Key.D3, 16 },
            { Key.E, 17 },
            { Key.R, 18 },
            { Key.D5, 19 },
            { Key.T, 20 },
            { Key.D6, 21 },
            { Key.Y, 22 },
            { Key.D7, 23 },
            { Key.U, 24 },
            { Key.I, 25 },
            { Key.D9, 26 },
            { Key.O, 27 },
            { Key.D0, 28 },
            { Key.P, 29 },
        };


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private Point _pianoNoteDragStartPoint;

        private DataController _dataController;
        private ObservableCollection<int> _baudRatesCollection;
        private ObservableCollection<ComPort> _devicesCollection;
        private ObservableCollection<PianoNote> _notes;
        private int _noteDuration = 12;
        private PianoNote _selectedNote;
        private bool _pianoRecording = false;
        private int _pianoShift = 3;
        private int _baudRate;
        private ComPort _selectedDevice;
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

        public System.Windows.Forms.NotifyIcon TrayIcon;

        public DataController DataController
        {
            get => _dataController;
            private set
            {
                _dataController = value;
                OnPropertyChanged(nameof(DataController));
            }
        }

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

        public ObservableCollection<PianoNote> Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                _notes.CollectionChanged += (s, e) =>
                {
                    OnPropertyChanged(nameof(Notes));
                    OnPropertyChanged(nameof(AnyNote));
                };
                OnPropertyChanged(nameof(Notes));
                OnPropertyChanged(nameof(AnyNote));
            }
        }

        public bool AnyNote
        {
            get => Notes?.Any() == true;
        }

        public int NoteDuration
        {
            get => _noteDuration;
            set
            {
                _noteDuration = Math.Max(1, value);
                OnPropertyChanged(nameof(NoteDuration));
            }
        }

        public PianoNote SelectedNote
        {
            get => _selectedNote;
            set
            {
                _selectedNote = value;
                OnPropertyChanged(nameof(SelectedNote));
            }
        }

        public bool PianoRecording
        {
            get => _pianoRecording;
            set
            {
                _pianoRecording = value;
                OnPropertyChanged(nameof(PianoRecording));
            }
        }

        public int PianoShift
        {
            get => _pianoShift;
            set
            {
                _pianoShift = Math.Min(6, Math.Max(0, value));
                OnPropertyChanged(nameof(PianoShift));
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
            DataController = DataController.Instance;
            DataController.SerialPortConnection.ReceivedMessage += ReceiveMessage;

            _timeUpdater = new BackgroundWorker();
            _timeUpdater.WorkerReportsProgress = true;
            _timeUpdater.WorkerSupportsCancellation = true;
            _timeUpdater.DoWork += TimeUpdaterWork;
            _timeUpdater.ProgressChanged += TimeUpdaterUpdate;

            SetupDataCollections();
            InitializeComponent();

            DataController.BluetoothConnection = new BluetoothConnection(InternalMessagesExContainer);

            TrayIcon = new System.Windows.Forms.NotifyIcon();
            TrayIcon.BalloonTipText = "Arduino Connect has been minimised. Click the tray icon to show.";
            TrayIcon.BalloonTipTitle = "Show Arduino Connect";
            TrayIcon.Text = "Arduino Connect";
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/ArudinoConnect;component/AppIco.ico")).Stream;
            TrayIcon.Icon = new System.Drawing.Icon(iconStream);
            TrayIcon.MouseClick += TrayIcon_MouseClick;
            TrayIcon.DoubleClick += TrayIcon_DoubleClick;
            TrayIcon.Visible = true;

            _timeUpdater.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing Window state. </summary>
        /// <param name="e"> Event arguments. </param>
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        #endregion CLASS METHODS

        #region BASE INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on ConnectionButtonEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ConnectionButtonEx_Click(object sender, RoutedEventArgs e)
        {
            if (DataController.SerialPortConnection != null)
            {
                if (!DataController.SerialPortConnection.IsConnected)
                {
                    DataController.SerialPortConnection.PortCom = SelectedDevice.PortName;
                    DataController.SerialPortConnection.Connect();
                }
                else
                {
                    DataController.SerialPortConnection.Disconnect();
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on ConnectionBluetoothButtonEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ConnectionBluetoothButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var btDiscoverIm = new BluetoothDiscoverIM(InternalMessagesExContainer);

            btDiscoverIm.Title = "Bluetooth discovery";
            btDiscoverIm.IconKind = PackIconKind.Bluetooth;
            btDiscoverIm.Buttons = new InternalMessageButtons[] { InternalMessageButtons.OkButton, InternalMessageButtons.CancelButton };
            btDiscoverIm.OnClose += OnBluetoothDeviceSelected;

            InternalMessagesExContainer.ShowMessage(btDiscoverIm);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting bluetooth device to connect. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Internal Message Close Event Arguments. </param>
        private void OnBluetoothDeviceSelected(object sender, InternalMessageCloseEventArgs e)
        {
            if (e.Result == InternalMessageResult.Ok)
            {
                var btDiscovery = BluetoothDiscovery.Instance;

                if (btDiscovery.SelectedDevice != null)
                {
                    if (DataController.BluetoothConnection == null)
                        DataController.BluetoothConnection = new BluetoothConnection(InternalMessagesExContainer);

                    var btConnection = DataController.BluetoothConnection;

                    btConnection.Connect(btDiscovery.SelectedDevice);
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

            if (DateTime.TryParseExact($"{DtYear}.{DtMonth}.{DtDay} {DtHour}:{DtMinute}:{DtSecond}", "yyyy.M.d H:m:s", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            {
                int week = DayOfWeekMap[dt.DayOfWeek];

                var data = new List<ConfigCommandCarrier>()
                {
                    new ConfigCommandCarrier()
                    {
                        Command = $"/lock Updating...",
                        CompleteMessage = "Service mode enabled.",
                        FailMessage = "Failed to enable service mode.",
                        RequiredResponse = "OK",
                    },

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
                        FailMessage = "Time configuration cannot be updated. Please check configuration.",
                    },

                    new ConfigCommandCarrier()
                    {
                        Command = $"/unlock",
                        CompleteMessage = "Service mode disabled.",
                        FailMessage = "Failed to disable service mode. Go to command line and type '/unlock', or restart device.",
                        RequiredResponse = "OK",
                    },
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
            var data = new List<ConfigCommandCarrier>();
            var title = "Configuration update.";
            var message = "Updating configuration...";
            var icon = PackIconKind.Gear;

            data.Add(new ConfigCommandCarrier()
            {
                Command = $"/lock Updating...",
                CompleteMessage = "Service mode enabled.",
                FailMessage = "Failed to enable service mode.",
                RequiredResponse = "OK",
            });

            if (DateTime.TryParseExact($"{DtYear}.{DtMonth}.{DtDay} {DtHour}:{DtMinute}:{DtSecond}", "yyyy.MM.dd HH:mm:ss",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            {
                int week = DayOfWeekMap[dt.DayOfWeek];

                data.Add(new ConfigCommandCarrier()
                {
                    Command = $"/date set {DtDay}.{week}.{DtMonth}.{DtYear}",
                    CompleteMessage = "Date configuration updated.",
                    FailMessage = "Date configuration cannot be updated. Please check configuration.",
                });

                data.Add(new ConfigCommandCarrier()
                {
                    Command = $"/time set {DtHour}:{DtMinute}:{DtSecond}",
                    CompleteMessage = "Time configuration updated.",
                    FailMessage = "Date configuration cannot be updated. Please check configuration.",
                });
            }

            data.Add(new ConfigCommandCarrier()
            {
                Command = AlarmSet ? $"/alarm set {AlarmHour}:{AlarmMinute}" : "/alarm set disable",
                CompleteMessage = "Alarm configuration updated.",
                FailMessage = "Alarm configuration cannot be updated. Please check configuration.",
            });

            data.Add(new ConfigCommandCarrier()
            {
                Command = BrightnessAuto ? "/brightness set auto" : $"/brightness set {Brightness}",
                CompleteMessage = "Brightness configuration updated.",
                FailMessage = "Brightness configuration cannot be updated. Please check configuration.",
            });

            data.Add(new ConfigCommandCarrier()
            {
                Command = HourBeep.Value == 0 ? "/beep set disable" : $"/beep set {HourBeep.Value}",
                CompleteMessage = "Beep hours configuration updated.",
                FailMessage = "Beep hours configuration cannot be updated. Please check configuration.",
            });

            data.Add(new ConfigCommandCarrier()
            {
                Command = $"/unlock",
                CompleteMessage = "Service mode disabled.",
                FailMessage = "Failed to disable service mode. Go to command line and type '/unlock', or restart device.",
                RequiredResponse = "OK",
            });

            if (data.Count > 0)
                UploadConfiguration(data, title, message, icon);
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
                    RequiredResponse = "YES",
                },
                new ConfigCommandCarrier()
                {
                    Command = $"/lock Downloading...",
                    CompleteMessage = "Service mode enabled.",
                    FailMessage = "Failed to enable service mode.",
                    RequiredResponse = "OK",
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
                    Command = "/brightness get",
                    CompleteMessage = "Unlocking device...",
                    FailMessage = "Downloading brightness configuration failed.",
                },
                new ConfigCommandCarrier()
                {
                    Command = $"/unlock",
                    CompleteMessage = "Service mode disabled.",
                    FailMessage = "Failed to disable service mode. Go to command line and type '/unlock', or restart device.",
                    RequiredResponse = "OK",
                },
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

                    var commandResult = ExecuteCommand(command.Command, command.RequiredResponse);

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
                            case 2:
                                if (data.Result.Success)
                                {
                                    var dateParts = data.Result.Data.Split('.');
                                    DtAuto = false;
                                    DtDay = dateParts[1];
                                    DtMonth = dateParts[2];
                                    DtYear = dateParts[3];
                                }
                                break;

                            case 3:
                                if (data.Result.Success)
                                {
                                    var dateParts = data.Result.Data.Split(':');
                                    DtAuto = false;
                                    DtHour = dateParts[0];
                                    DtMinute = dateParts[1];
                                    DtSecond = dateParts[2];
                                }
                                break;

                            case 4:
                                if (data.Result.Success)
                                {
                                    var dateParts = data.Result.Data.Split(new string[] { ":", " " }, StringSplitOptions.RemoveEmptyEntries);
                                    AlarmHour = dateParts[0];
                                    AlarmMinute = dateParts[1];
                                    AlarmSet = dateParts[2].ToLower() == "on";
                                }
                                break;

                            case 5:
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

                            case 6:
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
            SerialCommander scm = new SerialCommander(DataController.SerialPortConnection);

            var awaitMessage = new AwaitInternalMessageEx(InternalMessagesExContainer,
                title, message, icon);

            awaitMessage.AllowCancel = allowCancel;
            awaitMessage.KeepFinishedOpen = true;

            ProgressChangedEventHandler onProgress = (s, ep) =>
            {
                string progressMessage = (string)ep.UserState;

                if (progressMessage != null)
                    awaitMessage.Message = progressMessage;
            };

            RunWorkerCompletedEventHandler onComplete = (s, ec) =>
            {
                var result = ec.Result as List<ConfigCommandResult>;
                string resultMessage = string.Empty;

                if (ec.Cancelled)
                    resultMessage = "Configuration updating process has been cancelled.";

                else if (result != null && result.Any())
                    foreach (var singleResult in result)
                    {
                        if (singleResult.Result.Success && singleResult.Result.Data.StartsWith("OK"))
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
            scm.UploadConfigurationAsync(data, true, allowCancel, onProgress, onComplete);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Execute single configuration uplaod command. </summary>
        /// <param name="data"> Configuration command data carrier object. </param>
        private void UploadSingleConfiguration(ConfigCommandCarrier data)
        {
            SerialCommander scm = new SerialCommander(DataController.SerialPortConnection);

            var awaitMessage = new AwaitInternalMessageEx(InternalMessagesExContainer,
                data.Title, data.Message, data.Icon);

            awaitMessage.KeepFinishedOpen = true;

            RunWorkerCompletedEventHandler onComplete = (s, ec) =>
            {
                var result = ec.Result as CommandResult;

                if (result != null && result.Success && result.Data.StartsWith("OK"))
                    awaitMessage.Message = data.CompleteMessage;
                else
                    awaitMessage.Message = data.FailMessage;

                awaitMessage.InvokeFinsh();
            };

            InternalMessagesExContainer.ShowMessage(awaitMessage);
            scm.UploadSingleConfigurationAsync(data, true, onComplete);
        }

        #endregion CONFIGURATION METHODS

        #region CONSOLE INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on ConsoleClearButtonEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ConsoleClearButtonEx_Click(object sender, RoutedEventArgs e)
        {
            Console = string.Empty;
        }

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
            {
                var textBoxEx = (TextBoxEx)sender;

                SendMessageButtonEx.Focus();

                SendMessage();

                MessageTextBoxEx.Focus();
            }
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

        #region LEDS CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on Led function ButtonEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void LedButtonEx_Click(object sender, RoutedEventArgs e)
        {
            ButtonEx button = (ButtonEx)sender;

            if (button == LedOnButtonEx)
            {
                SendMessage("/led on");
            }
            else if (button == LedOffButtonEx)
            {
                SendMessage("/led off");
            }
            else if (button == LedBrightnessPlusButtonEx)
            {
                SendMessage("/led +");
            }
            else if (button == LedBrightnessMinusButtonEx)
            {
                SendMessage("/led -");
            }
            else if (button == LedFlashButtonEx)
            {
                SendMessage("/led flash");
            }
            else if (button == LedStrobeButtonEx)
            {
                SendMessage("/led strobe");
            }
            else if (button == LedFadeButtonEx)
            {
                SendMessage("/led fade");
            }
            else if (button == LedSmoothButtonEx)
            {
                SendMessage("/led smooth");
            }
            else if (button == LedRedButtonEx)
            {
                SendMessage("/led r 0");
            }
            else if (button == LedRustButtonEx)
            {
                SendMessage("/led r 1");
            }
            else if (button == LedOrangeButtonEx)
            {
                SendMessage("/led r 2");
            }
            else if (button == LedGlodButtonEx)
            {
                SendMessage("/led r 3");
            }
            else if (button == LedYellowButtonEx)
            {
                SendMessage("/led r 4");
            }
            else if (button == LedGreenButtonEx)
            {
                SendMessage("/led g 0");
            }
            else if (button == LedMintButtonEx)
            {
                SendMessage("/led g 1");
            }
            else if (button == LedAquaButtonEx)
            {
                SendMessage("/led g 2");
            }
            else if (button == LedBrightBlueButtonEx)
            {
                SendMessage("/led g 3");
            }
            else if (button == LedTurquoiseButtonEx)
            {
                SendMessage("/led g 4");
            }
            else if (button == LedBlueButtonEx)
            {
                SendMessage("/led b 0");
            }
            else if (button == LedVioletButtonEx)
            {
                SendMessage("/led b 1");
            }
            else if (button == LedOrchidButtonEx)
            {
                SendMessage("/led b 2");
            }
            else if (button == LedPlumButtonEx)
            {
                SendMessage("/led b 3");
            }
            else if (button == LedPinkButtonEx)
            {
                SendMessage("/led b 4");
            }
            else if (button == LedWhiteButtonEx)
            {
                SendMessage("/led w");
            }
            else
            {
                //
            }
        }

        #endregion LEDS CONTROL METHODS

        #region MESSAGING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Send message to serial port device. </summary>
        private void SendMessage()
        {
            if (!string.IsNullOrEmpty(ConsoleMessage) && DataController.SerialPortConnection?.IsConnected == true)
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

                DataController.SerialPortConnection.SendMessage(ConsoleMessage);
                consoleTextBoxEx.ScrollToEnd();
                ConsoleMessage = string.Empty;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Send message to serial port device. </summary>
        /// <param name="message"> Message to send. </param>
        private void SendMessage(string message)
        {
            if (!string.IsNullOrEmpty(message) && DataController.SerialPortConnection?.IsConnected == true)
            {
                DataController.SerialPortConnection.SendMessage(message);
                consoleTextBoxEx.ScrollToEnd();
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

                RequestProcess(e.Message);
            }
        }

        //  --------------------------------------------------------------------------------
        private void RequestProcess(string message)
        {
            if (message.StartsWith("/get weather"))
                UpdateWeatherRequestProcess();
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

        #region PIANO METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Mousr Button Event Arguments. </param>
        private void PianoKey_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;

            if (border != null)
            {
                if (PianoButtonsMap.TryGetValue(border.Name, out int keyIndex))
                {
                    var keyName = PianoNotes.GetNote(keyIndex, PianoShift);
                    UInt16 tone = Convert.ToUInt16(PianoNotes.GetNoteValue(keyName));
                    int duration = CalculateNoteDuration(NoteDuration);

                    PlayTone(tone, duration);

                    if (PianoRecording)
                    {
                        var newNote = new PianoNote(keyName, NoteDuration);
                        Notes.Add(newNote);
                        SelectedNote = newNote;
                    }
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Play piano tone. </summary>
        /// <param name="frequency"> Tone frequency. </param>
        /// <param name="msDuration"> Tone duration. </param>
        /// <param name="volume"> Tone volume. </param>
        public static void PlayTone(UInt16 frequency, int msDuration, UInt16 volume = 16383)
        {
            using (var mStrm = new MemoryStream())
            {
                using (var writer = new BinaryWriter(mStrm))
                {
                    const double tau = 2 * Math.PI;
                    const int formatChunkSize = 16;
                    const int headerSize = 8;
                    const short formatType = 1;
                    const short tracks = 1;
                    const int samplesPerSecond = 44100;
                    const short bitsPerSample = 16;
                    const short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
                    const int bytesPerSecond = samplesPerSecond * frameSize;
                    const int waveSize = 4;
                    var samples = (int)((decimal)samplesPerSecond * msDuration / 1000);
                    int dataChunkSize = samples * frameSize;
                    int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;

                    writer.Write(0x46464952);
                    writer.Write(fileSize);
                    writer.Write(0x45564157);
                    writer.Write(0x20746D66);
                    writer.Write(formatChunkSize);
                    writer.Write(formatType);
                    writer.Write(tracks);
                    writer.Write(samplesPerSecond);
                    writer.Write(bytesPerSecond);
                    writer.Write(frameSize);
                    writer.Write(bitsPerSample);
                    writer.Write(0x61746164);
                    writer.Write(dataChunkSize);

                    double theta = frequency * tau / samplesPerSecond;
                    double amp = volume >> 2;
                    for (int step = 0; step < samples; step++)
                    {
                        writer.Write((short)(amp * Math.Sin(theta * step)));
                    }

                    mStrm.Seek(0, SeekOrigin.Begin);
                    using (var player = new System.Media.SoundPlayer(mStrm))
                    {
                        player.PlaySync();
                    }
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking OpenSong button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OpenSongButtonEx_Click(object sender, RoutedEventArgs e)
        {
            PianoRecording = false;
            
            var imOpenFile = FilesSelectorInternalMessageEx.CreateOpenFileInternalMessageEx(
                InternalMessagesExContainer, "Open Arduino Song File", PackIconKind.MusicNote);

            imOpenFile.InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE");
            imOpenFile.MultipleFiles = false;
            imOpenFile.FilesTypes = new ObservableCollection<InternalMessageFileType>()
            {
                new InternalMessageFileType("Arduino Midi Song", new string[] { "*.ams" }),
                new InternalMessageFileType("All Files", new string[] { "*.*" }),
            };

            imOpenFile.OnClose += (s, fe) =>
            {
                if (fe.Result == InternalMessageResult.Ok && File.Exists(fe.FilePath))
                 {
                    var notesData = File.ReadAllText(fe.FilePath);
                    var deserializedData = JsonConvert.DeserializeObject<List<PianoNote>>(notesData);
                    Notes = new ObservableCollection<PianoNote>(deserializedData);
                }
            };

            InternalMessagesExContainer.ShowMessage(imOpenFile);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking SaveSong button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SaveSongButtonEx_Click(object sender, RoutedEventArgs e)
        {
            PianoRecording = false;

            var imSavenFile = FilesSelectorInternalMessageEx.CreateSaveFileInternalMessageEx(
                InternalMessagesExContainer, "Save Arduino Song File", PackIconKind.MusicNote);

            imSavenFile.InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE");
            imSavenFile.FilesTypes = new ObservableCollection<InternalMessageFileType>()
            {
                new InternalMessageFileType("Arduino Midi Song", new string[] { "*.ams" }),
                new InternalMessageFileType("All Files", new string[] { "*.*" }),
            };

            imSavenFile.OnClose += (s, fe) =>
            {
                if (fe.Result == InternalMessageResult.Ok)
                {
                    string notesData = JsonConvert.SerializeObject(Notes, Formatting.Indented);
                    File.WriteAllText(fe.FilePath, notesData);
                }
            };

            InternalMessagesExContainer.ShowMessage(imSavenFile);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking AddSongNote button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddSongNoteButtonEx_Click(object sender, RoutedEventArgs e)
        {
            string noteValue = PianoNotes.NoteC1;
            int duration = 2;

            if (SelectedNote != null)
            {
                noteValue = SelectedNote.Note;
                duration = SelectedNote.Duration;
            }

            var note = new PianoNote(noteValue, duration);
            Notes.Add(note);
            SelectedNote = note;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking AddSongBreakNote button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddSongBreakNoteButtonEx_Click(object sender, RoutedEventArgs e)
        {
            Notes.Add(PianoNote.LineBreak());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking AddSongPauseNote button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddSongPauseNoteButtonEx_Click(object sender, RoutedEventArgs e)
        {
            Notes.Add(PianoNote.Pause(100));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking RemoveSongNote button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void RemoveSongNoteButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var notes = PianoNotesListViewEx.SelectedItems.Cast<PianoNote>().ToList();

            if (notes != null && notes.Any())
            {
                notes.ForEach(note =>
                {
                    if (Notes.Any(n => n == note))
                        Notes.Remove(note);
                });

                SelectedNote = new PianoNote(PianoNotes.NoteC1, 2);
            }
            else if (SelectedNote != null)
            {
                if (Notes.Any(n => n == SelectedNote))
                    Notes.Remove(SelectedNote);

                SelectedNote = new PianoNote(PianoNotes.NoteC1, 2);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking ClearSong button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void ClearSongButtonEx_Click(object sender, RoutedEventArgs e)
        {
            PianoRecording = false;
            Notes.Clear();
            SelectedNote = new PianoNote(PianoNotes.NoteC1, 2);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking PlayPauseSong button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void PlayPauseSongButtonEx_Click(object sender, RoutedEventArgs e)
        {
            if (Notes.Any())
            {
                var bgPlayer = new BackgroundWorker()
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };

                bgPlayer.DoWork += (s, we) =>
                {
                    var notes = we.Argument as ObservableCollection<PianoNote>;
                    var worker = (BackgroundWorker)s;

                    if (notes != null)
                    {
                        foreach (var note in notes)
                        {
                            if (worker.CancellationPending)
                                return;

                            if (note.IsBreak)
                                continue;

                            else if (note.IsPause)
                            {
                                worker.ReportProgress(note.Duration, note.Note);
                                Thread.Sleep(note.Duration);
                            }

                            else
                            {
                                worker.ReportProgress(CalculateNoteDuration(note.Duration), note.Note);

                                UInt16 tone = Convert.ToUInt16(PianoNotes.GetNoteValue(note.Note));
                                int duration = CalculateNoteDuration(note.Duration);

                                PlayTone(tone, duration);
                                Thread.Sleep(duration);
                            }
                        }
                    }
                };

                var imAwaiter = new AwaitInternalMessageEx(InternalMessagesExContainer, "Playing", "Playing", PackIconKind.Play);
                imAwaiter.AllowCancel = true;
                imAwaiter.OnClose += (s, imc) =>
                {
                    if (imc.Result == InternalMessageResult.Cancel)
                        bgPlayer.CancelAsync();
                };

                bgPlayer.ProgressChanged += (s, wp) =>
                {
                    string note = (string) wp.UserState;
                    imAwaiter.Message = $"Playing note {note}, duration {wp.ProgressPercentage} ms.";
                };

                bgPlayer.RunWorkerCompleted += (s, wc) =>
                {
                    imAwaiter.Close();
                };

                InternalMessagesExContainer.ShowMessage(imAwaiter);
                bgPlayer.RunWorkerAsync(Notes);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking RecordPauseSong button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void RecordPauseSongButtonEx_Click(object sender, RoutedEventArgs e)
        {
            PianoRecording = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking StopRecordSong button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void StopRecordSongButtonEx_Click(object sender, RoutedEventArgs e)
        {
            PianoRecording = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking UploadSongToArduino button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void UploadSongToArduinoButtonEx_Click(object sender, RoutedEventArgs e)
        {
            string message = "/play ";

            foreach (var note in Notes.Where(n => !n.IsBreak))
                message += $"{PianoNotes.GetNoteValue(note.Note)},{note.Duration};";

            SendMessage(message);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Motehod invoked after pressing cursor key when it's over PianoNote ListViewEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void PianoNotesListViewEx_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListViewEx listViewEx = sender as ListViewEx;

            _pianoNoteDragStartPoint = e.GetPosition(null);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Motehod invoked after moving cursor over PianoNote ListViewEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void PianoNotesListViewEx_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListViewEx listViewEx = sender as ListViewEx;
            ListViewItemEx listViewItem = null;
            PianoNote pianoNote = null;
            DataObject dragData = null;

            // Is LMB down and did the mouse move far enough to register a drag?
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(_pianoNoteDragStartPoint.X - e.GetPosition(null).X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(_pianoNoteDragStartPoint.Y - e.GetPosition(null).Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the ListBoxItem object from the object being dragged
                listViewItem = FindParent<ListViewItemEx>((DependencyObject)e.OriginalSource);

                if (listViewItem != null)
                {
                    pianoNote = (PianoNote)listViewEx.ItemContainerGenerator.ItemFromContainer(listViewItem);
                    dragData = new DataObject("piano_note", pianoNote);

                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
            }

            // Is LMB down and did the mouse move far enough to register a drag?
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(_pianoNoteDragStartPoint.X - e.GetPosition(null).X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(_pianoNoteDragStartPoint.Y - e.GetPosition(null).Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the ListBoxItem object from the object being dragged
                listViewItem = FindParent<ListViewItemEx>((DependencyObject)e.OriginalSource);

                if (listViewItem != null)
                {
                    pianoNote = (PianoNote)listViewEx.ItemContainerGenerator.ItemFromContainer(listViewItem);
                    dragData = new DataObject("piano_note", pianoNote);

                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Motehod invoked after releasing cursor key when it's over PianoNote ListViewEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void PianoNotesListViewEx_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ListViewEx listViewEx = sender as ListViewEx;
            List<PianoNote> pianoNotes = null;

            if (listViewEx != null)
            {
                pianoNotes = listViewEx.SelectedItems.Cast<PianoNote>().ToList();
            }

            if (pianoNotes != null && pianoNotes.Any() && pianoNotes.Count > 1)
            {
                var noteValue = pianoNotes
                    .Where(n => !n.IsBreak)
                    .GroupBy(n => n.Note)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault().Key;

                var duration = pianoNotes
                    .Where(n => !n.IsBreak)
                    .GroupBy(n => n.Duration)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault().Key;

                SelectedNote = new PianoNote(noteValue, duration);
            }
            else if (listViewEx?.SelectedItem != null && listViewEx.SelectedItem.GetType() == typeof(PianoNote))
            {
                SelectedNote = listViewEx.SelectedItem as PianoNote;
            }
            else
            {
                SelectedNote = new PianoNote(PianoNotes.NoteC1, 2);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Motehod invoked after dropping item on PianoNote ListViewEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Drag Event Arguments. </param>
        private void PianoNotesListViewEx_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("piano_note"))
                return;

            PianoNote pianoNote = e.Data.GetData("piano_note") as PianoNote;
            // Hit-test needed for rearranging items in the same ListBox
            HitTestResult hit = VisualTreeHelper.HitTest((ListViewEx)sender, e.GetPosition((ListViewEx)sender));
            PianoNote target = (PianoNote)FindParent<ListViewItemEx>(hit.VisualHit)?.DataContext;

            int removeIdx = Notes.IndexOf(pianoNote);
            int targetIdx = Notes.IndexOf(target);

            if (removeIdx < targetIdx)
            {
                Notes.Insert(targetIdx + 1, pianoNote);
                Notes.RemoveAt(removeIdx);
            }
            else
            {
                removeIdx++;
                if (Notes.Count + 1 > removeIdx)
                {
                    Notes.Insert(targetIdx < 0 ? Notes.Count : targetIdx, pianoNote);
                    Notes.RemoveAt(removeIdx);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Motehod invoked after started dragging item on PianoNote ListViewEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Drag Event Arguments. </param>
        private void PianoNotesListViewEx_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("piano_note") || sender == e.Source)
                e.Effects = DragDropEffects.None;
        }

        #endregion PIANO METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data collections. </summary>
        private void SetupDataCollections()
        {
            BaudRatesCollection = new ObservableCollection<int>(SerialPortConnection.BAUD_RATES);
            DevicesCollection = new ObservableCollection<ComPort>(SerialPortConnection.GetComDevices());
            Notes = new ObservableCollection<PianoNote>();

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

        #region TRAY ICON

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on tray icon. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Mouse event arguments. </param>
        private void TrayIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var application = (App)Application.Current;
            var trayWindow = application.Windows.Cast<Window>()
                .FirstOrDefault(w => w.GetType() == typeof(TrayWindow));

            if (trayWindow == null)
                trayWindow = new TrayWindow();
            trayWindow.Show();

            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            trayWindow.Top = desktopWorkingArea.Bottom - (trayWindow.ActualHeight + 8);
            trayWindow.Left = desktopWorkingArea.Right - (trayWindow.ActualWidth + 8);
            trayWindow.Activate();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after double clicking on tray icon. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Event arguments. </param>
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        #endregion TRAY ICON

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Execute command with direct output. </summary>
        /// <param name="command"> Command to execute. </param>
        /// <param name="requiredResponse"> Waiting for particular response. </param>
        /// <param name="timeout"> Time waiting for response in miliseconds. </param>
        /// <returns> Tuple (bool, string) where first value idicates success or fail, 
        /// and second value contains received data. </returns>
        private CommandResult ExecuteCommand(string command, string requiredResponse = null, int timeout = 5000)
        {
            string message = null;
            bool successed = false;

            if (DataController.SerialPortConnection.IsConnected)
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

                DataController.SerialPortConnection.ReceivedMessage += receiver;
                DataController.SerialPortConnection.SendMessage(command);

                while (!cancel && dtStart.AddMilliseconds(timeout) > DateTime.Now)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        if (string.IsNullOrEmpty(requiredResponse))
                            break;
                        else if (message.EndsWith(requiredResponse))
                            break;
                    }
                };

                DataController.SerialPortConnection.ReceivedMessage -= receiver;
            }

            return new CommandResult(successed, message);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate note duration. </summary>
        /// <param name="initialDuration"> Initial duration. </param>
        /// <returns> Calculated note duration. </returns>
        private int CalculateNoteDuration(int initialDuration)
        {
            int note_duration = 1000 / initialDuration;
            int note_pause = (int)(note_duration * 1.30);

            return note_pause;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Find parent UI object. </summary>
        /// <typeparam name="T"> Object type. </typeparam>
        /// <param name="child"> Dependency of child object. </param>
        /// <param name="parentName"> Parent object name. </param>
        /// <returns> Parent UI object. </returns>
        private T FindParent<T>(DependencyObject child, string parentName = null) where T : DependencyObject
        {
            if (child == null) return null;

            T foundParent = null;
            var currentParent = VisualTreeHelper.GetParent(child);

            do
            {
                var frameworkElement = currentParent as FrameworkElement;
                
                if ((frameworkElement.Name == parentName || parentName == null) && frameworkElement is T)
                {
                    foundParent = (T)currentParent;
                    break;
                }

                currentParent = VisualTreeHelper.GetParent(currentParent);

            } while (currentParent != null);

            return foundParent;
        }

        #endregion UTILITY METHODS

        #region WEATHER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on DownloadWeather. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void DownloadWeatherButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var awaitMessage = new AwaitInternalMessageEx(InternalMessagesExContainer,
                "Weather", "Downloading weather...", PackIconKind.WeatherSunny);

            awaitMessage.AllowCancel = true;
            awaitMessage.KeepFinishedOpen = false;

            var bgDwonloader = DataController.DownloadWeatherAsync("Katowice", false, (s, ec) => { awaitMessage.Close(); });

            InternalMessagesExContainer.ShowMessage(awaitMessage);
            bgDwonloader.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on UploadWeather. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UploadWeatherButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var awaitMessage = new AwaitInternalMessageEx(InternalMessagesExContainer,
                "Uploading weather", "Uploading weather data to Arduino...", PackIconKind.WeatherSunny);

            awaitMessage.KeepFinishedOpen = true;

            ProgressChangedEventHandler onProgress = (s, ep) =>
            {
                string progressMessage = (string)ep.UserState;

                if (progressMessage != null)
                    awaitMessage.Message = progressMessage;
            };

            RunWorkerCompletedEventHandler onComplete = (s, ec) =>
            {
                var result = ec.Result as List<ConfigCommandResult>;
                string resultMessage = string.Empty;

                if (ec.Cancelled)
                    resultMessage = "Configuration updating process has been cancelled.";

                else if (result != null && result.Any())
                    foreach (var singleResult in result)
                    {
                        if (singleResult.Result.Success && singleResult.Result.Data.EndsWith("OK"))
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
            DataController.UploadWeatherAsync(true, onProgress, onComplete);
        }

        //  --------------------------------------------------------------------------------
        private void UpdateWeatherRequestProcess()
        {
            var awaitMessage = new AwaitInternalMessageEx(InternalMessagesExContainer,
                "Weather", "Downloading weather...", PackIconKind.WeatherSunny);

            awaitMessage.AllowCancel = true;
            awaitMessage.KeepFinishedOpen = true;

            ProgressChangedEventHandler onUploadProgress = (s, ep) =>
            {
                string progressMessage = (string)ep.UserState;

                if (progressMessage != null)
                    awaitMessage.Message = progressMessage;
            };

            RunWorkerCompletedEventHandler onUploadComplete = (s, ec) =>
            {
                var result = ec.Result as List<ConfigCommandResult>;
                string resultMessage = string.Empty;

                if (ec.Cancelled)
                    resultMessage = "Weather uploading process has been cancelled.";

                else if (result != null && result.Any())
                    foreach (var singleResult in result)
                    {
                        if (singleResult.Result.Success && singleResult.Result.Data.StartsWith("OK"))
                            resultMessage += $"{singleResult.CompleteMessage}{Environment.NewLine}";
                        else
                            resultMessage += $"{singleResult.FailMessage}{Environment.NewLine}";
                    }

                else
                    resultMessage = "Configuration cannot be updated. Please check configuration.";

                awaitMessage.Message = resultMessage;
                awaitMessage.InvokeFinsh();
            };

            var bgUploader = DataController.UploadWeatherAsync(false, onUploadProgress, onUploadComplete);

            RunWorkerCompletedEventHandler onDownloadComplete = (s, ec) =>
            {
                if (ec.Cancelled)
                {
                    awaitMessage.Message = "Weather downloading process has been cancelled.";
                    return;
                };

                awaitMessage.Title = "Uploading weather";
                awaitMessage.Message = "Uploading weather data to Arduino...";

                bgUploader.RunWorkerAsync();
            };

            var bgDwonloader = DataController.DownloadWeatherAsync("Katowice", false, onDownloadComplete);
            
            InternalMessageClose<InternalMessageCloseEventArgs> onIMClose = (s, e) =>
            {
                if (bgDwonloader != null && bgDwonloader.IsBusy)
                    bgDwonloader.CancelAsync();

                else if (bgUploader != null && bgUploader.IsBusy)
                    bgUploader.CancelAsync();
            };

            awaitMessage.OnClose += onIMClose;

            InternalMessagesExContainer.ShowMessage(awaitMessage);
            bgDwonloader.RunWorkerAsync();
        }

        #endregion WEATHER METHODS

        #region WINDOW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after closing window. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Event arguments. </param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataController.SerialPortConnection != null && DataController.SerialPortConnection.IsConnected)
                DataController.SerialPortConnection.Disconnect();

            if (_timeUpdater.IsBusy)
                _timeUpdater.CancelAsync();

            _dataController.Dispose();
            TrayIcon.Dispose();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing keys in window. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Key Event Arguments. </param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (MainTabControlEx.SelectedItem == PianoTabItemEx)
            {
                if (PianoKeyMap.TryGetValue(e.Key, out int keyIndex))
                {
                    var keyName = PianoNotes.GetNote(keyIndex, PianoShift);
                    UInt16 tone = Convert.ToUInt16(PianoNotes.GetNoteValue(keyName));
                    int duration = CalculateNoteDuration(NoteDuration);

                    PlayTone(tone, duration);

                    if (PianoRecording)
                    {
                        var newNote = new PianoNote(keyName, NoteDuration);
                        Notes.Add(newNote);
                        SelectedNote = newNote;
                    }
                }
                else if (e.Key == Key.Space)
                {
                    if (PianoRecording)
                    {
                        var newNote = new PianoNote(PianoNotes.NotePause, 100);
                        Notes.Add(newNote);
                        SelectedNote = newNote;
                    }
                }
            }
        }

        #endregion WINDOW METHODS

    }
}
