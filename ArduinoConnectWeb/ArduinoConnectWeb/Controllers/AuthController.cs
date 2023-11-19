using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base;
using ArduinoConnectWeb.Services.Auth;
using ArduinoConnectWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArduinoConnectWeb.Controllers
{
    [ApiController]
    [Route("api/v1/Auth")]
    public class AuthController : ControllerBase
    {

        //  VARIABLES

        private readonly IAuthService _authService;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AuthController class constructor. </summary>
        /// <param name="authService"> Interface of auth service. </param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #endregion CLASS METHODS

        #region GET AUTH CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get current user sessions. </summary>
        /// <returns> Sessions list or BadRequestObjectResult. </returns>
        [HttpGet("GetCurrentSessions")]
        [Authorize]
        public async Task<IActionResult> GetCurrentSessions()
        {
            var authorizationHeader = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.GetSessions(authorizationHeader);

            return response.IsSuccess
                ? new OkObjectResult(response.ResponseData)
                : CreateBadRequestObjectResultFromResponse(response);
        }

        #endregion GET AUTH CONTROLLER METHODS

        #region POST AUTH CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Login. </summary>
        /// <param name="requestLoginModel"> Request login model. </param>
        /// <returns> Session data or BadRequestObjectResult. </returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] RequestLoginModel requestLoginModel)
        {
            var response = await _authService.Login(requestLoginModel);

            return response.IsSuccess
                ? new OkObjectResult(response.ResponseData)
                : CreateBadRequestObjectResultFromResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Logout. </summary>
        /// <param name="requestLogoutModel"> Request logout model. </param>
        /// <returns> Success message or BadRequestObjectResult. </returns>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] RequestLogoutModel requestLogoutModel)
        {
            var response = await _authService.Logout(requestLogoutModel);

            return response.IsSuccess
                ? new OkObjectResult(response.ResponseData)
                : CreateBadRequestObjectResultFromResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Refresh tokens. </summary>
        /// <param name="requestRefreshModel"> Request refresh model. </param>
        /// <returns> Session data or BadRequestObjectResult. </returns>
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RequestRefreshModel requestRefreshModel)
        {
            var response = await _authService.Refresh(requestRefreshModel);

            return response.IsSuccess
                ? new OkObjectResult(response.ResponseData)
                : CreateBadRequestObjectResultFromResponse(response);
        }

        #endregion POST AUTH CONTROLLER METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create Bad request object result from response. </summary>
        /// <typeparam name="T"> Response data type. </typeparam>
        /// <param name="response"> Internal response. </param>
        /// <returns> Bad request object result. </returns>
        private BadRequestObjectResult CreateBadRequestObjectResultFromResponse<T>(ResponseBaseModel<T> response) where T : class
        {
            return new BadRequestObjectResult(new
            {
                Message = response.GetErrorMessagesAsOne()
            });
        }

        #endregion UTILITY METHODS


    }
}
