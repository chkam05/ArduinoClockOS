using ArduinoConnectWeb.Services.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;

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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion CLASS METHODS

        #region CONFIGURATION METHODS

        //  --------------------------------------------------------------------------------
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            ConfigureAuthenticationService(services);
            ConfigureSwaggerService(services);

            services.AddSingleton<IAuthService, AuthService>();
        }

        //  --------------------------------------------------------------------------------
        private void ConfigureAuthenticationService(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = "/Home/Index"; // Strona logowania
                options.LogoutPath = "/Home/Logout"; // Strona wylogowania
            });
        }

        //  --------------------------------------------------------------------------------
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
