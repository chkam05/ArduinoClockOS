namespace ArduinoConnectWeb.Services.Auth
{
    public class AuthServiceConfig
    {

        //  VARIABLES

        public string? JwtAudience { get; set; }
        public string? JwtIssuer { get; set; }
        public string JwtKey { get; set; }
        public TimeSpan JwtAccessTokenValidityTime { get; set; }
        public TimeSpan JwtRefreshTokenValidityTime { get; set; }
        public int JwtRefreshTokenSize { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AuthServiceConfig class constructor. </summary>
        /// <param name="jwtKey"> Jwt key. </param>
        public AuthServiceConfig(string jwtKey)
        {
            JwtKey = jwtKey;
        }

        #endregion CLASS METHODS

    }
}
