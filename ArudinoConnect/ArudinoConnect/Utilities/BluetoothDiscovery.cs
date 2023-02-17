using ArudinoConnect.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace ArudinoConnect.Utilities
{
    public class BluetoothDiscovery : INotifyPropertyChanged, IDisposable
    {

        //  CONST

        // Query for extra properties you want returned.
        private static readonly string[] REQUESTED_PROPERTIES = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private static BluetoothDiscovery _instance;
        private DeviceWatcher _deviceWatcher;
        private ObservableCollection<BluetoothDeviceInfo> _devicesCollection;
        private BluetoothDeviceInfo _selectedDevice;


        //  GETTERS & SETTERS

        public static BluetoothDiscovery Instance
        {
            get
            {
                if (_instance == null || !_instance.IsWorking)
                    _instance = new BluetoothDiscovery();

                return _instance;
            }
        }

        public ObservableCollection<BluetoothDeviceInfo> Devices
        {
            get => _devicesCollection;
            set
            {
                _devicesCollection = value;
                _devicesCollection.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(Devices)); };
                OnPropertyChanged(nameof(Devices));
            }
        }

        public Dispatcher ListViewDispatcher { get; set; }

        public bool IsWorking
        {
            get => _deviceWatcher != null && _deviceWatcher.Status == DeviceWatcherStatus.Started;
        }

        public BluetoothDeviceInfo SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BluetoothDiscovery class constructor. </summary>
        private BluetoothDiscovery()
        {
            //  Create data containers.
            Devices = new ObservableCollection<BluetoothDeviceInfo>();

            //  Create subprocesses.
            CreateWatcher(REQUESTED_PROPERTIES);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            if (IsWorking)
                _deviceWatcher.Stop();
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

        #region WATCHER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create and start Bluetooth watcher. </summary>
        /// <param name="requestedProperties"></param>
        private void CreateWatcher(string[] requestedProperties)
        {
            _deviceWatcher = DeviceInformation.CreateWatcher(
                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                requestedProperties,
                DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices.
            _deviceWatcher.Added += DeviceWatcher_Added;
            _deviceWatcher.Updated += DeviceWatcher_Updated;
            _deviceWatcher.Removed += DeviceWatcher_Removed;

            // EnumerationCompleted and Stopped are optional to implement.
            _deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            _deviceWatcher.Stopped += DeviceWatcher_Stopped;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Start bluetooth devices discovery. </summary>
        public void StartDevicesDiscovery()
        {
            if (IsWorking)
                _deviceWatcher.Stop();

            if (_deviceWatcher == null || _deviceWatcher.Status != DeviceWatcherStatus.Created)
                CreateWatcher(REQUESTED_PROPERTIES);

            _deviceWatcher.Start();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop bluetooth devices discovery. </summary>
        public void StopDevicesDiscovery()
        {
            if (IsWorking)
                _deviceWatcher.Stop();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after adding device. </summary>
        /// <param name="sender"> Device watcher. </param>
        /// <param name="args"> Device Informations. </param>
        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if (ListViewDispatcher != null)
            {
                ListViewDispatcher.Invoke(() =>
                {
                    if (Devices.Any(d => d.DeviceFullId == args.Id))
                        return;

                    Devices.Add(new BluetoothDeviceInfo(args));
                });
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after updating device. </summary>
        /// <param name="sender"> Device watcher. </param>
        /// <param name="args"> Device Informations Update. </param>
        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after removing device. </summary>
        /// <param name="sender"> Device watcher. </param>
        /// <param name="args"> Device Informations Update. </param>
        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after enumeration devices completed. </summary>
        /// <param name="sender"> Device watcher. </param>
        /// <param name="args"> Arguments. </param>
        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after watcher stopped. </summary>
        /// <param name="sender"> Device watcher. </param>
        /// <param name="args"> Arguments. </param>
        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            //
        }

        #endregion WATCHER METHODS

    }
}
