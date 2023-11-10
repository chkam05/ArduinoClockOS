using ArduinoConnectWeb.Services.Authentication.Models;
using ArduinoConnectWeb.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using ArduinoConnectWeb.Tools;

namespace ArduinoConnectWeb.Controllers
{
    [ApiController]
    [Route("api/v1/tools")]
    public class ToolsController : ControllerBase
    {

        //  VARIABLES

        private readonly IConfiguration _configuration;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        public ToolsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion CLASS METHODS

        #region IMAGE TOOLS CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        [HttpGet("textToImage/{text}")]
        public async Task<IActionResult> GenerateImageWithText(string text)
        {
            if (!_configuration.GetValue<bool>("EnableTools"))
                return new BadRequestResult();

            return File(ImageTools.GenerateImage(text), "image/png");
        }

        #endregion IMAGE TOOLS CONTROLLER METHODS

        #region SECURITY TOOLS CONTROLLER METHODS

        //  --------------------------------------------------------------------------------
        [HttpGet("computeSha256Hash/{text}")]
        public async Task<IActionResult> ComputeSha256Hash(string text)
        {
            if (!_configuration.GetValue<bool>("EnableTools"))
                return new BadRequestResult();

            if (!string.IsNullOrEmpty(text))
            {
                string sha256hash = SecurityTools.ComputeSha256Hash(text);
                return new OkObjectResult(sha256hash);
            }

            return new BadRequestResult();
        }

        #endregion SECURITY TOOLS CONTROLLER METHODS

    }
}
