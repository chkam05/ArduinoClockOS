using ArduinoConnectWeb.Services.Auth;
using ArduinoConnectWeb.Services.Weather;
using ArduinoConnectWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArduinoConnectWeb.Controllers
{
    [ApiController]
    [Route("api/v1/Weather")]
    public class WeatherController : ControllerBase
    {

        //  VARIABLES

        private readonly IAuthService _authService;
        private readonly IWeatherService _weatherService;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> WeatherController class constructor. </summary>
        /// <param name="authService"> Auth service interface. </param>
        /// <param name="weatherService"> Weather service interface. </param>
        public WeatherController(IAuthService authService, IWeatherService weatherService)
        {
            _authService = authService;
            _weatherService = weatherService;
        }

        #endregion CLASS METHODS

        #region GET METHDOS

        //  --------------------------------------------------------------------------------
        /// <summary> Get weather by city async. </summary>
        /// <param name="cityName"> City name. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpGet("GetWeatherByCity")]
        [Authorize]
        public async Task<IActionResult> GetWeatherByCityAsync([FromQuery] string cityName)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _weatherService.GetWeatherByCityAsync(session, cityName));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion GET METHODS

    }
}
