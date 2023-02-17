using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace ArudinoConnect.Data
{
    public class BluetoothDeviceInfo : INotifyPropertyChanged
    {

        //  CONST

        private static readonly string DEVICE_ID_PREFIX = "BluetoothLE#BluetoothLE";


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private DeviceInformation _deviceInformation;


        //  GETTERS & SETTERS

        public DeviceInformation DeviceInformation
        {
            get => _deviceInformation;
            private set
            {
                _deviceInformation = value;
                OnPropertyChanged(nameof(DeviceInformation));
                OnPropertyChanged(nameof(DeviceFullId));
                OnPropertyChanged(nameof(DeviceId));
                OnPropertyChanged(nameof(DeviceName));
            }
        }

        public string DeviceFullId
        {
            get => _deviceInformation.Id;
        }

        public string DeviceId
        {
            get => _deviceInformation.Id.Replace(DEVICE_ID_PREFIX, "");
        }

        public string DeviceName
        {
            get => _deviceInformation.Name;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BluetoothDevice class constructor. </summary>
        /// <param name="deviceInformation"> Device informations. </param>
        public BluetoothDeviceInfo(DeviceInformation deviceInformation)
        {
            DeviceInformation = deviceInformation;
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
