namespace ArduinoConnectWeb.Models.Serial.ResponseModels
{
    public class ConnectionResponseModel
    {

        //  VARIABLES

        public string? Id { get; set; }
        public int BaudRate { get; set; }
        public bool IsConnected { get; set; }
        public string? Port { get; set; }
        public string? UserId { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ConnectionResponseModel class constructor. </summary>
        public ConnectionResponseModel()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ConnectionResponseModel class constructor. </summary>
        /// <param name="connectionHandler"> Serial port connection hadnler. </param>
        public ConnectionResponseModel(SerialPortHandler connectionHandler)
        {
            Id = connectionHandler.Id;
            BaudRate = connectionHandler.BaudRate;
            IsConnected = connectionHandler.IsConnected;
            Port = connectionHandler.Port;
            UserId = connectionHandler.UserId;
        }

        #endregion CLASS METHODS

    }
}
