using ArduinoConnectWeb.DataContexts;
using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base;
using ArduinoConnectWeb.Models.Users;
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
        public async Task<ResponseBaseModel<ResponseSessionsListModel>> GetSessions(string? accessToken)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var session = _authDataContext.Sessions.FirstOrDefault(s => s.AccessToken == accessToken);

                    if (session is null)
                        return new ResponseBaseModel<ResponseSessionsListModel>("Incorrect authorization, access denied.");

                    if (!session.IsAccessTokenValid())
                        return new ResponseBaseModel<ResponseSessionsListModel>("Access token has expired, access denied.");

                    var allUserSessions = _authDataContext.Sessions.Where(s => s.UserId == session.UserId).ToList();

                    var result = new ResponseSessionsListModel()
                    {
                        CurrentSessions = allUserSessions?.Select(s => new ResponseSessionsListItemModel()
                        {
                            AccessTokenValidityTime = s.AccessTokenValidityTime,
                            RefreshTokenValidityTime = s.RefreshTokenValidityTime,
                            SessionStart = s.SessionStart,
                            UserId = s.UserId
                        }).ToList()
                    };

                    return new ResponseBaseModel<ResponseSessionsListModel>(result);
                }
                catch (Exception exc)
                {
                    return new ResponseBaseModel<ResponseSessionsListModel>(errorMessage: $"Authorization failed : {exc.Message}.");
                }
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Login. </summary>
        /// <param name="requestLoginModel"> Request login model. </param>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<SessionDataModel>> Login(RequestLoginModel requestLoginModel)
        {
            var userResponse = await _usersService.ValidateUser(
                requestLoginModel?.UserName, requestLoginModel?.Password);

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

                try
                {
                    _authDataContext.AddSession(session);

                    return new ResponseBaseModel<SessionDataModel>(session);
                }
                catch (Exception exc)
                {
                    return new ResponseBaseModel<SessionDataModel>($"The session could not be started: {exc.Message}.");
                }
            }
            else if (userResponse.ErrorMessages?.Any() ?? false)
            {
                return new ResponseBaseModel<SessionDataModel>(userResponse.ErrorMessages);
            }
            else
            {
                return new ResponseBaseModel<SessionDataModel>("An unknown error occurred while processing data.");
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Logout. </summary>
        /// <param name="requestLogoutModel"> Request logout model. </param>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<string>> Logout(RequestLogoutModel requestLogoutModel)
        {
            var userResponse = await _usersService.GetUserByUserName(requestLogoutModel.UserName);

            if (userResponse.IsSuccess && userResponse.Content is ResponseUserModel responseUserModel)
            {
                try
                {
                    var sessions = _authDataContext.Sessions.Where(s => s.UserId == responseUserModel.Id)?.ToList();

                    if (!(sessions?.Any() ?? false))
                        return new ResponseBaseModel<string>(errorMessage: $"User \"{requestLogoutModel.UserName}\" is not logged in.");

                    var currentSessionFromAccessToken = sessions.FirstOrDefault(s 
                        => s.AccessToken == requestLogoutModel.AccessToken && s.IsAccessTokenValid());

                    var currentSessionFromRefreshToken = sessions.FirstOrDefault(s 
                        => s.RefreshToken == requestLogoutModel.RefreshToken && s.IsRefreshTokenValid());

                    if (currentSessionFromAccessToken is null && currentSessionFromRefreshToken is null)
                        return new ResponseBaseModel<string>(errorMessage: "Invalid tokens.");

                    if (currentSessionFromAccessToken is not null && currentSessionFromRefreshToken is not null
                        && !currentSessionFromAccessToken.Equals(currentSessionFromRefreshToken))
                        return new ResponseBaseModel<string>(errorMessage: "Different session tokens has been used.");

                    if (requestLogoutModel.AllSessions)
                    {
                        sessions.ForEach(_authDataContext.RemoveSession);
                        return new ResponseBaseModel<string>(content: $"Logged out of every session.");
                    }
                    else
                    {
                        if (currentSessionFromAccessToken is not null)
                        {
                            _authDataContext.RemoveSession(currentSessionFromAccessToken);
                            return new ResponseBaseModel<string>(content: $"Logged out.");
                        }
                        else if (currentSessionFromRefreshToken is not null)
                        {
                            _authDataContext.RemoveSession(currentSessionFromRefreshToken);
                            return new ResponseBaseModel<string>(content: $"Logged out.");
                        }
                        else
                        {
                            return new ResponseBaseModel<string>(errorMessage: "An unknown error occurred while processing data.");
                        }
                    }
                }
                catch (Exception exc)
                {
                    return new ResponseBaseModel<string>(errorMessage: $"Unable to logout user \"{requestLogoutModel.UserName}\": {exc.Message}.");
                }
            }
            else if (userResponse.ErrorMessages?.Any() ?? false)
            {
                return new ResponseBaseModel<string>(userResponse.ErrorMessages);
            }
            else
            {
                return new ResponseBaseModel<string>(errorMessage: "An unknown error occurred while processing data.");
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Refresh tokens. </summary>
        /// <param name="requestRefreshModel"> Request refresh model. </param>
        /// <returns> Response view model. </returns>
        public async Task<ResponseBaseModel<SessionDataModel>> Refresh(RequestRefreshModel requestRefreshModel)
        {
            var userResponse = await _usersService.GetUserByUserName(requestRefreshModel.UserName);

            if (userResponse.IsSuccess && userResponse.Content is ResponseUserModel responseUserModel)
            {
                try
                {
                    var sessions = _authDataContext.Sessions.Where(s => s.UserId == responseUserModel.Id);

                    if (!sessions.Any())
                        return new ResponseBaseModel<SessionDataModel>($"User \"{requestRefreshModel.UserName}\" is not logged in.");

                    var currentSession = sessions.FirstOrDefault(s => s.RefreshToken == requestRefreshModel.RefreshToken);

                    if (currentSession is null)
                        return new ResponseBaseModel<SessionDataModel>($"Invalid {nameof(requestRefreshModel.RefreshToken)}.");

                    if (!currentSession.IsRefreshTokenValid())
                    {
                        _authDataContext.RemoveSession(currentSession);
                        return new ResponseBaseModel<SessionDataModel>($"Session expired.");
                    }

                    var user = responseUserModel.ConvertToUserDataModel();

                    var newAccessToken = GenerateAccessToken(user, out DateTime newAccessTokenValidityTime);
                    var newRefreshToken = GenerateRefreshToken(out DateTime newRefreshTokenValidityTime);

                    currentSession.AccessToken = newAccessToken;
                    currentSession.RefreshToken = newRefreshToken;
                    currentSession.AccessTokenValidityTime = newAccessTokenValidityTime;
                    currentSession.RefreshTokenValidityTime = newRefreshTokenValidityTime;

                    _authDataContext.UpdateSession(currentSession);
                    return new ResponseBaseModel<SessionDataModel>(currentSession);
                }
                catch (Exception exc)
                {
                    return new ResponseBaseModel<SessionDataModel>($"Unable to refresh token: {exc.Message}.");
                }
            }
            else if (userResponse.ErrorMessages?.Any() ?? false)
            {
                return new ResponseBaseModel<SessionDataModel>(userResponse.ErrorMessages);
            }
            else
            {
                return new ResponseBaseModel<SessionDataModel>("An unknown error occurred while processing data.");
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
