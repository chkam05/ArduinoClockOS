using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ArduinoConnectWeb.Services
{
    public static class SwaggerServiceExtension
    {

        //  METHODS

        #region REGISTRATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Register and configure swagger service.  </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </param>
        public static IServiceCollection RegisterSwaggerService(this IServiceCollection services, IConfiguration configuration)
        {
            //  Add swagger.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    name: "v1",
                    info: new OpenApiInfo
                    {
                        Description = configuration["ServiceDescription"],
                        Title = $"{configuration["ServiceName"]}V1",
                        Version = "v1",
                    });
            });

            return services;
        }

        #endregion REGISTRATION METHODS

    }
}
