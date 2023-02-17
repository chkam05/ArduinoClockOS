using ArudinoConnect.Static;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArudinoConnect.Data
{
    public class PianoNote : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _id;
        private string _note;
        private int _duration;


        //  GETTERS & SETTERS

        public string Id
        {
            get => _id;
            private set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Note
        {
            get => _note;
            set
            {
                _note = value;
                OnPropertyChanged(nameof(Note));
                OnPropertyChanged(nameof(IsPause));
                OnPropertyChanged(nameof(IsBreak));
            }
        }

        public int Duration
        {
            get => _duration;
            set
            {
                _duration = Math.Max(0, value);
                OnPropertyChanged(nameof(Duration));
            }
        }

        public bool IsPause
        {
            get => Note == PianoNotes.NotePause;
        }

        public bool IsBreak
        {
            get => string.IsNullOrEmpty(Note);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PianoNote class constructor. </summary>
        public PianoNote()
        {
            CreateId();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> PianoNote class constructor. </summary>
        /// <param name="note"> Note. </param>
        /// <param name="duration"> Duration. </param>
        public PianoNote(string note, int duration)
        {
            CreateId();
            Note = note;
            Duration = duration;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Creates PianoNote pause. </summary>
        /// <param name="duration"> Pause duration. </param>
        /// <returns> PianoNote pause. </returns>
        public static PianoNote Pause(int duration)
        {
            return new PianoNote(PianoNotes.NotePause, duration);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Creates PianoNote line break. </summary>
        /// <returns> PianoNote line break. </returns>
        public static PianoNote LineBreak()
        {
            return new PianoNote(null, 0);
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

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create Id for note. </summary>
        private void CreateId()
        {
            Id = Guid.NewGuid().ToString("N").ToLower();
        }

        #endregion UTILITY METHODS

    }
}
