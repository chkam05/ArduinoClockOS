using ArduinoConnectWeb.Models.Users;

namespace ArduinoConnectWeb.Services.Users
{
    public class UsersServiceConfig
    {

        //  VARIABLES

        public List<UserDataModel>? DefaultUsers { get; set; }
        public List<string>? InitErrorMessages { get; set; }
        public string? UsersStorageFile { get; set; }

    }
}
