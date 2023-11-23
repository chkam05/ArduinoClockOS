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
        /// <summary> Authorize. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<SessionDataModel>> AuthorizeAsync(string? accessToken)
        {
            return await ProcessAsync(() =>
            {
                if (string.IsNullOrEmpty(accessToken))
                    throw new UnauthorizedException("Invalid AccessToken");

                var session = _authDataContext.Sessions.FirstOrDefault(s => s.AccessToken == accessToken);

                if (session is null)
                    throw new UnauthorizedException("Invalid AccessToken");

                if (!session.IsAccessTokenValid())
                    throw new UnauthorizedException("AccessToken expired");

                return new BaseResponseModel<SessionDataModel>(session);
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Refresh tokens. </summary>
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

                var userResponse = await _usersService.GetUserByIdAsync(session.UserId);

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
        /// <summary> Get current user sessions. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<SessionListResponseModel>> GetSessionsAsync(string? accessToken)
        {
            return await ProcessAsyncWithAuthorization(accessToken, this, (session) =>
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
        /// <summary> Login. </summary>
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
        /// <summary> Logout. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<string>> LogoutAsync(string? accessToken)
        {
            return await ProcessAsyncWithAuthorization(accessToken, this, (session) =>
            {
                _authDataContext.RemoveSession(session);

                return new BaseResponseModel<string>(content: $"Logged out");
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Logout from all sessions. </summary>
        /// <param name="accessToken"> Access token. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<string>> LogoutAllSessionsAsync(string? accessToken)
        {
            return await ProcessAsyncWithAuthorization(accessToken, this, (session) =>
            {
                var allSessions = _authDataContext.Sessions.Where(s => s.UserId == session.UserId);

                return new BaseResponseModel<string>(content: $"Logged out");
            });
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
