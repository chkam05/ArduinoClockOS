using ArduinoConnectWeb.Models.Config;
using Newtonsoft.Json;

namespace ArduinoConnectWeb.Utilities
{
    public static class ApplicationUtilities
    {

        //  CONST

        private const string PROPERTIES_FILE_PATH = "Properties\\launchSettings.json";


        //  METHODS

        #region ARGUMENTS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Read arguments and return as dictionary (param name: value). </summary>
        /// <param name="args"> Array of arguments. </param>
        /// <returns> Arguments dictionary. </returns>
        public static Dictionary<string, string> GetArguments(string[] args)
        {
            var arguments = new Dictionary<string, string>();

            string? key = null;
            int keyCounter = 0;

            foreach (var arg in args)
            {
                if (key != null)
                {
                    if (IsArgumentKey(arg))
                    {
                        arguments.Add(key, string.Empty);
                        key = null;
                    }
                    else
                    {
                        arguments.Add(key, arg);
                        key = null;
                        continue;
                    }
                }

                if (IsArgumentKey(arg))
                {
                    key = arg.Substring(1).Trim().ToLower();
                    continue;
                }
                else
                {
                    arguments.Add($"param{keyCounter}", arg);
                    keyCounter++;
                }

                key = null;
            }

            return arguments;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if argument is argument key. </summary>
        /// <param name="arg"> Argument. </param>
        /// <returns> True - argument is argument key; False - otherwise. </returns>
        private static bool IsArgumentKey(string arg)
        {
            return arg.StartsWith("/") || arg.StartsWith("-");
        }

        #endregion ARGUMENTS MANAGEMENT METHODS

        #region CONFIGURATION MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get application launch settings. </summary>
        /// <returns> Application launch settings. </returns>
        public static LaunchSettings? GetLaunchSettings()
        {
            var launchSettingsPath = FindLaunchSettingsPath();

            if (!string.IsNullOrEmpty(launchSettingsPath))
            {
                try
                {
                    var serializedData = File.ReadAllText(launchSettingsPath);
                    var launchSettings = JsonConvert.DeserializeObject<LaunchSettings>(serializedData);

                    if (launchSettings != null)
                        return launchSettings;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Find launch settings file path. </summary>
        /// <returns> Launch settings file path or null. </returns>
        private static string? FindLaunchSettingsPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            while (currentDirectory != null)
            {
                var launchSettingsPath = Path.Combine(currentDirectory, PROPERTIES_FILE_PATH);

                if (File.Exists(launchSettingsPath))
                    return launchSettingsPath;

                currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
            }

            return null;
        }

        #endregion CONFIGURATION MANAGEMENT METHODS

    }
}
