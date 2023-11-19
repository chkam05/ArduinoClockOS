namespace ArduinoConnectWeb.Models.Config
{
    public class LaunchSettings
    {

        //  VARIABLES

        public Profile? Profiles { get; set; }

    }

    public class Profile
    {

        //  VARIABLES

        public ArduinoConnectWebProfile? ArduinoConnectWeb { get; set; }

    }

    public class ArduinoConnectWebProfile
    {

        //  VARIABLES

        public string? ApplicationUrl { get; set; }
        public string? CommandName { get; set; }
        public bool DotnetRunMessages { get; set; }
        public bool LaunchBrowser { get; set; }
        public string? LaunchUrl { get; set; }
        public ProfileEnvironmentVariables? EnvironmentVariables { get; set; }

    }

    public class ProfileEnvironmentVariables
    {

        //  VARIABLES

        public string? ASPNETCORE_ENVIRONMENT { get; set; }

    }
}
