namespace ArduinoConnectWeb.Models.Auth
{
    public class ResponseSessionsListModel
    {

        //  VARIABLES

        public List<ResponseSessionsListItemModel>? CurrentSessions { get; set; }

    }

    public class ResponseSessionsListItemModel
    {

        //  VARIABLES

        public string? UserId { get; set; }
        public DateTime AccessTokenValidityTime { get; set; }
        public DateTime RefreshTokenValidityTime { get; set; }
        public DateTime SessionStart { get; set; }

    }

}
