using ArudinoConnect.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ArudinoConnect.Utilities
{
    public class WeatherDownloader
    {

        //  CONST

        private const string MEDIA_TYPE = "application/json";
        private const string URL = "https://wttr.in/{0}?format=j1";


        //  VARIABLES

        public string City { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> WeatherDownloader class constructor. </summary>
        /// <param name="city"> City for which the weather is checked. </param>
        public WeatherDownloader(string city = null)
        {
            City = !string.IsNullOrEmpty(city) ? city : string.Empty;
        }

        #endregion CLASS METHODS

        #region DOWNLOAD METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Download weather. </summary>
        /// <returns> Weather response object. </returns>
        public WeahterResponse DownloadWeather()
        {
            var client = new HttpClient();
            var meidaType = new MediaTypeWithQualityHeaderValue(MEDIA_TYPE);
            var url = string.Format(URL, City);

            client.DefaultRequestHeaders.Accept.Add(meidaType);

            var response = new WeahterResponse();

            try
            {
                var responseMsg = client.GetAsync(url).Result;

                if (responseMsg.IsSuccessStatusCode)
                {
                    string content = responseMsg.Content.ReadAsStringAsync().Result;
                    var weatherData = JsonConvert.DeserializeObject<WeatherData>(content);

                    if (weatherData != null)
                    {
                        response.Success = true;
                        response.Data = weatherData;
                        response.Error = null;
                    }
                }
                else
                {
                    int code = (int)responseMsg.StatusCode;
                    string codeName = responseMsg.StatusCode.ToString();

                    response.Success = false;
                    response.Error = $"Downloading failed with code: {codeName} [{code}]";
                }
            }
            catch (Exception exc)
            {
                response.Success = false;
                response.Error = $"Downloading failed with message: {exc.Message}";
            }

            return response;
        }

        #endregion DOWNLOAD METHODS

    }
}
