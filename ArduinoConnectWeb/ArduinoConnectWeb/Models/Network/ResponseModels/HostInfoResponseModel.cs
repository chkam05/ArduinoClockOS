namespace ArduinoConnectWeb.Models.Network.ResponseModels
{
    public class HostInfoResponseModel
    {

        //  VARIABLES

        public string? HostName { get; set; }
        public List<IPAddressInfoResponseModel>? Addresses { get; set; }

    }
}
