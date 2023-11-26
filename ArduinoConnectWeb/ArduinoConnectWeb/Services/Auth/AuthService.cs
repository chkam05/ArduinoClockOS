using ArduinoConnectWeb.DataContexts;
using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Auth.RequestModels;
using ArduinoConnectWeb.Models.Auth.ResponseModels;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Exceptions;
using ArduinoConnectWeb.Models.Users;
using ArduinoConnectWeb.Models.Users.ResponseModels;
using ArduinoConnectWeb.Services.Base;
using ArduinoConnectWeb.Services.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ArduinoConnectWeb.Services.Auth
{
    public class AuthService : DataProcessor, IAuthService
    {

        //  VARIABLES

        private readonly AuthDataContext _authDataContext;
        private readonly AuthServiceConfig _config;
        private readonly IUsersService _usersService;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AuthService class constructor. </summary>
        /// <param name="config"> Users service config. </param>
        /// <param name="logger"> Application logger. </param>
        public AuthService(AuthServiceConfig config, ILogger<AuthService> logger, IUsersService usersService)
            : base(logger)
        {
            _config = config;
            _usersService = usersService;

            _authDataContext = new AuthDataContext();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Authorize async. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<SessionDataModel>> AuthorizeAsync(string? accessToken)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(accessToken))
                        throw new UnauthorizedException("Invalid AccessToken");

                    var session = _authDataContext.Sessions.FirstOrDefault(s => s.AccessToken == accessToken);

                    if (session is null)
                        throw new UnauthorizedException("Invalid AccessToken");

                    if (!session.IsAccessTokenValid())
                        throw new UnauthorizedException("AccessToken expired");

                    return new BaseResponseModel<SessionDataModel>(session);
                }
                catch (UnauthorizedException exc)
                {
                    return new BaseResponseModel<SessionDataModel>(exc.Message, exc.StatusCode);
                }
                catch (ProcessingException exc)
                {
                    return new BaseResponseModel<SessionDataModel>(exc.Message, exc.StatusCode);
                }
                catch (Exception exc)
                {
                    var errorMessage = $"{ERROR_MESSAGE}: {exc.Message}";

                    return new BaseResponseModel<SessionDataModel>(errorMessage, StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Refresh tokens async. </summary>
        /// <param name="refreshRequestModel"> Refresh request model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<SessionDataModel>> RefreshAsync(RefreshRequestModel refreshRequestModel)
        {
            return await ProcessTaskAsync(async () =>
            {
                if (string.IsNullOrEmpty(refreshRequestModel.RefreshToken))
                    throw new UnauthorizedException("Invalid RefreshToken");

                var session = _authDataContext.Sessions.FirstOrDefault(s => s.RefreshToken == refreshRequestModel.RefreshToken);

                if (session is null)
                    throw new UnauthorizedException("Invalid RefreshToken");

                if (!session.IsRefreshTokenValid())
                {
                    _authDataContext.RemoveSession(session);
                    throw new UnauthorizedException("Session expired");
                }

                var userResponse = await _usersService.GetFullUserByIdAsync(session.UserId);

                if (userResponse.IsSuccess && userResponse.Content is UserDataModel user)
                {
                    var newAccessToken = GenerateAccessToken(user, out DateTime newAccessTokenValidityTime);
                    var newRefreshToken = GenerateRefreshToken(out DateTime newRefreshTokenValidityTime);

                    session.AccessToken = newAccessToken;
                    session.RefreshToken = newRefreshToken;
                    session.AccessTokenValidityTime = newAccessTokenValidityTime;
                    session.RefreshTokenValidityTime = newRefreshTokenValidityTime;

                    _authDataContext.UpdateSession(session);

                    return new BaseResponseModel<SessionDataModel>(session);
                }
                else if (userResponse.ErrorMessages?.Any() ?? false)
                {
                    var message = userResponse.GetErrorMessagesAsOne() ?? "Token cannot be refreshed";
                    throw new ProcessingException(message, StatusCodes.Status400BadRequest);
                }
                else
                {
                    throw new ProcessingException("Token cannot be refreshed", StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get current user sessions async. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<SessionListResponseModel>> GetSessionsAsync(SessionDataModel session)
        {
            return await ProcessAsync(() =>
            {
                var allUserSessions = _authDataContext.Sessions.Where(s => s.UserId == session.UserId).ToList();

                var result = new SessionListResponseModel()
                {
                    CurrentSessions = allUserSessions?.Select(s => new SessionListItemResponseModel()
                    {
                        AccessTokenValidityTime = s.AccessTokenValidityTime,
                        RefreshTokenValidityTime = s.RefreshTokenValidityTime,
                        SessionStart = s.SessionStart,
                        UserId = s.UserId
                    }).ToList()
                };

                return new BaseResponseModel<SessionListResponseModel>(result);
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Login async. </summary>
        /// <param name="loginRequestModel"> Login request model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<SessionDataModel>> LoginAsync(LoginRequestModel loginRequestModel)
        {
            return await ProcessTaskAsync<SessionDataModel>(async () =>
            {
                var userResponse = await _usersService.ValidateUserAsync(
                    loginRequestModel?.UserName, loginRequestModel?.Password);

                if (userResponse.IsSuccess && userResponse.Content is UserDataModel user)
                {
                    var accessToken = GenerateAccessToken(user, out DateTime accessTokenValidityTime);
                    var refreshToken = GenerateRefreshToken(out DateTime refreshTokenValidityTime);

                    var session = new SessionDataModel(
                        null,
                        user.Id,
                        accessToken,
                        refreshToken,
                        accessTokenValidityTime,
                        refreshTokenValidityTime);

                    _authDataContext.AddSession(session);

                    return new BaseResponseModel<SessionDataModel>(session);
                }
                else if (userResponse.ErrorMessages?.Any() ?? false)
                {
                    var message = userResponse.GetErrorMessagesAsOne() ?? "The session cannot be started";
                    throw new ProcessingException(message, StatusCodes.Status400BadRequest);
                }
                else
                {
                    throw new ProcessingException("The session cannot be started", StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Logout async. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<string>> LogoutAsync(SessionDataModel session)
        {
            return await ProcessAsync(() =>
            {
                _authDataContext.RemoveSession(session);

                return new BaseResponseModel<string>(content: $"Logged out");
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Logout from all sessions async. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<string>> LogoutAllSessionsAsync(SessionDataModel session)
        {
            return await ProcessAsync(() =>
            {
                var allSessions = _authDataContext.Sessions.Where(s => s.UserId == session.UserId);

                return new BaseResponseModel<string>(content: $"Logged out");
            });
        }

        #endregion INTERACTION METHODS

        #region PROCESSING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Process async with authorization. </summary>
        /// <typeparam name="T"> Response data model. </typeparam>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="func"> Task to process. </param>
        /// <returns> Task with response view model. </returns>
        public async Task<BaseResponseModel<T>> ProcessAsyncWithAuthorization<T>(string? accessToken,
            Func<SessionDataModel, BaseResponseModel<T>> func) where T : class
        {
            var authorizationResponse = await AuthorizeAsync(accessToken);

            if (authorizationResponse.IsSuccess && authorizationResponse.Content is SessionDataModel sessionDataModel)
            {
                return await Task.Run(() =>
                {
                    try
                    {
                        return func(sessionDataModel);
                    }
                    catch (UnauthorizedException uexc)
                    {
                        return new BaseResponseModel<T>(uexc.Message, uexc.StatusCode);
                    }
                    catch (ProcessingException pexc)
                    {
                        return new BaseResponseModel<T>(pexc.Message, pexc.StatusCode);
                    }
                    catch (Exception exc)
                    {
                        var errorMessage = $"{ERROR_MESSAGE}: {exc.Message}";
                        return new BaseResponseModel<T>(errorMessage, StatusCodes.Status400BadRequest);
                    }
                });
            }
            else
            {
                var errorMessages = authorizationResponse.ErrorMessages ?? new List<string> { ERROR_MESSAGE };
                var statusCode = authorizationResponse.StatusCode;
                return new BaseResponseModel<T>(errorMessages, statusCode);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Process task async with authorization. </summary>
        /// <typeparam name="T"> Response data model. </typeparam>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="func"> Task to process. </param>
        /// <returns> Task with response view model. </returns>
        public async Task<BaseResponseModel<T>> ProcessTaskAsyncWithAuthorization<T>(string? accessToken,
            Func<SessionDataModel, Task<BaseResponseModel<T>>> func) where T : class
        {
            var authorizationResponse = await AuthorizeAsync(accessToken);

            if (authorizationResponse.IsSuccess && authorizationResponse.Content is SessionDataModel sessionDataModel)
            {
                try
                {
                    return await func(sessionDataModel);
                }
                catch (UnauthorizedException uexc)
                {
                    return new BaseResponseModel<T>(uexc.Message, uexc.StatusCode);
                }
                catch (ProcessingException pexc)
                {
                    return new BaseResponseModel<T>(pexc.Message, pexc.StatusCode);
                }
                catch (Exception exc)
                {
                    var errorMessage = $"{ERROR_MESSAGE}: {exc.Message}";
                    return new BaseResponseModel<T>(errorMessage, StatusCodes.Status400BadRequest);
                }
            }
            else
            {
                var errorMessages = authorizationResponse.ErrorMessages ?? new List<string> { ERROR_MESSAGE };
                var statusCode = authorizationResponse.StatusCode;
                return new BaseResponseModel<T>(errorMessages, statusCode);
            }
        }

        #endregion PROCESSING METHODS

        #region TOKEN MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Generate jwt access token. </summary>
        /// <param name="user"> User data model. </param>
        /// <returns> Jwt access token. </returns>
        private string GenerateAccessToken(UserDataModel user, out DateTime validityTime)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            validityTime = DateTime.UtcNow.Add(_config.JwtAccessTokenValidityTime);

            var token = new JwtSecurityToken(
                issuer: _config.JwtIssuer,
                audience: _config.JwtAudience,
                claims: GetClaims(user),
                expires: validityTime,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Generate refresh token. </summary>
        /// <returns> Refresh token. </returns>
        private string GenerateRefreshToken(out DateTime validityTime)
        {
            var randomNumber = new byte[_config.JwtRefreshTokenSize];

            validityTime = DateTime.UtcNow.Add(_config.JwtRefreshTokenValidityTime);

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get claims. </summary>
        /// <param name="user"> User data model. </param>
        /// <returns> Claims array. </returns>
        private Claim[] GetClaims(UserDataModel user)
        {
            return new[]
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.GivenName, user.UserName),
                new Claim(ClaimTypes.Role, $"{user.PermissionLevel}")
            };
        }

        #endregion TOKEN MANAGEMENT METHODS

    }
}
