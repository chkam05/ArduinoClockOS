

namespace ArduinoConnectWeb.Services.Serial
{
    public static class SerialPortServiceExtension
    {

        //  METHODS

        #region REGISTRATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Register and configure serial port service.  </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </param>
        public static IServiceCollection RegisterSerialPortService(this IServiceCollection services, IConfiguration configuration)
        {
            //  Register users service configuration.
            services.RegisterConfiguration(configuration);

            //  Register users service.
            services.AddSingleton<ISerialPortService, SerialPortService>();

            return services;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Register serial port service configuration. </summary>
        /// <param name="services"> ServiceCollection interface that contains collection of services available in application. </param>
        /// <param name="configuration"> Application configuration. </param>
        /// <returns> ServiceCollection interface that contains collection of services available in application. </param>
        private static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            //  Initialize configuration.
            var config = new SerialPortServiceConfig()
            {
                //
            };

            //  Register users service configuration.
            services.AddSingleton(config);

            return services;
        }

        #endregion REGISTRATION METHODS

    }
}
