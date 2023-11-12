using ArduinoConnectWeb.Models.Config;
using ArduinoConnectWeb.Services;
using ArduinoConnectWeb.Services.Auth;
using ArduinoConnectWeb.Services.Users;
using ArduinoConnectWeb.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace ArduinoConnectWeb
{
    public class Startup
    {

        //  VARIABLES

        public IConfiguration Configuration { get; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Startup class constructor. </summary>
        /// <param name="configuration"> Interface of application configuration properties. </param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion CLASS METHODS

        #region CONFIGURATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Configure application runtime environment. </summary>
        /// <param name="app"> ApplicationBuilder interface for configuring runtime components. </param>
        /// <param name="env"> WebHostEnvironment interface that provides information about the environment. </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //  Developer mode configuration.
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            //  Configure authentication middleware.
            app.UseAuthentication();

            //  Configure swagger middleware.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    url: "/swagger/v1/swagger.json",
                    name: $"{Configuration["ServiceName"]} API V1");
            });

            //  Configure routing.
            app.UseRouting();

            //  Configure authentication & autorization middleware.
            app.UseAuthentication();
            app.UseAuthorization();

            //  Configure endpoints.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=HomePage}/{id?}");
            });
        }

        #endregion CONFIGURATION METHODS

        #region SERVICES CONFIGURATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Configure and register services that will be available in application. </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services.SetupAuthentication(Configuration);

            services.RegisterUsersService(Configuration);

            services.AddControllersWithViews();
            services.RegisterSwaggerService(Configuration);
        }

        #endregion SERVICES CONFIGURATION METHODS

    }
}
