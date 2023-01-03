using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArudinoConnect.Data
{
    public class WeatherTreeItem : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _name;
        private ObservableCollection<WeatherTreeItem> _subitems;
        private string _value;


        //  GETTERS & SETTERS

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<WeatherTreeItem> Subitems
        {
            get => _subitems;
            set
            {
                _subitems = value;
                _subitems.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(Subitems)); };
                OnPropertyChanged(nameof(Subitems));
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> WeatherTreeItem class constructor. </summary>
        /// <param name="name"> Item name. </param>
        /// <param name="value"> Item value. </param>
        public WeatherTreeItem(string name, string value = null)
        {
            if (Subitems == null)
                Subitems = new ObservableCollection<WeatherTreeItem>();

            Name = name;
            Value = value;
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
