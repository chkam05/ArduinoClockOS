using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Exceptions;
using ArduinoConnectWeb.Models.Weather;
using ArduinoConnectWeb.Models.Weather.ResponseModels;
using ArduinoConnectWeb.Services.Base;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;

namespace ArduinoConnectWeb.Services.Weather
{
    public class WeatherService : DataProcessor, IWeatherService
    {

        //  CONST

        private const string MEDIA_TYPE = "application/json";
        private const string URL = "https://wttr.in/{0}?format=j1";


        //  VARIABLES

        private readonly WeatherServiceConfig _config;
		
		
		//  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> WeatherService class constructor. </summary>
        /// <param name="config"> Weather service config. </param>
        /// <param name="logger"> Application logger. </param>
        public WeatherService(WeatherServiceConfig config, ILogger<WeatherService> logger) : base(logger)
        {
            _config = config;
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get weather data async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <param name="cityName"> City name. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<WeatherListResponseModel>> GetWeatherAsync(SessionDataModel session, string? cityName)
        {
            return await ProcessTaskAsync(async () =>
            {
                if (string.IsNullOrEmpty(cityName))
                    throw new ProcessingException("Invalid city name", StatusCodes.Status400BadRequest);

                var (isSuccess, weatherData, errorMessage) = await DownloadWeatherRaw(cityName);

                if (isSuccess && weatherData?.Weather != null)
                {
                    var weatherResponse = new WeatherListResponseModel()
                    {
                        Weather = weatherData.Weather.Select(w => new WeatherItemResponseModel(w)).ToList()
                    };

                    return new BaseResponseModel<WeatherListResponseModel>(weatherResponse);
                }
                else
                    throw new ProcessingException(errorMessage, StatusCodes.Status400BadRequest);
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get raw weather data async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <param name="cityName"> City name. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<WeatherDataModel>> GetWeatherRawAsync(SessionDataModel session, string? cityName)
        {
            return await ProcessTaskAsync(async () =>
            {
                if (string.IsNullOrEmpty(cityName))
                    throw new ProcessingException("Invalid city name", StatusCodes.Status400BadRequest);

                var (isSuccess, weatherData, errorMessage) = await DownloadWeatherRaw(cityName);

                if (isSuccess)
                    return new BaseResponseModel<WeatherDataModel>(weatherData);
                else
                    throw new ProcessingException(errorMessage, StatusCodes.Status400BadRequest);
            });
        }

        #endregion INTERACTION METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Download raw weather data. </summary>
        /// <param name="cityName"> City name. </param>
        /// <returns> (Is success; Raw weather data; Error message) </returns>
        private async Task<(bool, WeatherDataModel?, string?)> DownloadWeatherRaw(string? cityName)
        {
            var client = new HttpClient();
            var meidaType = new MediaTypeWithQualityHeaderValue(WeatherServiceConfig.MEDIA_TYPE);
            var url = string.Format(WeatherServiceConfig.URL, cityName);

            client.DefaultRequestHeaders.Accept.Add(meidaType);
            client.Timeout = _config.TimeOut;

            var responseMsg = await client.GetAsync(url);

            if (responseMsg.IsSuccessStatusCode)
            {
                string content = await responseMsg.Content.ReadAsStringAsync();
                var weatherData = JsonConvert.DeserializeObject<WeatherDataModel>(content);
                return (true, weatherData, null);
            }
            else
            {
                string content = await responseMsg.Content.ReadAsStringAsync();
                return (false, null, content);
            }
        }

        #endregion UTILITY METHODS

    }
}
