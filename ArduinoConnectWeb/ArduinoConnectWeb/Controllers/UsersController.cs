using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Users.RequestModels;
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

        #region DELETE USER CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Remove user by id. </summary>
        /// <param name="id"> User identifier. </param>
        /// <returns> Success message or BadRequestObjectResult. </returns>
        [HttpDelete("DeleteUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromQuery] string id)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);
            var response = await _usersService.RemoveUserAsync(accessToken, _authService, id);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion DELETE USER CONTROLLER METHODS

        #region GET USER CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by identifier. </summary>
        /// <param name="id"> User identifier. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpGet("GetUserById")]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromQuery] string id)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);
            var response = await _usersService.GetUserByIdAsync(accessToken, _authService, id);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by user name. </summary>
        /// <param name="userName"> User name. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpGet("GetUserByUserName")]
        [Authorize]
        public async Task<IActionResult> GetUserByUserName([FromQuery] string userName)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);
            var response = await _usersService.GetUserByUserNameAsync(accessToken, _authService, userName);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get users list. </summary>
        /// <returns> Users list or BadRequestObjectResult. </returns>
        [HttpGet("GetUsersList")]
        [Authorize]
        public async Task<IActionResult> GetUsersList()
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);
            var response = await _usersService.GetUsersListAsync(accessToken, _authService);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion GET USER CONTROLLER METHODS

        #region POST USER CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create new user. </summary>
        /// <param name="request"> Request user create model. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpPost("CreateUser")]
        [Authorize]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestModel request)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);
            var response = await _usersService.CreateUserAsync(accessToken, _authService, request);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion POST USER CONTROLLER METHODS

        #region UPDATE USER CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update user. </summary>
        /// <param name="id"> User identifier. </param>
        /// <param name="request"> Request user update model. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpPatch("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(
            [FromQuery] string id,
            [FromBody] UpdateUserRequestModel request)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);
            var response = await _usersService.UpdateUserAsync(accessToken, _authService, id, request);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion UPDATE USER CONTROLLER METHODS

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
