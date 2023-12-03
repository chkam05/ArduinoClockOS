using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Users.RequestModels;
using ArduinoConnectWeb.Models.Users.ResponseModels;
using ArduinoConnectWeb.Services.Auth;
using ArduinoConnectWeb.Services.Users;
using ArduinoConnectWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArduinoConnectWeb.Controllers
{
    [ApiController]
    [Route("api/v1/Users")]
    public class UsersController : ControllerBase
    {

        //  VARIABLES

        private readonly IAuthService _authService;
        private readonly IUsersService _usersService;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UsersController class constructor. </summary>
        /// <param name="authService"> Auth service interface. </param>
        /// <param name="usersService"> Users service interface. </param>
        public UsersController(IAuthService authService, IUsersService usersService)
        {
            _authService = authService;
            _usersService = usersService;
        }

        #endregion CLASS METHODS

        #region DELETE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Remove user by id async. </summary>
        /// <param name="id"> User identifier. </param>
        /// <returns> Success message or BadRequestObjectResult. </returns>
        [HttpDelete("DeleteUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUserAsync([FromQuery] string id)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _usersService.RemoveUserAsync(id));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion DELETE METHODS

        #region GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by identifier async. </summary>
        /// <param name="id"> User identifier. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpGet("GetUserById")]
        [Authorize]
        public async Task<IActionResult> GetUserByIdAsync([FromQuery] string id)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _usersService.GetUserByIdAsync(id));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by user name async. </summary>
        /// <param name="userName"> User name. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpGet("GetUserByUserName")]
        [Authorize]
        public async Task<IActionResult> GetUserByUserNameAsync([FromQuery] string userName)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _usersService.GetUserByUserNameAsync(userName));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get users list async. </summary>
        /// <returns> Users list or BadRequestObjectResult. </returns>
        [HttpGet("GetUsersList")]
        [Authorize]
        public async Task<IActionResult> GetUsersListAsync()
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _usersService.GetUsersListAsync());

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion GET METHODS

        #region POST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create new user async. </summary>
        /// <param name="requestModel"> Request user create model. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpPost("CreateUser")]
        [Authorize]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequestModel requestModel)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _usersService.CreateUserAsync(requestModel));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion POST METHODS

        #region UPDATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update user async. </summary>
        /// <param name="id"> User identifier. </param>
        /// <param name="requestModel"> Request user update model. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpPatch("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync(
            [FromQuery] string id,
            [FromBody] UpdateUserRequestModel requestModel)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _usersService.UpdateUserAsync(id, requestModel));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion UPDATE METHODS

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
