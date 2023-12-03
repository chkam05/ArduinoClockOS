using ArduinoConnectWeb.Services.Auth;
using ArduinoConnectWeb.Services.Network;
using ArduinoConnectWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArduinoConnectWeb.Controllers
{
    [ApiController]
    [Route("api/v1/Network")]
    public class NetworkController : ControllerBase
    {

        //  VARIABLES

        private readonly IAuthService _authService;
        private readonly INetworkService _networkService;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> NetworkController class constructor. </summary>
        /// <param name="authService"> Auth service interface. </param>
        /// <param name="networkService"> Network service interface. </param>
        public NetworkController(IAuthService authService, INetworkService networkService)
        {
            _authService = authService;
            _networkService = networkService;
        }

        #endregion CLASS METHODS

        #region GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get external host info async. </summary>
        /// <returns> External host info or BadRequestObjectResult. </returns>
        [HttpGet("GetExternalHostInfo")]
        [Authorize]
        public async Task<IActionResult> GetExternalHostInfo()
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken, _networkService.GetExternalHostInfoAsync);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get host info async. </summary>
        /// <returns> Host info or BadRequestObjectResult. </returns>
        [HttpGet("GetHostInfo")]
        [Authorize]
        public async Task<IActionResult> GetHostInfoAsync()
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken, _networkService.GetHostInfoAsync);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion GET METHODS

    }
}
