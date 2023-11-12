using ArduinoConnectWeb.DataContexts;
using ArduinoConnectWeb.Services.Users;

namespace ArduinoConnectWeb.Services.Auth
{
    public class AuthService : IAuthService
    {

        //  VARIABLES

        private readonly AuthServiceConfig _config;
        private readonly ILogger<AuthService> _logger;
        private readonly IUsersService _usersService;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AuthService class constructor. </summary>
        /// <param name="config"> Users service config. </param>
        /// <param name="logger"> Application logger. </param>
        public AuthService(AuthServiceConfig config, ILogger<AuthService> logger, IUsersService usersService)
        {
            _config = config;
            _logger = logger;
            _usersService = usersService;

            //_usersDataContext = new UsersDataContext(_config.UsersStorageFile);
            //_usersDataContext.LoadData();
        }

        #endregion CLASS METHODS

    }
}
