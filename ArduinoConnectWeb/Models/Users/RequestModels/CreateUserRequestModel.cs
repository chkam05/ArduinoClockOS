namespace ArduinoConnectWeb.Models.Users.RequestModels
{
    public class CreateUserRequestModel
    {

        //  VARIABLES

        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? PasswordRepeat { get; set; }
        public UserPermissionLevel? PermissionLevel { get; set; }

    }
}
