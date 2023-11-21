using ArduinoConnectWeb.DataContexts;
using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Auth.RequestModels;
using ArduinoConnectWeb.Models.Auth.ResponseModels;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Exceptions;
using ArduinoConnectWeb.Models.Users;
using ArduinoConnectWeb.Models.Users.ResponseModels;
using ArduinoConnectWeb.Services.Users;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ArduinoConnectWeb.Services.Auth
{
    public class AuthService : IAuthService
    {

        //  VARIABLES

        private readonly AuthDataContext _authDataContext;
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

            _authDataContext = new AuthDataContext();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get current user sessions. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<SessionListResponseModel>> GetSessions(string? accessToken)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(accessToken))
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    var session = _authDataContext.Sessions.FirstOrDefault(s => s.AccessToken == accessToken);

                    if (session is null)
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    if (!session.IsAccessTokenValid())
                        throw new ProcessingException("AccessToken expired", StatusCodes.Status401Unauthorized);

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
                }
                catch (ProcessingException exc)
                {
                    return new BaseResponseModel<SessionListResponseModel>(exc.Message, exc.StatusCode);
                }
                catch (Exception exc)
                {
                    var errorMessage = $"An unknown error occurred while processing data: {exc.Message}";

                    return new BaseResponseModel<SessionListResponseModel>(errorMessage, StatusCodes.Status400BadRequest);
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Login. </summary>
        /// <param name="loginRequestModel"> Login request model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<SessionDataModel>> Login(LoginRequestModel loginRequestModel)
        {
            try
            {
                var userResponse = await _usersService.ValidateUser(
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
            }
            catch (ProcessingException exc)
            {
                return new BaseResponseModel<SessionDataModel>(exc.Message, exc.StatusCode);
            }
            catch (Exception exc)
            {
                return new BaseResponseModel<SessionDataModel>($"An unknown error occurred while processing data: {exc.Message}");
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Logout. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<string>> Logout(string? accessToken)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(accessToken))
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    var session = _authDataContext.Sessions.FirstOrDefault(s => s.AccessToken == accessToken);

                    if (session is null)
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    if (!session.IsAccessTokenValid())
                        throw new ProcessingException("AccessToken expired", StatusCodes.Status401Unauthorized);

                    _authDataContext.RemoveSession(session);

                    return new BaseResponseModel<string>(content: $"Logged out");
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
        /// <summary> Logout from all sessions. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<string>> LogoutAllSessions(string? accessToken)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(accessToken))
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    var session = _authDataContext.Sessions.FirstOrDefault(s => s.AccessToken == accessToken);

                    if (session is null)
                        throw new ProcessingException("Invalid AccessToken", StatusCodes.Status401Unauthorized);

                    if (!session.IsAccessTokenValid())
                        throw new ProcessingException("AccessToken expired", StatusCodes.Status401Unauthorized);

                    var allSessions = _authDataContext.Sessions.Where(s => s.UserId == session.UserId);

                    if (allSessions.Any())
                        _authDataContext.RemoveSessions(allSessions);

                    return new BaseResponseModel<string>(content: $"Logged out");
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
        /// <summary> Refresh tokens. </summary>
        /// <param name="refreshRequestModel"> Refresh request model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<SessionDataModel>> Refresh(RefreshRequestModel refreshRequestModel)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshRequestModel.RefreshToken))
                    throw new ProcessingException("Invalid RefreshToken", StatusCodes.Status401Unauthorized);

                var session = _authDataContext.Sessions.FirstOrDefault(s => s.RefreshToken == refreshRequestModel.RefreshToken);

                if (session is null)
                    throw new ProcessingException("Invalid RefreshToken", StatusCodes.Status401Unauthorized);

                if (!session.IsRefreshTokenValid())
                {
                    _authDataContext.RemoveSession(session);
                    throw new ProcessingException("Session expired", StatusCodes.Status401Unauthorized);
                }

                var userResponse = await _usersService.GetUserById(session.UserId);

                if (userResponse.IsSuccess && userResponse.Content is UserResponseModel responseUserModel)
                {
                    var user = responseUserModel.ConvertToUserDataModel();

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
            }
            catch (ProcessingException exc)
            {
                return new BaseResponseModel<SessionDataModel>(exc.Message, exc.StatusCode);
            }
            catch (Exception exc)
            {
                var errorMessage = $"An unknown error occurred while processing data: {exc.Message}";
                return new BaseResponseModel<SessionDataModel>(errorMessage, StatusCodes.Status400BadRequest);
            }
        }

        #endregion INTERACTION METHODS

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
