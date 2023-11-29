namespace ArduinoConnectWeb.Services.NetworkService
{
    public static class NetworkServiceExtension
    {

        //  METHODS

        #region REGISTRATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Register and configure network service.  </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </param>
        public static IServiceCollection RegisterNetworkService(this IServiceCollection services, IConfiguration configuration)
        {
            //  Register users service configuration.
            services.RegisterConfiguration(configuration);

            //  Register users service.
            services.AddTransient<INetworkService, NetworkService>();

            return services;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Register network service configuration. </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </param>
        private static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            //  Initialize configuration.
            var config = new NetworkServiceConfig()
            {
                //
            };

            //  Register network service configuration.
            services.AddSingleton(config);

            return services;
        }

        #endregion REGISTRATION METHODS

    }
}
