﻿using ArduinoConnectWeb.DataContexts;
using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Auth.ResponseModels;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Exceptions;
using ArduinoConnectWeb.Models.Users;
using ArduinoConnectWeb.Models.Users.RequestModels;
using ArduinoConnectWeb.Models.Users.ResponseModels;
using ArduinoConnectWeb.Services.Auth;
using ArduinoConnectWeb.Services.Base;
using ArduinoConnectWeb.Utilities;
using Newtonsoft.Json;
using System;

namespace ArduinoConnectWeb.Services.Users
{
    public class UsersService : DataProcessor, IUsersService
    {

        //  VARIABLES

        private readonly UsersServiceConfig _config;
        private readonly UsersDataContext _usersDataContext;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UsersService class constructor. </summary>
        /// <param name="config"> Users service config. </param>
        /// <param name="logger"> Application logger. </param>
        public UsersService(UsersServiceConfig config, ILogger<UsersService> logger) : base(logger)
        {
            _config = config;

            _usersDataContext = new UsersDataContext(_config.UsersStorageFile);
            _usersDataContext.LoadData();

            if (!_usersDataContext.HasUsers())
                InitializeDefaultUsers();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create new user async. </summary>
        /// <param name="requestUserCreateModel"> Request user create model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserResponseModel>> CreateUserAsync(CreateUserRequestModel requestUserCreateModel)
        {
            return await ProcessAsync(() =>
            {
                if (requestUserCreateModel == null)
                    throw new ProcessingException("Invalid input data", StatusCodes.Status400BadRequest);

                List<string> errorMessages = new();

                if (string.IsNullOrWhiteSpace(requestUserCreateModel.UserName))
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

                return new BaseResponseModel<UserResponseModel>(new UserResponseModel(newUser));
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by identifier async. </summary>
        /// <param name="id"> User identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserResponseModel>> GetUserByIdAsync(string? id)
        {
            return await ProcessAsync(() =>
            {
                if (string.IsNullOrEmpty(id))
                    throw new ProcessingException("Invalid user identifier", StatusCodes.Status400BadRequest);

                var user = _usersDataContext.Users.FirstOrDefault(u => u.Id == id.ToUpper());

                if (user is null)
                    throw new ProcessingException("User not found", StatusCodes.Status400BadRequest);

                return new BaseResponseModel<UserResponseModel>(new UserResponseModel(user));
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by identifier async. </summary>
        /// <param name="id"> User identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserDataModel>> GetFullUserByIdAsync(string? id)
        {
            return await ProcessAsync(() =>
            {
                if (string.IsNullOrEmpty(id))
                    throw new ProcessingException("Invalid user identifier", StatusCodes.Status400BadRequest);

                var user = _usersDataContext.Users.FirstOrDefault(u => u.Id == id.ToUpper());

                if (user is null)
                    throw new ProcessingException("User not found", StatusCodes.Status400BadRequest);

                return new BaseResponseModel<UserDataModel>(user);
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by user name async. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="authService"> Authentication service interface. </param>
        /// <param name="userName"> User name. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserResponseModel>> GetUserByUserNameAsync(string? userName)
        {
            return await ProcessAsync(() =>
            {
                if (string.IsNullOrEmpty(userName))
                    throw new ProcessingException("Invalid user name", StatusCodes.Status400BadRequest);

                var user = _usersDataContext.Users.FirstOrDefault(u => u.UserName == userName);

                if (user is null)
                    throw new ProcessingException("User not found", StatusCodes.Status400BadRequest);

                return new BaseResponseModel<UserResponseModel>(new UserResponseModel(user));
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get users list async. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="authService"> Authentication service interface. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserListResponseModel>> GetUsersListAsync()
        {
            return await ProcessAsync(() =>
            {
                var responseData = new UserListResponseModel()
                {
                    Users = _usersDataContext.Users.Select(u => new UserListItemResponseModel()
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                    }).ToList()
                };

                return new BaseResponseModel<UserListResponseModel>(responseData);
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove user by id async. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="authService"> Authentication service interface. </param>
        /// <param name="id"> User identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<string>> RemoveUserAsync(string? id)
        {
            return await ProcessAsync(() =>
            {
                if (string.IsNullOrEmpty(id))
                    throw new ProcessingException("Invalid user identifier", StatusCodes.Status400BadRequest);

                var user = _usersDataContext.Users.FirstOrDefault(u => u.Id == id.ToUpper());

                if (user is null)
                    throw new ProcessingException("User not found", StatusCodes.Status400BadRequest);

                _usersDataContext.RemoveUser(user);

                SaveDataContext();

                return new BaseResponseModel<string>(content: $"User removed");
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update user async. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="authService"> Authentication service interface. </param>
        /// <param name="id"> User identifier. </param>
        /// <param name="requestUserUpdateModel"></param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserResponseModel>> UpdateUserAsync(string? id, UpdateUserRequestModel requestUserUpdateModel)
        {
            return await ProcessAsync(() =>
            {
                var anyChange = false;

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

                return new BaseResponseModel<UserResponseModel>(new UserResponseModel(user));
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate user async. </summary>
        /// <param name="userName"> User name. </param>
        /// <param name="password"> Password. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<UserDataModel>> ValidateUserAsync(string? userName, string? password)
        {
            return await ProcessAsync(() =>
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

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Initialize default users. </summary>
        private void InitializeDefaultUsers()
        {
            Logger.LogInformation($"{nameof(UsersService)}: Initializing default users.");

            //  Log initialization errors.

            if (_config.InitErrorMessages?.Any() ?? false)
            {
                foreach (var errorMessage in _config.InitErrorMessages)
                {
                    Logger.LogWarning($"{nameof(UsersService)}: An error occurred while adding default user ({errorMessage}).");
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
                        Logger.LogWarning($"{nameof(UsersService)}: An error occurred while adding default user ({exc.Message}).");
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
                Logger.LogInformation($"{nameof(UsersService)}: Users configuration updated.");
        }

        #endregion UTILITY METHODS

    }
}