namespace ArduinoConnectWeb.Models.Serial.ResponseModels
{
    public class SendAndReceiveMessageResponseModel
    {

        //  VARIABLES

        public List<MessageListItemResponseModel>? Messages { get; set; }
        public string? Port { get; set; }

    }
}
