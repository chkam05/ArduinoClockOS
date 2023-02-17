using ArudinoConnect.Data;
using ArudinoConnect.Utilities;
using chkam05.Tools.ControlsEx.Data;
using chkam05.Tools.ControlsEx;
using chkam05.Tools.ControlsEx.Events;
using chkam05.Tools.ControlsEx.InternalMessages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace ArudinoConnect.InternalMessages
{
    public partial class BluetoothDiscoverIM : StandardInternalMessageEx
    {

        //  CONST

        // Query for extra properties you want returned.
        private static readonly string[] REQUESTED_PROPERTIES = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };


        //  VARIABLES

        private BluetoothDiscovery _btDiscovery;


        //  GETTERS & SETTERS

        public BluetoothDiscovery BTDiscovery
        {
            get => _btDiscovery;
            private set
            {
                _btDiscovery = value;
                OnPropertyChanged(nameof(BTDiscovery));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BluetoothDiscoverIM class constructor. </summary>
        /// <param name="parentContainer"> Parent InternalMessagesEx container. </param>
        public BluetoothDiscoverIM(InternalMessagesExContainer parentContainer) : base(parentContainer)
        {
            OnClose += (s, e) =>
            {

            };

            //  Initialize interface components.
            InitializeComponent();

            //  Initialize modules.
            BTDiscovery = BluetoothDiscovery.Instance;
            BTDiscovery.ListViewDispatcher = ((App)Application.Current).MainWindow.Dispatcher;
            BTDiscovery.StartDevicesDiscovery();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting bluetooth device item. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Selection Changed Event Arguments. </param>
        private void BtDevicesListViewEx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listViewEx = (ListViewEx)sender;
            var btDevice = listViewEx.SelectedItem as Data.BluetoothDeviceInfo;

            if (IsLoadingComplete)
                ManageOkButton(btDevice != null);
        }

        #endregion INTERACTION METHODS

        #region TEMPLATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> When overridden in a derived class,cis invoked whenever 
        /// application code or internal processes call ApplyTemplate. </summary>
        public override void OnApplyTemplate()
        {
            //  Apply Template
            base.OnApplyTemplate();
            ManageOkButton();
        }

        #endregion TEMPLATE METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Manage Ok Button. </summary>
        /// <param name="deviceSelected"> Is device selected. </param>
        private void ManageOkButton(bool deviceSelected = false)
        {
            var button = GetButtonEx("okButton");

            if (button != null)
                button.IsEnabled = deviceSelected;
        }

        #endregion UTILITY METHODS

    }
}
