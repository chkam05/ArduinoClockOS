using Microsoft.AspNetCore.Hosting;

namespace ArduinoConnectWeb
{
    public class Program
    {

        #region APPLICATION METHODS

        //  --------------------------------------------------------------------------------
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        #endregion APPLICATION METHODS

        #region WEB SERVICE METHODS

        //  --------------------------------------------------------------------------------
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

        #endregion WEB SERVICE METHODS

    }
}