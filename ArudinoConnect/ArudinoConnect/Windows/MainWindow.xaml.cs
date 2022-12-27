using ArudinoConnect.Data;
using ArudinoConnect.Events;
using ArudinoConnect.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ObservableCollection<int> _baudRatesCollection;
        private ObservableCollection<ComPort> _devicesCollection;
        private int _baudRate;
        private ComPort _selectedDevice;
        private SerialPortConnection _serialPortConnection;

        private string _console;
        private string _consoleMessage;


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
                Console += $"{value}{Environment.NewLine}";
                OnPropertyChanged(nameof(ConsoleMessage));
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

            SetupDataCollections();
            InitializeComponent();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Shutting down application - disposing objects. </summary>
        public void Dispose()
        {
            if (SerialPortConnection != null && SerialPortConnection.IsConnected)
                SerialPortConnection.Disconnect();
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

        #region MESSAGING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Send message to serial port device. </summary>
        private void SendMessage()
        {
            SerialPortConnection.SendMessage(ConsoleMessage);
            Console += $"{ConsoleMessage}{Environment.NewLine}";
            consoleTextBoxEx.ScrollToEnd();
            ConsoleMessage = string.Empty;
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

            if (BaudRatesCollection != null && BaudRatesCollection.Any())
                BaudRate = SerialPortConnection.DEFAULT_BAUD_RATE;

            if (DevicesCollection != null && DevicesCollection.Any())
                SelectedDevice = DevicesCollection.First();
        }

        #endregion SETUP METHODS

    }
}
