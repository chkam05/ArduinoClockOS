namespace ArduinoConnectWeb.Models.Serial.RequestModels
{
    public class SendAndReceiveMessageRequestModel
    {

        //  VARIABLES

        public string? Message { get; set; }
        public TimeSpan? TimeOut { get; set; }

    }
}
