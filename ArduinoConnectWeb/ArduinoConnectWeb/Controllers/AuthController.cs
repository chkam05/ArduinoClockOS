using Microsoft.AspNetCore.Mvc;

namespace ArduinoConnectWeb.Controllers
{
    [ApiController]
    [Route("api/v1/Auth")]
    public class AuthController : ControllerBase
    {

        //  VARIABLES

        private readonly IConfiguration _configuration;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AuthController class constructor. </summary>
        /// <param name="configuration"> Interface of application configuration properties. </param>
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion CLASS METHODS

    }
}
