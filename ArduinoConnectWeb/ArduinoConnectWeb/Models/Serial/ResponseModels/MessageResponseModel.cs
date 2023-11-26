namespace ArduinoConnectWeb.Models.Serial.ResponseModels
{
    public class MessageResponseModel
    {

        //  VARIABLES

        public DateTime? DateTime { get; set; }
        public string? Message { get; set; }
        public string? Port { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MessageResponseModel class constructor. </summary>
        public MessageResponseModel()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> MessageResponseModel class constructor. </summary>
        /// <param name="message"> Message data model. </param>
        /// <param name="port"> Serial port. </param>
        public MessageResponseModel(MessageDataModel? message, string? port)
        {
            DateTime = message?.DateTime;
            Message = message?.Message;
            Port = port;
        }

        #endregion CLASS METHODS

    }
}
