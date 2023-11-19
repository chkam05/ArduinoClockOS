namespace ArduinoConnectWeb.Models.Auth
{
    public class RequestLogoutModel
    {

        //  VARIABLES

        public string? UserName { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool AllSessions { get; set; }

    }
}
