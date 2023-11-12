using ArduinoConnectWeb.Models.Config;
using ArduinoConnectWeb.Services.Users;
using ArduinoConnectWeb.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

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
                    IssuerSigningKey = jwtIssuerSigningKey,
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
        private static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationUrl = GetApplicationUrl(services, configuration);
            var jwtIssuerSigningKey = GetJwtIssuerSigningKey(configuration);
            var jwtAccessTokenValidityTime = configuration.GetValue<TimeSpan?>("Jwt:AccessTokenValidityTime") ?? new TimeSpan(0, 15, 0);
            var jwtRefreshTokenValidityTime = configuration.GetValue<TimeSpan?>("Jwt:RefreshTokenValidityTime") ?? new TimeSpan(0, 30, 0);
            var jwtRefreshTokenSize = configuration.GetValue<int?>("Jwt:RefreshTokenSize") ?? 32;

            //  Initialize configuration.
            var config = new AuthServiceConfig()
            {
                JwtAudience = applicationUrl,
                JwtIssuer = applicationUrl,
                JwtKey = SecurityUtilities.EncodeSymmetricSecurityKey(jwtIssuerSigningKey),
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
        private static string? GetApplicationUrl(IServiceCollection services, IConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(Program.URL))
                return Program.URL;

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var launchSettings = configuration.GetSection($"profiles:{environment}");
            var applicationUrl = launchSettings["applicationUrl"];

            if (!string.IsNullOrEmpty(applicationUrl))
            {
                if (applicationUrl.Contains(";"))
                    return applicationUrl.Split(";").First();
                else
                    return applicationUrl;
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get jwt issuer signing key. </summary>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> Symmetric security key. </returns>
        private static SymmetricSecurityKey GetJwtIssuerSigningKey(IConfiguration configuration)
        {
            string keysStorageFilePath = configuration["Storage:KeysStorageFile"] ?? DEFAULT_KEYS_FILE_PATH;

            KeysStorage? keysStorage = null;
            SymmetricSecurityKey? symmetricSecurityKey;

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
                keysStorage.JwtKey = SecurityUtilities.EncodeSymmetricSecurityKey(symmetricSecurityKey);

                var keysFileContent = JsonConvert.SerializeObject(keysStorage, Formatting.Indented);
                File.WriteAllText(keysStorageFilePath, keysFileContent);
            }
            else
            {
                symmetricSecurityKey = SecurityUtilities.DecodeSymmetricSecurityKey(keysStorage.JwtKey);
            }

            return symmetricSecurityKey;
        }

        #endregion UTILITY METHODS

    }
}
