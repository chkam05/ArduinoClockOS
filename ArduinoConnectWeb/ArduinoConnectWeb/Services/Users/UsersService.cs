using ArduinoConnectWeb.DataContexts;
using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Auth.ResponseModels;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Exceptions;
using ArduinoConnectWeb.Models.Users;
using ArduinoConnectWeb.Models.Users.RequestModels;
using ArduinoConnectWeb.Models.Users.ResponseModels;
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
        /// <param name="accessToken"> Access token. </param>
        /// <param name="requestUserCreateModel"> Request user create model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserResponseModel>> CreateUser(string? accessToken, CreateUserRequestModel requestUserCreateModel)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(accessToken))
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    if (requestUserCreateModel == null)
                        throw new ProcessingException("Invalid input data", StatusCodes.Status400BadRequest);

                    List<string> errorMessages = new();

                    if (string.IsNullOrEmpty(requestUserCreateModel.UserName))
                        errorMessages.Add($"Invalid {nameof(requestUserCreateModel.UserName)}");

                    if (string.IsNullOrEmpty(requestUserCreateModel.Password))
                        errorMessages.Add($"Invalid {nameof(requestUserCreateModel.Password)}");

                    using (var passwordValidator = PasswordValidator.GetStrongConfiguration())
                    {
                        if (!passwordValidator.ValidatePassword(requestUserCreateModel.Password, out string? validationError))
                            errorMessages.Add($"Invalid {nameof(requestUserCreateModel.Password)}. {validationError}");
                    }

                    if (requestUserCreateModel.Password != requestUserCreateModel.PasswordRepeat)
                        errorMessages.Add($"{nameof(requestUserCreateModel.Password)} and {nameof(requestUserCreateModel.PasswordRepeat)} do not match");

                    if (errorMessages.Any())
                        throw new ProcessingException(string.Join("; ", errorMessages), StatusCodes.Status400BadRequest);

                    if (_usersDataContext.Users.Any(u => u.UserName.ToLower() == requestUserCreateModel.UserName?.ToLower()))
                        throw new ProcessingException($"User named \"{requestUserCreateModel.UserName}\" already exists", StatusCodes.Status400BadRequest);

                    var newUser = new UserDataModel(
                        null,
                        requestUserCreateModel.UserName,
                        SecurityUtilities.ComputeSha256Hash(requestUserCreateModel.Password),
                        requestUserCreateModel.PermissionLevel);

                    _usersDataContext.AddUser(newUser);
                    SaveDataContext();

                    return new BaseResponseModel<UserResponseModel>(GetResponseUserModel(newUser));
                }
                catch (ProcessingException exc)
                {
                    return new BaseResponseModel<UserResponseModel>(exc.Message, exc.StatusCode);
                }
                catch (Exception exc)
                {
                    var errorMessage = $"An unknown error occurred while processing data: {exc.Message}";

                    return new BaseResponseModel<UserResponseModel>(errorMessage, StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by identifier. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="id"> User identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserResponseModel>> GetUserById(string? accessToken, string? id)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(accessToken))
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    if (string.IsNullOrEmpty(id))
                        throw new ProcessingException("Invalid user identifier", StatusCodes.Status400BadRequest);

                    var user = _usersDataContext.Users.FirstOrDefault(u => u.Id == id.ToUpper());

                    if (user is null)
                        throw new ProcessingException("User not found", StatusCodes.Status400BadRequest);

                    return new BaseResponseModel<UserResponseModel>(GetResponseUserModel(user));
                }
                catch (ProcessingException exc)
                {
                    return new BaseResponseModel<UserResponseModel>(exc.Message, exc.StatusCode);
                }
                catch (Exception exc)
                {
                    var errorMessage = $"An unknown error occurred while processing data: {exc.Message}";

                    return new BaseResponseModel<UserResponseModel>(errorMessage, StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by identifier. </summary>
        /// <param name="id"> User identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserResponseModel>> GetUserById(string? id)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(id))
                        throw new ProcessingException("Invalid user identifier", StatusCodes.Status400BadRequest);

                    var user = _usersDataContext.Users.FirstOrDefault(u => u.Id == id.ToUpper());

                    if (user is null)
                        throw new ProcessingException("User not found", StatusCodes.Status400BadRequest);

                    return new BaseResponseModel<UserResponseModel>(GetResponseUserModel(user));
                }
                catch (ProcessingException exc)
                {
                    return new BaseResponseModel<UserResponseModel>(exc.Message, exc.StatusCode);
                }
                catch (Exception exc)
                {
                    var errorMessage = $"An unknown error occurred while processing data: {exc.Message}";

                    return new BaseResponseModel<UserResponseModel>(errorMessage, StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by user name. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="userName"> User name. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserResponseModel>> GetUserByUserName(string? accessToken, string? userName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(accessToken))
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    if (string.IsNullOrEmpty(userName))
                        throw new ProcessingException("Invalid user name", StatusCodes.Status400BadRequest);

                    var user = _usersDataContext.Users.FirstOrDefault(u => u.UserName == userName);

                    if (user is null)
                        throw new ProcessingException("User not found", StatusCodes.Status400BadRequest);

                    return new BaseResponseModel<UserResponseModel>(GetResponseUserModel(user));
                }
                catch (ProcessingException exc)
                {
                    return new BaseResponseModel<UserResponseModel>(exc.Message, exc.StatusCode);
                }
                catch (Exception exc)
                {
                    var errorMessage = $"An unknown error occurred while processing data: {exc.Message}";

                    return new BaseResponseModel<UserResponseModel>(errorMessage, StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get users list. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserListResponseModel>> GetUsersList(string? accessToken)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(accessToken))
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    var responseData = new UserListResponseModel()
                    {
                        Users = _usersDataContext.Users.Select(u => new UserListItemResponseModel()
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                        }).ToList()
                    };

                    return new BaseResponseModel<UserListResponseModel>(responseData);
                }
                catch (ProcessingException exc)
                {
                    return new BaseResponseModel<UserListResponseModel>(exc.Message, exc.StatusCode);
                }
                catch (Exception exc)
                {
                    var errorMessage = $"An unknown error occurred while processing data: {exc.Message}";

                    return new BaseResponseModel<UserListResponseModel>(errorMessage, StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove user by id. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="id"> User identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<string>> RemoveUser(string? accessToken, string? id)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(accessToken))
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    if (string.IsNullOrEmpty(id))
                        throw new ProcessingException("Invalid user identifier", StatusCodes.Status400BadRequest);

                    var user = _usersDataContext.Users.FirstOrDefault(u => u.Id == id.ToUpper());

                    if (user is null)
                        throw new ProcessingException("User not found", StatusCodes.Status400BadRequest);

                    _usersDataContext.RemoveUser(user);
                    SaveDataContext();

                    return new BaseResponseModel<string>(content: $"User removed");
                }
                catch (ProcessingException exc)
                {
                    return new BaseResponseModel<string>(exc.Message, exc.StatusCode);
                }
                catch (Exception exc)
                {
                    var errorMessage = $"An unknown error occurred while processing data: {exc.Message}";

                    return new BaseResponseModel<string>(errorMessage, StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update user. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="id"> User identifier. </param>
        /// <param name="requestUserUpdateModel"></param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserResponseModel>> UpdateUser(string? accessToken, string? id, UpdateUserRequestModel requestUserUpdateModel)
        {
            return await Task.Run(() =>
            {
                var anyChange = false;

                try
                {
                    if (string.IsNullOrEmpty(accessToken))
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    if (string.IsNullOrEmpty(id))
                        throw new ProcessingException("Invalid user identifier", StatusCodes.Status400BadRequest);

                    var user = _usersDataContext.Users.FirstOrDefault(u => u.Id == id.ToUpper())?.CloneWithType();

                    if (user is null)
                        throw new ProcessingException("User not found", StatusCodes.Status400BadRequest);

                    if (!string.IsNullOrEmpty(requestUserUpdateModel.NewUserName))
                    {
                        if (_usersDataContext.Users.Any(u => u.UserName.ToLower() == requestUserUpdateModel.NewUserName.ToLower()))
                            throw new ProcessingException($"User named \"{requestUserUpdateModel.NewUserName}\" already exists", StatusCodes.Status400BadRequest);

                        user.UserName = requestUserUpdateModel.NewUserName;
                        anyChange = true;
                    }

                    if (!string.IsNullOrEmpty(requestUserUpdateModel.NewPassword))
                    {
                        using (var passwordValidator = PasswordValidator.GetStrongConfiguration())
                        {
                            if (!passwordValidator.ValidatePassword(requestUserUpdateModel.NewPassword, out string? validationError))
                            {
                                var errorMessage = $"Invalid {nameof(requestUserUpdateModel.NewPassword)}. {validationError}";
                                throw new ProcessingException(errorMessage, StatusCodes.Status400BadRequest);
                            }
                        }

                        if (requestUserUpdateModel.NewPassword != requestUserUpdateModel.NewPasswordRepeat)
                        {
                            var errorMessage = $"{nameof(requestUserUpdateModel.NewPassword)} and {nameof(requestUserUpdateModel.NewPasswordRepeat)} do not match";
                            throw new ProcessingException(errorMessage, StatusCodes.Status400BadRequest);
                        }

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

                        _usersDataContext.UpdateUser(user);
                        SaveDataContext();
                    }

                    return new BaseResponseModel<UserResponseModel>(GetResponseUserModel(user));
                }
                catch (ProcessingException exc)
                {
                    return new BaseResponseModel<UserResponseModel>(exc.Message, exc.StatusCode);
                }
                catch (Exception exc)
                {
                    var errorMessage = $"An unknown error occurred while processing data: {exc.Message}";

                    return new BaseResponseModel<UserResponseModel>(errorMessage, StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate user. </summary>
        /// <param name="userName"> User name. </param>
        /// <param name="password"> Password. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserDataModel>> ValidateUser(string? userName, string? password)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(userName))
                    return new BaseResponseModel<UserDataModel>("User name can not be empty.");

                if (string.IsNullOrEmpty(password))
                    return new BaseResponseModel<UserDataModel>("Password can not be empty.");

                var user = _usersDataContext.Users.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());

                if (user is null)
                    return new BaseResponseModel<UserDataModel>("Invalid user name.");

                if (user.PasswordHash != SecurityUtilities.ComputeSha256Hash(password))
                    return new BaseResponseModel<UserDataModel>("Invalid password.");

                return new BaseResponseModel<UserDataModel>(user);
            });
        }

        #endregion INTERACTION METHODS

        #region RESPONSE CREATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get response user model. </summary>
        /// <param name="user"> User data model. </param>
        /// <returns> Response user model. </returns>
        private UserResponseModel GetResponseUserModel(UserDataModel user)
        {
            return new UserResponseModel(
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
