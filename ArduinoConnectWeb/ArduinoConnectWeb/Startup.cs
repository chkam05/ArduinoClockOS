using ArduinoConnectWeb.Services.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using System.Net;


namespace ArduinoConnectWeb
{
    public class Startup
    {

        //  VARIABLES

        private readonly string _serviceName = "ArduinoConnect";

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{_serviceName} API V1");
            });

            //  Configure routing.
            app.UseRouting();

            //  Configure autorization middleware.
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
            services.AddSingleton(Configuration);
            services.AddSingleton<IUsersService, UsersService>();
            services.AddControllersWithViews();

            ConfigureSwaggerService(services);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Configure swagger service. </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application </param>
        private void ConfigureSwaggerService(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = $"{_serviceName}V1",
                    Version = "v1",
                });
            });
        }

        #endregion SERVICES CONFIGURATION METHODS

    }
}
