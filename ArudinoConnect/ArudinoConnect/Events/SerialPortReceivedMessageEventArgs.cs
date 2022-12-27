using ArudinoConnect.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArudinoConnect.Events
{
    public class SerialPortReceivedMessageEventArgs : EventArgs
    {

        //  VARIABLES

        public string Message { get; set; }
        public ReceivedMessageState State { get; set; }


        //  GETTERS & SETTERS

        public bool HasMessage
        {
            get => !string.IsNullOrWhiteSpace(Message);
        }

        public int Length
        {
            get => !string.IsNullOrWhiteSpace(Message) ? Message.Length : 0;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SerialPortReceivedMessageEventArgs class constructor. </summary>
        /// <param name="message"> Received message. </param>
        /// <param name="state"> Received message state. </param>
        public SerialPortReceivedMessageEventArgs(string message, ReceivedMessageState state) : base()
        {
            Message = message;
            State = state;
        }

        #endregion CLASS METHODS

    }
}
