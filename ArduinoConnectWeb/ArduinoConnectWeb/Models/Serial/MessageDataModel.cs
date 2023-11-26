namespace ArduinoConnectWeb.Models.Serial
{
    public class MessageDataModel
    {

        //  VARIABLES

        public DateTime DateTime { get; set; }
        public string? Message { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MessageDataModel class constructor. </summary>
        /// <param name="message"> Message. </param>
        /// <param name="dateTime"> Receinved message date time. </param>
        public MessageDataModel(string? message, DateTime dateTime)
        {
            Message = message;
            DateTime = dateTime;
        }

        //  --------------------------------------------------------------------------------
        // <summary> MessageDataModel class constructor. </summary>
        /// <param name="message"> Message. </param>
        public MessageDataModel(string? message)
        {
            Message = message;
            DateTime = DateTime.Now;
        }

        #endregion CLASS METHODS

    }
}
