
using ArduinoConnectWeb.Utilities;
using Microsoft.AspNetCore.Hosting;

namespace ArduinoConnectWeb
{
    public class Program
    {

        //  METHODS

        #region APPLICATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Application startup method. </summary>
        /// <param name="args"> Application arguments. </param>
        public static void Main(string[] args)
        {
            var argsDict = ApplicationUtilities.GetArguments(args);

            if (argsDict.ContainsKey("custom"))
                CreateCustomHostBuilder(argsDict).Build().Run();
            else
                CreateHostBuilder(args).Build().Run();
        }

        #endregion APPLICATION METHODS

        #region WEB SERVICE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create host builder. </summary>
        /// <param name="args"> Application arguments. </param>
        /// <returns> Host builder interface. </returns>
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create custom host builder. </summary>
        /// <param name="argsDict"> Application arguments dictionary. </param>
        /// <returns> Host builder interface. </returns>
        private static IHostBuilder CreateCustomHostBuilder(Dictionary<string, string> argsDict)
        {
            string host = argsDict.ContainsKey("host") ? argsDict["host"] : "127.0.0.1";
            string port = argsDict.ContainsKey("port") ? argsDict["port"] : "5000";
            string protocol = argsDict.ContainsKey("usehttps") ? "https" : "http";

            return Host
                .CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls($"{protocol}://{host}:{port}");
                });
        }

        #endregion WEB SERVICE METHODS

    }
}