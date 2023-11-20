using ArduinoConnectWeb.Models.Config;
using ArduinoConnectWeb.Services.Users;
using ArduinoConnectWeb.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace ArduinoConnectWeb.Services.Auth
{
    public static class AuthServiceExtension
    {

        //  CONST

        private const string DEFAULT_KEYS_FILE_PATH = "key_config.json";


        //  METHODS

        #region REGISTRATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup authentication. </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </returns>
        public static IServiceCollection SetupAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationUrl = GetApplicationUrl(services, configuration);
            var jwtIssuerSigningKey = GetJwtIssuerSigningKey(configuration);

            //  Add authentication.
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            //  Add JwtBearer authentication.
            authenticationBuilder.AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtIssuerSigningKey)),
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidAudience = applicationUrl,
                    ValidIssuer = applicationUrl,
                };
            });

            return services;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Register and configure auth service.  </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </param>
        public static IServiceCollection RegisterAuthService(this IServiceCollection services, IConfiguration configuration)
        {
            //  Register auth service configuration.
            services.RegisterConfiguration(configuration);

            //  Register auth service.
            services.AddSingleton<IAuthService, AuthService>();

            return services;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Register auth service configuration. </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </param>
        private static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationUrl = GetApplicationUrl(services, configuration);
            var jwtIssuerSigningKey = GetJwtIssuerSigningKey(configuration);
            var jwtAccessTokenValidityTime = configuration.GetValue<TimeSpan?>("Jwt:AccessTokenValidityTime") ?? new TimeSpan(0, 15, 0);
            var jwtRefreshTokenValidityTime = configuration.GetValue<TimeSpan?>("Jwt:RefreshTokenValidityTime") ?? new TimeSpan(0, 30, 0);
            var jwtRefreshTokenSize = configuration.GetValue<int?>("Jwt:RefreshTokenSize") ?? 32;

            //  Initialize configuration.
            var config = new AuthServiceConfig(jwtIssuerSigningKey)
            {
                JwtAudience = applicationUrl,
                JwtIssuer = applicationUrl,
                JwtAccessTokenValidityTime = jwtAccessTokenValidityTime,
                JwtRefreshTokenValidityTime = jwtRefreshTokenValidityTime,
                JwtRefreshTokenSize = jwtRefreshTokenSize,
            };

            //  Register users service configuration.
            services.AddSingleton(config);

            return services;
        }

        #endregion REGISTRATION METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get application URL. </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <returns> Application URL. </returns>
        private static string GetApplicationUrl(IServiceCollection services, IConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(Program.URL))
                return Program.URL;

            var launchSettings = ApplicationUtilities.GetLaunchSettings();

            if (launchSettings == null)
                throw new Exception($"Launch settings could not be loaded.");

            var applicationUrl = launchSettings?.Profiles?.ArduinoConnectWeb?.ApplicationUrl?
                .Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(u => u.Trim())
                .FirstOrDefault();

            if (string.IsNullOrEmpty(applicationUrl))
                throw new Exception("Application URL not found in launch settings.");

            return applicationUrl;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get jwt issuer signing key. </summary>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> Symmetric security key. </returns>
        private static string GetJwtIssuerSigningKey(IConfiguration configuration)
        {
            string keysStorageFilePath = configuration["Storage:KeysStorageFile"] ?? DEFAULT_KEYS_FILE_PATH;

            KeysStorage? keysStorage = null;
            string? symmetricSecurityKey;

            try
            {
                var keysFileContent = File.ReadAllText(keysStorageFilePath);
                keysStorage = JsonConvert.DeserializeObject<KeysStorage>(keysFileContent);
            }
            catch (Exception)
            {
                //
            }

            if (keysStorage == null)
                keysStorage = new KeysStorage();

            if (keysStorage.JwtKey == null)
            {
                symmetricSecurityKey = SecurityUtilities.GenerateSymmetricSecurityKey();
                keysStorage.JwtKey = symmetricSecurityKey;

                var keysFileContent = JsonConvert.SerializeObject(keysStorage, Formatting.Indented);
                File.WriteAllText(keysStorageFilePath, keysFileContent);
            }
            else
            {
                symmetricSecurityKey = keysStorage.JwtKey;
            }

            return symmetricSecurityKey;
        }

        #endregion UTILITY METHODS

    }
}
