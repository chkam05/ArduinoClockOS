namespace ArduinoConnectWeb.Models.Auth.ResponseModels
{
    public class SessionListItemResponseModel
    {

        //  VARIABLES

        public string? UserId { get; set; }
        public DateTime AccessTokenValidityTime { get; set; }
        public DateTime RefreshTokenValidityTime { get; set; }
        public DateTime SessionStart { get; set; }

    }
}
