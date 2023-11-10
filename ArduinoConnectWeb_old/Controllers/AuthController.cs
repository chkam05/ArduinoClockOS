using ArduinoConnectWeb.Services.Authentication;
using ArduinoConnectWeb.Services.Authentication.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArduinoConnectWeb.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {

        //  VARIABLES

        private readonly IAuthService _authService;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #endregion CLASS METHODS

        #region LOGIN CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequestModel)
        {
            if (loginRequestModel != null)
            {
                var tokens = await _authService.LoginFromApi(
                    loginRequestModel.Login, loginRequestModel.Password);

                if (tokens != null)
                    return new OkObjectResult(tokens);
            }

            return new BadRequestResult();
        }

        #endregion LOGIN CONTROLLER METHODS

    }
}
