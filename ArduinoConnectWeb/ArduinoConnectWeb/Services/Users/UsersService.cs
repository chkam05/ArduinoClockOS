using ArduinoConnectWeb.DataContexts;
using ArduinoConnectWeb.Models.Base;
using ArduinoConnectWeb.Models.Users;
using Newtonsoft.Json;
using System;

namespace ArduinoConnectWeb.Services.Users
{
    public class UsersService : IUsersService
    {

        //  VARIABLES

        private readonly ILogger<UsersService> _logger;
        private readonly UsersDataContext _usersDataContext;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UsersService class constructor. </summary>
        /// <param name="configuration"> Interface of application configuration properties. </param>
        /// <param name="logger"> Application logger. </param>
        public UsersService(IConfiguration configuration, ILogger<UsersService> logger)
        {
            _logger = logger;

            _usersDataContext = new UsersDataContext();
            _usersDataContext.LoadData();

            if (!_usersDataContext.HasUsers())
                InitializeDefaultUsers(configuration);
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        public void CreateUser()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by identifier. </summary>
        /// <param name="id"> Identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<ResponseUserModel>> GetUserById(string id)
        {
            return await Task.Run(() =>
            {
                var user = _usersDataContext.Users.FirstOrDefault(u => u.Id == id.ToUpper());

                if (user is null)
                    return new ResponseBaseModel<ResponseUserModel>($"No user with the given \"id\" id was found.");

                return new ResponseBaseModel<ResponseUserModel>(GetResponseUserModel(user));
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by user name. </summary>
        /// <param name="userName"> User name. </param>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<ResponseUserModel>> GetUserByUserName(string userName)
        {
            return await Task.Run(() =>
            {
                var user = _usersDataContext.Users.FirstOrDefault(u => u.UserName == userName);

                if (user is null)
                    return new ResponseBaseModel<ResponseUserModel>($"No user with the given \"userName\" name was found.");

                return new ResponseBaseModel<ResponseUserModel>(GetResponseUserModel(user));
            });
        }
        
        //  --------------------------------------------------------------------------------
        /// <summary> Get users list. </summary>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<ResponseUsersListModel>> GetUsersList()
        {
            return await Task.Run(() =>
            {
                var responseData = new ResponseUsersListModel()
                {
                    Users = _usersDataContext.Users.Select(u => new ResponseUsersListItemModel()
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                    }).ToList()
                };

                return new ResponseBaseModel<ResponseUsersListModel>(responseData);
            });
        }

        //  --------------------------------------------------------------------------------
        public void RemoveUser()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        public void UpdateUser()
        {
            //
        }

        #endregion INTERACTION METHODS

        #region RESPONSE CREATE METHODS

        //  --------------------------------------------------------------------------------
        private ResponseUserModel GetResponseUserModel(UserDataModel user)
        {
            return new ResponseUserModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                PermissionLevel = user.PermissionLevel,
                CreatedAt = user.CreatedAt,
                LastModifiedAt = user.LastModifiedAt
            };
        }

        #endregion RESPONSE CREATE METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Initialize default users. </summary>
        /// <param name="configuration"> Interface of application configuration properties. </param>
        private void InitializeDefaultUsers(IConfiguration configuration)
        {
            _logger.LogInformation($"{nameof(UsersService)}: Initializing default users.");

            try
            {
                var usersConfig = configuration.GetSection("Users");

                if (usersConfig != null)
                {
                    foreach (var userConfig in usersConfig.GetChildren())
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

                            _usersDataContext.AddUser(user);
                            _logger.LogInformation($"{nameof(UsersService)}: Adding default user \"{user.UserName}\".");
                        }
                        catch (Exception exc)
                        {
                            _logger.LogWarning($"{nameof(UsersService)}: An error occurred while adding user ({exc.Message}).");
                        }
                    }
                }
            }
            catch (Exception)
            {
                _logger.LogWarning($"{nameof(UsersService)}: Default users configuration has not been found in appsettings.");
            }
            
            if (!_usersDataContext.HasUsers())
            {
                var defaultUser = new UserDataModel(null,
                    "Administrator",
                    "fd74bdd901857b89f5737e5352a2a8a2d1f000aa4bed4aee47c95afaa37d0f99", // P@55w0rd
                    UserPermissionLevel.Administrator);

                _usersDataContext.AddUser(defaultUser);
                _logger.LogInformation($"{nameof(UsersService)}: Adding default user \"{defaultUser.UserName}\".");
            }
        }

        #endregion SETUP METHODS

    }
}
