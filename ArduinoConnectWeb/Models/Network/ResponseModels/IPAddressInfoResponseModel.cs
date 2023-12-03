namespace ArduinoConnectWeb.Models.Network.ResponseModels
{
    public class IPAddressInfoResponseModel
    {

        //  VARIABLES

        public string? IPv4 { get; set; }
        public string? IPv6 { get; set; }
        public string? IPv4AddressFamily { get; set; }
        public string? IPv6AddressFamily { get; set; }
        public long? IPv6ScopeId { get; set; }

    }
}
