using Newtonsoft.Json;

namespace ArduinoConnectWeb.Data.Authentication
{
    public class UserProfile
    {

        //  VARIABLES

        public string Id { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }

        [JsonIgnore]
        private Tokens? Tokens { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        [JsonConstructor]
        public UserProfile(string? id = null)
        {
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString("N").ToLower();
        }

        #endregion CLASS METHODS

    }
}
