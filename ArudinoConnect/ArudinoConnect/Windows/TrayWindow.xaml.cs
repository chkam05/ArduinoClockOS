using ArudinoConnect.Data;
using ArudinoConnect.Utilities;
using chkam05.Tools.ControlsEx;
using chkam05.Tools.ControlsEx.InternalMessages;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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
    public partial class TrayWindow : Window, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private DataController _dataController;


        //  GETTERS & SETTERS

        public DataController DataController
        {
            get => _dataController;
            private set
            {
                _dataController = value;
                OnPropertyChanged(nameof(DataController));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> TrayWindow class constructor. </summary>
        public TrayWindow()
        {
            DataController = DataController.Instance;

            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region BASE INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on ShowAppButtonEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ShowAppButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var application = (App)Application.Current;
            var mainWindow = application.MainWindow;

            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Activate();

            this.Close();
        }
        
        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on CloseAppButtonEx. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void CloseAppButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var application = (App)Application.Current;
            application.Shutdown();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting item in hourly weather list. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Selection changed event arguments. </param>
        private void WeahterListViewEx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listViewEx = (ListViewEx)sender;
            
            if (listViewEx.SelectedItem != null)
                listViewEx.SelectedItem = null;
        }

        #endregion BASE INTERACTION METHODS

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

        #region WEATHER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on WeatherRefresh button. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void WeatherRefreshButtonEx_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)((App)Application.Current).MainWindow;
            var imContainer = mainWindow?.InternalMessagesExContainer;

            var awaitMessage = new AwaitInternalMessageEx(
                    imContainer, "Weather", "Downloading weather...", PackIconKind.WeatherSunny);

            awaitMessage.AllowCancel = true;
            awaitMessage.KeepFinishedOpen = false;

            var bgDwonloader = DataController.DownloadWeatherAsync("Katowice", false, (s, ec) =>
            {
                awaitMessage.Close();
                WeatherRefreshButtonEx.IsEnabled = true;
            });

            WeatherRefreshButtonEx.IsEnabled = false;
            imContainer.ShowMessage(awaitMessage);
            bgDwonloader.RunWorkerAsync();
        }

        #endregion WEATHER INTERACTION METHODS

        #region WINDOW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking close window. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Event arguments. </param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after moving cursor outisde window. </summary>
        /// <param name="sender"> Object from which method has been invoked. </param>
        /// <param name="e"> Mouse event arguments. </param>
        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch
            {
                //  Ignore this.
            }
        }

        #endregion WINDOW METHODS

    }
}
