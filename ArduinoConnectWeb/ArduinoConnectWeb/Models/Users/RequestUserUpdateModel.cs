namespace ArduinoConnectWeb.Models.Users
{
    public class RequestUserUpdateModel
    {

        //  VARIABLES

        public string Id { get; set; }
        public string NewUserName { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordRepeat { get; set; }
        public UserPermissionLevel NewPermissionLevel { get; set; }

    }
}
