﻿using ArduinoConnectWeb.Models.Auth.RequestModels;
using ArduinoConnectWeb.Models.Base.ResponseModels;
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

        #region GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get current user sessions async. </summary>
        /// <returns> Sessions list or BadRequestObjectResult. </returns>
        [HttpGet("GetCurrentSessions")]
        [Authorize]
        public async Task<IActionResult> GetCurrentSessionsAsync()
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);
            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken, _authService.GetSessionsAsync);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion GET METHODS

        #region POST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Login async. </summary>
        /// <param name="requestLoginModel"> Request login model. </param>
        /// <returns> Session data or BadRequestObjectResult. </returns>
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestModel requestLoginModel)
        {
            var response = await _authService.LoginAsync(requestLoginModel);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Logout async. </summary>
        /// <returns> Success message or BadRequestObjectResult. </returns>
        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);
            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken, _authService.LogoutAsync);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Logout from all sessions async. </summary>
        /// <returns> Success message or BadRequestObjectResult. </returns>
        [HttpPost("LogoutAllSessions")]
        [Authorize]
        public async Task<IActionResult> LogoutAllSessionsAsync()
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);
            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken, _authService.LogoutAllSessionsAsync);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Refresh tokens async. </summary>
        /// <param name="refreshRequestModel"> Refresh request model. </param>
        /// <returns> Session data or BadRequestObjectResult. </returns>
        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshRequestModel refreshRequestModel)
        {
            var response = await _authService.RefreshAsync(refreshRequestModel);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion POST METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create Bad request object result from response. </summary>
        /// <typeparam name="T"> Response data type. </typeparam>
        /// <param name="response"> Internal response. </param>
        /// <returns> Bad request object result. </returns>
        private BadRequestObjectResult CreateBadRequestObjectResultFromResponse<T>(BaseResponseModel<T> response) where T : class
        {
            return new BadRequestObjectResult(new
            {
                Message = response.GetErrorMessagesAsOne()
            });
        }

        #endregion UTILITY METHODS


    }
}