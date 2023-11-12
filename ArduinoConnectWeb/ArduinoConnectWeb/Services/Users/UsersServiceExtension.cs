using ArduinoConnectWeb.DataContexts;
using ArduinoConnectWeb.Models.Users;

namespace ArduinoConnectWeb.Services.Users
{
    public static class UsersServiceExtension
    {

        //  METHODS

        #region REGISTRATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Register and configure users service.  </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </param>
        public static IServiceCollection RegisterUsersService(this IServiceCollection services, IConfiguration configuration)
        {
            //  Register users service configuration.
            services.RegisterConfiguration(configuration);

            //  Register users service.
            services.AddSingleton<IUsersService, UsersService>();

            return services;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Register users service configuration. </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </param>
        private static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultUsers = GetDefaultUsers(configuration, out List<string> initializationErrors);
            var usersStorageFile = configuration["Storage:UsersStorageFile"];

            //  Initialize configuration.
            var config = new UsersServiceConfig()
            {
                DefaultUsers = defaultUsers,
                InitErrorMessages = initializationErrors,
                UsersStorageFile = usersStorageFile
            };

            //  Register users service configuration.
            services.AddSingleton(config);

            return services;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Read default users from configuration. </summary>
        /// <param name="configuration"> Application configuration. </param>
        /// <param name="initializationErrors"> Initialization errors. </param>
        /// <returns> Default users. </returns>
        private static List<UserDataModel> GetDefaultUsers(IConfiguration configuration, out List<string> initializationErrors)
        {
            initializationErrors = new List<string>();

            var usersConfig = configuration.GetSection("Users")?.GetChildren();
            var usersResult = new List<UserDataModel>();

            //  Read default users from appsettings.json.
            if (usersConfig?.Any() ?? false)
            {
                foreach (var userConfig in usersConfig)
                {
                    try
                    {
                        var userName = userConfig.GetValue<string>("UserName");
                        var passwordHash = userConfig.GetValue<string>("PasswordHash");
                        var permissionLevelIndex = userConfig.GetValue<int>("PermissionLevel");
                        var permissionLevel = Enum.IsDefined(typeof(UserPermissionLevel), permissionLevelIndex)
                            ? (UserPermissionLevel)permissionLevelIndex
                            : UserPermissionLevel.Guest;

                        if (string.IsNullOrEmpty(userName))
                            throw new ArgumentException("Invalid default user configuration: UserName cannot be empty.");

                        if (string.IsNullOrEmpty(passwordHash))
                            throw new ArgumentException("Invalid default user configuration: PasswordHash (SHA256) cannot be empty.");

                        var user = new UserDataModel(null, userName, passwordHash, permissionLevel);

                        if (usersResult.Any(u => u.Equals(user)))
                            throw new ArgumentException($"User \"{user.UserName}\" already exists.");

                        usersResult.Add(user);
                    }
                    catch (Exception exc)
                    {
                        initializationErrors.Add(exc.Message);
                    }
                }
            }

            //  Create default user if users have not been defined in appsettings.json.
            if (!usersResult.Any())
            {
                usersResult.Add(
                    new UserDataModel(
                        null,
                        "Administrator",
                        "fd74bdd901857b89f5737e5352a2a8a2d1f000aa4bed4aee47c95afaa37d0f99", // P@55w0rd
                        UserPermissionLevel.Administrator));
            }

            return usersResult;
        }

        #endregion REGISTRATION METHODS

    }
}
