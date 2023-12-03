namespace ArduinoConnectWeb.Services.Weather
{
    public static class WeatherServiceExtension
    {

        //  METHODS

        #region REGISTRATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Register and configure Weather service.  </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </param>
        public static IServiceCollection RegisterWeatherSerivce(this IServiceCollection services, IConfiguration configuration)
        {
            //  Register Weather service configuration.
            services.RegisterConfiguration(configuration);

            //  Register Weather service.
            services.AddTransient<IWeatherService, WeatherService>();

            return services;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Register Weather service configuration. </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </param>
        private static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            //  Initialize configuration.
            var config = new WeatherServiceConfig()
            {
                TimeOut = configuration.GetValue<TimeSpan>("WeatherServiceConfig:Timeout")
            };

            //  Register Weather service configuration.
            services.AddSingleton(config);

            return services;
        }

        #endregion REGISTRATION METHODS

    }
}
