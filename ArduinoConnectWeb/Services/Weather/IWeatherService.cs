using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Weather;

namespace ArduinoConnectWeb.Services.Weather
{
    public interface IWeatherService
    {

        //  METHODS

        public Task<BaseResponseModel<WeatherDataModel>> GetWeatherByCityAsync(SessionDataModel session, string? cityName);

    }
}
