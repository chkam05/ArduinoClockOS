using ArduinoConnectWeb.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Versioning;

namespace ArduinoConnectWeb.Controllers
{
    [ApiController]
    [Route("api/v1/Utilities")]
    public class UtilitiesController : ControllerBase
    {

        //  VARIABLES

        private readonly IConfiguration _configuration;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UtilitiesController class constructor. </summary>
        /// <param name="configuration"> Interface of application configuration properties. </param>
        public UtilitiesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion CLASS METHODS

        #region CONTROLLER GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Generate image from string. </summary>
        /// <param name="text"> String text. </param>
        /// <param name="foregroundColor"> Foreground hexadecimal color code. </param>
        /// <param name="backgroundColor"> Background hexadecimal color code. </param>
        /// <param name="fontFamily"> Font family name. </param>
        /// <param name="fontSize"> Font size. </param>
        /// <returns> Image or BadRequestObjectResult. </returns>
        [SupportedOSPlatform("windows")]
        [HttpGet("ImageFromText")]
        public async Task<IActionResult> GenerateImageFromText(
            [FromQuery] string text,
            [FromQuery] string? foregroundColor,
            [FromQuery] string? backgroundColor,
            [FromQuery] string? fontFamily,
            [FromQuery] int? fontSize)
        {
            if (string.IsNullOrEmpty(text))
                return new BadRequestObjectResult(new { Message = "Text cannot be null or empty." });

            var result = await Task.Run(
                () => ImageUtilities.GenerateImageFromString(
                    text, foregroundColor, backgroundColor, fontFamily, fontSize));

            return File(result, "image/png");
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate SHA256 hash from string. </summary>
        /// <param name="text"> Raw string data. </param>
        /// <returns> SHA256 hash from string or BadRequestObjectResult. </returns>
        [HttpGet("ComputeSha256Hash")]
        public async Task<IActionResult> ComputeSha256Hash([FromQuery]  string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                string sha256hash = await Task.Run(() => SecurityUtilities.ComputeSha256Hash(text));
                return new OkObjectResult(sha256hash);
            }

            return new BadRequestResult();
        }

        #endregion CONTROLLER GET METHODS

    }
}
