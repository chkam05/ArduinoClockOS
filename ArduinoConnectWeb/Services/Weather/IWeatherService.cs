using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Weather;
using ArduinoConnectWeb.Models.Weather.ResponseModels;

namespace ArduinoConnectWeb.Services.Weather
{
    public interface IWeatherService
    {

        //  METHODS

        public Task<BaseResponseModel<WeatherDataModel>> GetWeatherRawAsync(SessionDataModel session, string? cityName);
        public Task<BaseResponseModel<WeatherListResponseModel>> GetWeatherAsync(SessionDataModel session, string? cityName);

    }
}
