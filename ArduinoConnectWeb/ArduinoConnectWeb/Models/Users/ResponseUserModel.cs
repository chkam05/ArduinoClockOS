namespace ArduinoConnectWeb.Models.Users
{
    public class ResponseUserModel
    {

        //  VARIABLES

        public string? Id { get; set; }
        public string? UserName { get; set; }
        public UserPermissionLevel PermissionLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }

    }
}
