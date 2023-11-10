using ArduinoConnectWeb.Data.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ArduinoConnectWeb.Services.Authentication
{
    public class AuthService : IAuthService
    {

        //  CONST

        private static readonly string USERS_FILE_NAME = "users.json";


        //  VARIABLES

        private readonly IConfiguration _configuration;
        private List<UserProfile>? _userProfiles;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            LoadUserProfiles();
        }

        #endregion CLASS METHODS

        #region SERVICE METHODS

        //  --------------------------------------------------------------------------------
        public async Task<Tokens?> LoginFromApi(string? login, string? password)
        {
            var userProfile = GetUserProfile(login, password);

            return await Task.FromResult(
                userProfile != null ? GenerateTokens(userProfile) : null);
        }

        //  --------------------------------------------------------------------------------
        public async Task<bool> LoginFromWeb(string login, string password, HttpContext httpContext)
        {
            var userProfile = GetUserProfile(login, password);
            var validityTime = TimeSpan.Parse(_configuration["Jwt:AccessTokenValidityTime"]);

            if (userProfile != null)
            {
                var claims = GetClaims(userProfile);
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(validityTime),
                    IssuedUtc = DateTimeOffset.UtcNow,
                    IsPersistent = false
                };

                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return true;
            }

            return false;
        }

        #endregion SERVICE METHODS

        #region TOKENS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        private Token GenerateAccessToken(UserProfile userProfile)
        {
            var claims = GetClaims(userProfile);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var validityTime = TimeSpan.Parse(_configuration["Jwt:AccessTokenValidityTime"]);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.Add(validityTime),
                signingCredentials: credentials);

            var tokenContent = new JwtSecurityTokenHandler().WriteToken(token);

            return new Token(tokenContent, userProfile.Id, validityTime);
        }

        //  --------------------------------------------------------------------------------
        private Token GenerateRefreshToken(UserProfile userProfile)
        {
            int size = int.Parse(_configuration["Jwt:RefreshTokenSize"]);
            byte[] randomNumber = new byte[size];
            var validityTime = TimeSpan.Parse(_configuration["Jwt:RefreshTokenValidityTime"]);

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var tokenContent = Convert.ToBase64String(randomNumber);
                return new Token(tokenContent, userProfile.Id, validityTime);
            }
        }

        //  --------------------------------------------------------------------------------
        private Tokens GenerateTokens(UserProfile userProfile)
        {
            return new Tokens()
            {
                AccessToken = GenerateAccessToken(userProfile),
                RefreshToken = GenerateRefreshToken(userProfile),
            };
        }

        //  --------------------------------------------------------------------------------
        private Claim[] GetClaims(UserProfile userProfile)
        {
            return new Claim[]
            {
                new Claim(ClaimTypes.Name, userProfile.Login),
                new Claim(ClaimTypes.NameIdentifier, userProfile.Id)
            };
        }

        #endregion TOKENS MANAGEMENT METHODS

        #region USER PROFILES LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        private void LoadDefaultUserProfiles()
        {
            _userProfiles = new List<UserProfile>()
            {
                new UserProfile()
                {
                    Login = _configuration["Defaults:Admin:Login"],
                    Password = _configuration["Defaults:Admin:Password"],
                }
            };

            SaveUserProfiles();
        }

        //  --------------------------------------------------------------------------------
        private void LoadUserProfiles()
        {
            if (File.Exists(USERS_FILE_NAME))
            {
                var usersData = File.ReadAllText(USERS_FILE_NAME);

                if (!string.IsNullOrEmpty(usersData))
                    _userProfiles = JsonConvert.DeserializeObject<List<UserProfile>>(usersData);
            }

            if (_userProfiles == null || !_userProfiles.Any())
                LoadDefaultUserProfiles();
        }

        //  --------------------------------------------------------------------------------
        private void SaveUserProfiles()
        {
            var usersData = JsonConvert.SerializeObject(_userProfiles, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            });

            File.WriteAllText(USERS_FILE_NAME, usersData);
        }

        #endregion USER PROFILES LOAD & SAVE METHODS

        #region USER PROFILES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        private UserProfile? GetUserProfile(string? login, string? password)
        {
            return _userProfiles?.FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        #endregion USER PROFILES MANAGEMENT METHODS

    }
}
