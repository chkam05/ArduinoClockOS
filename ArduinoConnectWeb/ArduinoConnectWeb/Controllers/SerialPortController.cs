using ArduinoConnectWeb.Models.Serial.RequestModels;
using ArduinoConnectWeb.Services.Auth;
using ArduinoConnectWeb.Services.Serial;
using ArduinoConnectWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArduinoConnectWeb.Controllers
{
    [ApiController]
    [Route("api/v1/SerialPort")]
    public class SerialPortController : ControllerBase
    {

        //  VARIABLES

        private readonly IAuthService _authService;
        private readonly ISerialPortService _serialPortService;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SerialPortController class constructor. </summary>
        /// <param name="authService"> Auth service interface. </param>
        /// <param name="serialPortService"> Serial port service interface. </param>
        public SerialPortController(IAuthService authService, ISerialPortService serialPortService)
        {
            _authService = authService;
            _serialPortService = serialPortService;
        }

        #endregion CLASS METHODS

        #region GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get all ports async. </summary>
        /// <returns> Ports or BadRequestObjectResult. </returns>
        [HttpGet("GetAllPorts")]
        [Authorize]
        public async Task<IActionResult> GetAllPortsAsync()
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken, _serialPortService.GetAllPortsAsync);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get all available ports async. </summary>
        /// <returns> Ports or BadRequestObjectResult. </returns>
        [HttpGet("GetAvailablePorts")]
        [Authorize]
        public async Task<IActionResult> GetAvailablePortsAsync()
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken, _serialPortService.GetAvailablePortsAsync);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get connection by id async. </summary>
        /// <param name="id"> Connection identifier. </param>
        /// <returns> Serial port connection or BadRequestObjectResult. </returns>
        [HttpGet("GetConnectionById")]
        [Authorize]
        public async Task<IActionResult> GetConnectionByIdAsync([FromQuery] string id)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _serialPortService.GetConnectionByIdAsync(session, id));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get all connections async. </summary>
        /// <returns> Serial port connections or BadRequestObjectResult. </returns>
        [HttpGet("GetAllConnections")]
        [Authorize]
        public async Task<IActionResult> GetAllConnectionsAsync()
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken, _serialPortService.GetAllConnectionsAsync);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get current user connections async. </summary>
        /// <returns> Serial port connections or BadRequestObjectResult. </returns>
        [HttpGet("GetOwnConnections")]
        [Authorize]
        public async Task<IActionResult> GetOwnConnectionsAsync()
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken, _serialPortService.GetOwnConnectionsAsync);

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get message async. </summary>
        /// <param name="id"> Connection identifier. </param>
        /// <returns> Response message or BadRequestObjectResult. </returns>
        [HttpGet("GetMessage")]
        [Authorize]
        public async Task<IActionResult> GetMessageAsync([FromQuery] string id)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _serialPortService.GetMessageAsync(session, id));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get message async. </summary>
        /// <param name="id"> Connection identifier. </param>
        /// <returns> Response message or BadRequestObjectResult. </returns>
        [HttpGet("GetLastMessage")]
        [Authorize]
        public async Task<IActionResult> GetLastMessageAsync([FromQuery] string id)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _serialPortService.GetLastMessageAsync(session, id));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion GET METHODS

        #region POST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Connect async. </summary>
        /// <param name="requestModel"> Open connection request model. </param>
        /// <returns> Serial port connection or BadRequestObjectResult. </returns>
        [HttpPost("Connect")]
        [Authorize]
        public async Task<IActionResult> ConnectAsync([FromBody] OpenConnectionRequestModel requestModel)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _serialPortService.OpenConnectionAsync(session, requestModel));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Disconnect async. </summary>
        /// <param name="connectionId"> Connection identifier. </param>
        /// <returns> Message or BadRequestObjectResult. </returns>
        [HttpPost("Disconnect")]
        [Authorize]
        public async Task<IActionResult> DisconnectAsync([FromQuery] string connectionId)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _serialPortService.CloseConnectionAsync(session, connectionId));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Send and receive message async. </summary>
        /// <param name="id"> Connection identifier. </param>
        /// <param name="requestModel"> Send and receive message request model. </param>
        /// <returns> Response message or BadRequestObjectResult. </returns>
        [HttpPost("SendAndReceiveMessage")]
        [Authorize]
        public async Task<IActionResult> SendAndReceiveMessageAsync(
            [FromQuery] string id,
            [FromBody] SendAndReceiveMessageRequestModel requestModel)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _serialPortService.SendAndReceiveMessageAsync(session, id, requestModel));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Send message async. </summary>
        /// <param name="id"> Connection identifier. </param>
        /// <param name="requestModel"> Send message request model. </param>
        /// <returns> Message or BadRequestObjectResult. </returns>
        [HttpPost("SendMessage")]
        [Authorize]
        public async Task<IActionResult> SendMessageAsync(
            [FromQuery] string id,
            [FromBody] SendMessageRequestModel requestModel)
        {
            var accessToken = ControllerUtilities.GetAuthorizationToken(HttpContext);

            var response = await _authService.ProcessTaskAsyncWithAuthorization(accessToken,
                session => _serialPortService.SendMessageAsync(session, id, requestModel));

            return ControllerUtilities.CreateHttpObjectResponse(response);
        }

        #endregion POST METHODS

    }
}
