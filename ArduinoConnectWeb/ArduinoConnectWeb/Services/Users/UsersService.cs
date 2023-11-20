using ArduinoConnectWeb.DataContexts;
using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base;
using ArduinoConnectWeb.Models.Users;
using ArduinoConnectWeb.Utilities;
using Newtonsoft.Json;
using System;

namespace ArduinoConnectWeb.Services.Users
{
    public class UsersService : IUsersService
    {

        //  VARIABLES

        private readonly UsersServiceConfig _config;
        private readonly ILogger<UsersService> _logger;
        private readonly UsersDataContext _usersDataContext;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UsersService class constructor. </summary>
        /// <param name="config"> Users service config. </param>
        /// <param name="logger"> Application logger. </param>
        public UsersService(UsersServiceConfig config, ILogger<UsersService> logger)
        {
            _config = config;
            _logger = logger;

            _usersDataContext = new UsersDataContext(_config.UsersStorageFile);
            _usersDataContext.LoadData();

            if (!_usersDataContext.HasUsers())
                InitializeDefaultUsers();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create new user. </summary>
        /// <param name="requestUserCreateModel"> Request user create model. </param>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<ResponseUserModel>> CreateUser(RequestUserCreateModel requestUserCreateModel)
        {
            return await Task.Run(() =>
            {
                if (requestUserCreateModel == null)
                    return new ResponseBaseModel<ResponseUserModel>(
                        $"No input data has been entered.");

                if (string.IsNullOrEmpty(requestUserCreateModel.UserName))
                    return new ResponseBaseModel<ResponseUserModel>(
                        $"{nameof(requestUserCreateModel.UserName)} cannot be empty.");

                if (string.IsNullOrEmpty(requestUserCreateModel.Password))
                    return new ResponseBaseModel<ResponseUserModel>(
                        $"{nameof(requestUserCreateModel.Password)} cannot be empty.");

                if (requestUserCreateModel.Password != requestUserCreateModel.PasswordRepeat)
                    return new ResponseBaseModel<ResponseUserModel>(
                        $"{nameof(requestUserCreateModel.Password)} and {nameof(requestUserCreateModel.PasswordRepeat)} does not match.");

                if (_usersDataContext.Users.Any(u => u.UserName.ToLower() == requestUserCreateModel.UserName.ToLower()))
                    return new ResponseBaseModel<ResponseUserModel>(
                        $"User with {nameof(requestUserCreateModel.UserName)} {requestUserCreateModel.UserName} already exists.");

                var newUser = new UserDataModel(
                    null,
                    requestUserCreateModel.UserName,
                    SecurityUtilities.ComputeSha256Hash(requestUserCreateModel.Password),
                    requestUserCreateModel.PermissionLevel);

                try
                {
                    _usersDataContext.AddUser(newUser);
                    SaveDataContext();
                }
                catch (Exception exc)
                {
                    return new ResponseBaseModel<ResponseUserModel>(exc.Message);
                }

                return new ResponseBaseModel<ResponseUserModel>(GetResponseUserModel(newUser));
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by identifier. </summary>
        /// <param name="id"> User identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<ResponseUserModel>> GetUserById(string? id)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(id))
                    return new ResponseBaseModel<ResponseUserModel>($"{nameof(id)} cannot be empty.");

                var user = _usersDataContext.Users.FirstOrDefault(u => u.Id == id.ToUpper());

                if (user is null)
                    return new ResponseBaseModel<ResponseUserModel>(
                        $"No user with the given \"id\" id was found.");

                return new ResponseBaseModel<ResponseUserModel>(GetResponseUserModel(user));
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by user name. </summary>
        /// <param name="userName"> User name. </param>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<ResponseUserModel>> GetUserByUserName(string? userName)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(userName))
                    return new ResponseBaseModel<ResponseUserModel>($"{nameof(userName)} cannot be empty.");

                var user = _usersDataContext.Users.FirstOrDefault(u => u.UserName == userName);

                if (user is null)
                    return new ResponseBaseModel<ResponseUserModel>(
                        $"No user with the given \"userName\" name was found.");

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
        /// <summary> Remove user by id. </summary>
        /// <param name="id"> User identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<string>> RemoveUser(string? id)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(id))
                    return new ResponseBaseModel<string>(errorMessage: $"{nameof(id)} cannot be empty.");

                var user = _usersDataContext.Users.FirstOrDefault(u => u.Id == id.ToUpper());

                if (user is null)
                    return new ResponseBaseModel<string>(
                        errorMessage: $"No user with the given \"id\" id was found.");

                try
                {
                    _usersDataContext.RemoveUser(user);
                    SaveDataContext();
                }
                catch (Exception exc)
                {
                    return new ResponseBaseModel<string>(exc.Message);
                }

                return new ResponseBaseModel<string>(content: $"User deleted successfully.");
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update user. </summary>
        /// <param name="id"> User identifier. </param>
        /// <param name="requestUserUpdateModel"></param>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<ResponseUserModel>> UpdateUser(string? id, RequestUserUpdateModel requestUserUpdateModel)
        {
            return await Task.Run(() =>
            {
                var anyChange = false;

                if (string.IsNullOrEmpty(id))
                    return new ResponseBaseModel<ResponseUserModel>($"{nameof(id)} cannot be empty.");

                var user = _usersDataContext.Users.FirstOrDefault(u => u.Id == id.ToUpper())?.CloneWithType();

                if (user is null)
                    return new ResponseBaseModel<ResponseUserModel>($"No user with the given \"{id}\" id was found.");

                if (!string.IsNullOrEmpty(requestUserUpdateModel.NewUserName))
                {
                    if (_usersDataContext.Users.Any(u => u.UserName.ToLower() == requestUserUpdateModel.NewUserName.ToLower()))
                        return new ResponseBaseModel<ResponseUserModel>(
                            $"User with {nameof(requestUserUpdateModel.NewUserName)} {requestUserUpdateModel.NewUserName} already exists.");

                    user.UserName = requestUserUpdateModel.NewUserName;
                    anyChange = true;
                }

                if (!string.IsNullOrEmpty(requestUserUpdateModel.NewPassword))
                {
                    if (string.IsNullOrEmpty(requestUserUpdateModel.NewPasswordRepeat))
                        return new ResponseBaseModel<ResponseUserModel>(
                            $"{nameof(requestUserUpdateModel.NewPassword)} and {nameof(requestUserUpdateModel.NewPasswordRepeat)} does not match.");

                    user.PasswordHash = SecurityUtilities.ComputeSha256Hash(requestUserUpdateModel.NewPassword);
                    anyChange = true;
                }

                if (requestUserUpdateModel.NewPermissionLevel.HasValue)
                {
                    user.PermissionLevel = requestUserUpdateModel.NewPermissionLevel.Value;
                    anyChange = true;
                }

                if (anyChange)
                {
                    user.UpdateLastModifiedAt();

                    try
                    {
                        _usersDataContext.UpdateUser(user);
                        SaveDataContext();
                    }
                    catch (Exception exc)
                    {
                        return new ResponseBaseModel<ResponseUserModel>(exc.Message);
                    }
                }

                return new ResponseBaseModel<ResponseUserModel>(GetResponseUserModel(user));
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate user. </summary>
        /// <param name="userName"> User name. </param>
        /// <param name="password"> Password. </param>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<UserDataModel>> ValidateUser(string? userName, string? password)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(userName))
                    return new ResponseBaseModel<UserDataModel>("User name can not be empty.");

                if (string.IsNullOrEmpty(password))
                    return new ResponseBaseModel<UserDataModel>("Password can not be empty.");

                var user = _usersDataContext.Users.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());

                if (user is null)
                    return new ResponseBaseModel<UserDataModel>("Invalid user name.");

                if (user.PasswordHash != SecurityUtilities.ComputeSha256Hash(password))
                    return new ResponseBaseModel<UserDataModel>("Invalid password.");

                return new ResponseBaseModel<UserDataModel>(user);
            });
        }

        #endregion INTERACTION METHODS

        #region RESPONSE CREATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get response user model. </summary>
        /// <param name="user"> User data model. </param>
        /// <returns> Response user model. </returns>
        private ResponseUserModel GetResponseUserModel(UserDataModel user)
        {
            return new ResponseUserModel(
                user.Id,
                user.UserName,
                user.PermissionLevel,
                user.CreatedAt,
                user.LastModifiedAt);
        }

        #endregion RESPONSE CREATE METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Initialize default users. </summary>
        private void InitializeDefaultUsers()
        {
            _logger.LogInformation($"{nameof(UsersService)}: Initializing default users.");

            //  Log initialization errors.

            if (_config.InitErrorMessages?.Any() ?? false)
            {
                foreach (var errorMessage in _config.InitErrorMessages)
                {
                    _logger.LogWarning($"{nameof(UsersService)}: An error occurred while adding user ({errorMessage}).");
                }

                _config.InitErrorMessages = null;
            }

            //  Load default users.

            if (_config.DefaultUsers?.Any() ?? false)
            {
                foreach (var defaultUser in _config.DefaultUsers)
                {
                    try
                    {
                        _usersDataContext.AddUser(defaultUser);
                    }
                    catch (Exception exc)
                    {
                        _logger.LogWarning($"{nameof(UsersService)}: An error occurred while adding user ({exc.Message}).");
                    }
                }

                _config.DefaultUsers = null;
            }

            //  Throw exception if no any default user has been loaded.

            if (!_usersDataContext.Users.Any())
            {
                throw new Exception("No default user has been defined.");
            }
        }

        #endregion SETUP METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Save data context. </summary>
        private void SaveDataContext()
        {
            var saved = _usersDataContext.SaveData();

            if (saved)
                _logger.LogInformation($"{nameof(UsersService)}: Users configuration updated.");
        }

        #endregion UTILITY METHODS

    }
}
