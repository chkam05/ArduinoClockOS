using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Exceptions;
using ArduinoConnectWeb.Models.Weather;
using ArduinoConnectWeb.Services.Base;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
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
        /// <param name="cityName"> City. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<WeatherDataModel>> GetWeatherByCityAsync(SessionDataModel session, string? cityName)
        {
            return await ProcessTaskAsync(async () =>
            {
                if (string.IsNullOrEmpty(cityName))
                    throw new ProcessingException("Invalid city name", StatusCodes.Status400BadRequest);

                var client = new HttpClient();
                var meidaType = new MediaTypeWithQualityHeaderValue(MEDIA_TYPE);
                var url = string.Format(URL, cityName);

                client.DefaultRequestHeaders.Accept.Add(meidaType);
                client.Timeout = _config.TimeOut;

                var responseMsg = await client.GetAsync(url);

                if (responseMsg.IsSuccessStatusCode)
                {
                    string content = await responseMsg.Content.ReadAsStringAsync();
                    var weatherData = JsonConvert.DeserializeObject<WeatherDataModel>(content);

                    return new BaseResponseModel<WeatherDataModel>(weatherData);
                }
                else
                {
                    string content = await responseMsg.Content.ReadAsStringAsync();
                    throw new ProcessingException(content, StatusCodes.Status400BadRequest);
                }
            });
        }

        #endregion INTERACTION METHODS

    }
}
