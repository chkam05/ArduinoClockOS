namespace ArduinoConnectWeb.Models.Users
{
    public class RequestUserCreateModel
    {

        //  VARIABLES

        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? PasswordRepeat { get; set; }
        public UserPermissionLevel? PermissionLevel { get; set; }

    }
}
