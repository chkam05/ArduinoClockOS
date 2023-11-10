namespace ArduinoConnectWeb.Data.Authentication
{
    public class UserProfilesContext
    {

        //  VARIABLES

        private static List<UserProfile> _userProfiles = new List<UserProfile>();
        private static object _userProfilesLock = new object();

        public UserProfilesContext()
        {
            //
        }

    }
}
