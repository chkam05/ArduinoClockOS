namespace ArduinoConnectWeb.Models.Serial.ResponseModels
{
    public class MessageListItemResponseModel
    {

        //  VARIABLES

        public DateTime? DateTime { get; set; }
        public string? Message { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MessageListItemResponseModel class constructor. </summary>
        public MessageListItemResponseModel()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> MessageListItemResponseModel class constructor. </summary>
        /// <param name="messageDataModel"> Message data model. </param>
        public MessageListItemResponseModel(MessageDataModel messageDataModel)
        {
            DateTime = messageDataModel.DateTime;
            Message = messageDataModel.Message;
        }

        #endregion CLASS METHODS

    }
}
