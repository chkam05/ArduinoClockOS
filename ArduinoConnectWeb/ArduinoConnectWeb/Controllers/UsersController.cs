using ArduinoConnectWeb.Models.Base;
using ArduinoConnectWeb.Models.Users;
using ArduinoConnectWeb.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace ArduinoConnectWeb.Controllers
{
    [ApiController]
    [Route("api/v1/Users")]
    public class UsersController : ControllerBase
    {

        //  VARIABLES

        private readonly IUsersService _usersService;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UsersController class constructor. </summary>
        /// <param name="usersService"> Interface of users service. </param>
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        #endregion CLASS METHODS

        #region DELETE USER CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Remove user by id. </summary>
        /// <param name="id"> User identifier. </param>
        /// <returns> Success message or BadRequestObjectResult. </returns>
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] string id)
        {
            var response = await _usersService.RemoveUser(id);

            return response.IsSuccess
                ? new OkObjectResult(response.ResponseData)
                : CreateBadRequestObjectResultFromResponse(response);
        }

        #endregion DELETE USER CONTROLLER METHODS

        #region GET USER CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by identifier. </summary>
        /// <param name="id"> User identifier. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
                return new BadRequestObjectResult(new { Message = $"The \"{nameof(id)}\" parameter cannot be empty" });

            var response = await _usersService.GetUserById(id);

            return response.IsSuccess
                ? new OkObjectResult(response.ResponseData)
                : CreateBadRequestObjectResultFromResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user by user name. </summary>
        /// <param name="userName"> User name. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpGet("GetUserByUserName")]
        public async Task<IActionResult> GetUserByUserName([FromQuery] string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return new BadRequestObjectResult(new { Message = $"The \"{nameof(userName)}\" parameter cannot be empty" });

            var response = await _usersService.GetUserByUserName(userName);

            return response.IsSuccess
                ? new OkObjectResult(response.ResponseData)
                : CreateBadRequestObjectResultFromResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get users list. </summary>
        /// <returns> Users list or BadRequestObjectResult. </returns>
        [HttpGet("GetUsersList")]
        public async Task<IActionResult> GetUsersList()
        {
            var response = await _usersService.GetUsersList();

            return response.IsSuccess
                ? new OkObjectResult(response.ResponseData)
                : CreateBadRequestObjectResultFromResponse(response);
        }

        #endregion GET USER CONTROLLER METHODS

        #region POST USER CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create new user. </summary>
        /// <param name="request"> Request user create model. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] RequestUserCreateModel request)
        {
            var response = await _usersService.CreateUser(request);

            return response.IsSuccess
                ? new OkObjectResult(response.ResponseData)
                : CreateBadRequestObjectResultFromResponse(response);
        }

        #endregion POST USER CONTROLLER METHODS

        #region UPDATE USER CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update user. </summary>
        /// <param name="id"> User identifier. </param>
        /// <param name="request"> Request user update model. </param>
        /// <returns> User data or BadRequestObjectResult. </returns>
        [HttpPatch("UpdateUser")]
        public async Task<IActionResult> UpdateUser(
            [FromQuery] string id,
            [FromBody] RequestUserUpdateModel request)
        {
            var response = await _usersService.UpdateUser(id, request);

            return response.IsSuccess
                ? new OkObjectResult(response.ResponseData)
                : CreateBadRequestObjectResultFromResponse(response);
        }

        #endregion UPDATE USER CONTROLLER METHODS

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
