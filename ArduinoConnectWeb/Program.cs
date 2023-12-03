using ArduinoConnectWeb.Utilities;
using Microsoft.AspNetCore.Hosting;

namespace ArduinoConnectWeb
{
    public class Program
    {

        //  VARIABLES

        public static string? URL { get; set; }


        //  METHODS

        #region APPLICATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Application startup method. </summary>
        /// <param name="args"> Application arguments. </param>
        public static void Main(string[] args)
        {
            var argsDict = ApplicationUtilities.GetArguments(args);

            if (argsDict.ContainsKey("help"))
            {
                ShowHelp();
                return;
            }

            if (argsDict.ContainsKey("custom"))
                CreateCustomHostBuilder(argsDict).Build().Run();
            else
                CreateHostBuilder(args).Build().Run();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show application help. </summary>
        private static void ShowHelp()
        {
            Console.WriteLine(" --- ArduinoConnectWeb Help --- ");
            Console.WriteLine("");
            Console.WriteLine(" /custom           - use custom configuration. ");
            Console.WriteLine(" /help             - show help. ");
            Console.WriteLine(" /host [127.0.0.1] - set application host url (required /custom option). ");
            Console.WriteLine(" /port [5000]      - set application port (required /custom option). ");
            Console.WriteLine(" /usehttps         - use HTTPS instead of HTTP (required /custom option). ");
            Console.WriteLine("");
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

            URL = $"{protocol}://{host}:{port}/";

            return Host
                .CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(URL);
                });
        }

        #endregion WEB SERVICE METHODS

    }
}