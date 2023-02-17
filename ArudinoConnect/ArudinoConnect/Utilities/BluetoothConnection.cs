using ArudinoConnect.Data;
using ArudinoConnect.Windows;
using chkam05.Tools.ControlsEx.InternalMessages;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;

namespace ArudinoConnect.Utilities
{
    public class BluetoothConnection : INotifyPropertyChanged, IDisposable
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private GattDeviceServicesResult _connectionInfo;
        private BluetoothDeviceInfo _deviceInfo;
        private BluetoothLEDevice _device;
        private InternalMessagesExContainer _imContainer;


        //  GETTERS & SETTERS

        public GattDeviceServicesResult ConnectionInfo
        {
            get => _connectionInfo;
            private set
            {
                _connectionInfo = value;
                OnPropertyChanged(nameof(ConnectionInfo));
            }
        }

        public BluetoothDeviceInfo DeviceInfo
        {
            get => _deviceInfo;
            private set
            {
                _deviceInfo = value;
                OnPropertyChanged(nameof(DeviceInfo));
            }
        }

        public BluetoothLEDevice Device
        {
            get => _device;
            private set
            {
                _device = value;
                OnPropertyChanged(nameof(Device));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BluetoothConnection class constructor. </summary>
        /// <param name="bluetoothDeviceInfo"> Bluetooth device. </param>
        public BluetoothConnection(InternalMessagesExContainer internalMessagesExContainer)
        {
            _imContainer = internalMessagesExContainer;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Disconnect();
        }

        #endregion CLASS METHODS

        #region CONNECTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Connect to bluetooth device. </summary>
        /// <param name="bluetoothDeviceInfo"> Bluetooth device info. </param>
        public void Connect(BluetoothDeviceInfo bluetoothDeviceInfo)
        {
            DeviceInfo = bluetoothDeviceInfo;
            Pair();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Pair with bluetooth device.</summary>
        private void Pair()
        {
            var title = "Bluetooth pairing";
            var awaitMessage = $"Pairing with device \"{DeviceInfo?.DeviceName}\" [{DeviceInfo?.DeviceId}]";

            var imAwait = new AwaitInternalMessageEx(_imContainer, title, awaitMessage, PackIconKind.Bluetooth);

            var bgWorker = new BackgroundWorker();

            bgWorker.DoWork += (s, e) =>
            {
                PairWork();
                GetConnectionInfoWork();
            };

            bgWorker.RunWorkerCompleted += (s, e) =>
            {
                imAwait.Close();

                if (Device == null || ConnectionInfo?.Status != GattCommunicationStatus.Success)
                {
                    string postfix = ConnectionInfo?.Status.ToString();

                    var imFailed = new InternalMessageEx(
                        _imContainer, title, $"Pairing with device \"{DeviceInfo?.DeviceName}\" failed. {postfix}", PackIconKind.Bluetooth);

                    _imContainer.ShowMessage(imFailed);
                }
                else
                {
                    var imSuccess = new InternalMessageEx(
                        _imContainer, title, $"Pairing with device \"{DeviceInfo?.DeviceName}\" complete." 
                            + Environment.NewLine + "You should now see additional COM ports in list on the right.", PackIconKind.Bluetooth);

                    _imContainer.ShowMessage(imSuccess);
                }
            };

            _imContainer.ShowMessage(imAwait);
            bgWorker.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Pari (connect) bluetooth device. </summary>
        private void PairWork()
        {
            Disconnect();

            if (DeviceInfo != null)
            {
                var device = BluetoothLEDevice.FromIdAsync(DeviceInfo.DeviceFullId);
                Thread.Sleep(1000);
                Device = device.GetResults();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get connection info. </summary>
        /// <returns> True - success; False - otherwise. </returns>
        private bool GetConnectionInfoWork()
        {
            if (Device != null)
            {
                var connectionInfo = Device.GetGattServicesAsync();
                Thread.Sleep(2000);
                ConnectionInfo = connectionInfo.GetResults();

                return ConnectionInfo.Status == GattCommunicationStatus.Success;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        public void Disconnect()
        {
            ConnectionInfo = null;
            
            if (Device != null)
            {
                Device.Dispose();
                Device = null;
            }
        }

        #endregion CONNECTION METHODS

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
