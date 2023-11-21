using ArduinoConnectWeb.Models.Base.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace ArduinoConnectWeb.Utilities
{
    public static class ControllerUtilities
    {

        //  CONST

        private const string BEARER_TOKEN_HEADER = "Bearer ";


        //  METHODS

        #region HEADERS UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get authorization token from http context headers. </summary>
        /// <param name="httpContext"> Http context. </param>
        /// <returns> Authorization token or null. </returns>
        public static string? GetAuthorizationToken(HttpContext httpContext)
        {
            var authorizationHeaders = httpContext.Request.Headers["Authorization"];
            var bearerToken = authorizationHeaders.FirstOrDefault(h => h?.StartsWith(BEARER_TOKEN_HEADER) ?? false);

            if (!string.IsNullOrEmpty(bearerToken))
                return bearerToken.Replace(BEARER_TOKEN_HEADER, "");

            return null;
        }

        #endregion HEADERS UTILITY METHODS

        #region RESPONSE UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create http object response. </summary>
        /// <typeparam name="T"> Response data type. </typeparam>
        /// <param name="response"> Data processing response. </param>
        /// <returns> Action result. </returns>
        public static IActionResult CreateHttpObjectResponse<T>(BaseResponseModel<T> response) where T : class
        {
            if (response.IsSuccess)
            {
                return GetHttpObjectResponse(response.StatusCode, response.Content);
            }
            else
            {
                return GetHttpObjectResponse(response.StatusCode, new
                {
                    Message = response.GetErrorMessagesAsOne()
                });
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get http object response. </summary>
        /// <param name="statusCode"> Http status code. </param>
        /// <param name="content"> Content object. </param>
        /// <returns> Http object response. </returns>
        public static IActionResult GetHttpObjectResponse(int statusCode, object? content)
        {
            IActionResult? result = null;

            switch (statusCode)
            {
                //case StatusCodes.Status100Continue:
                //case StatusCodes.Status101SwitchingProtocols:
                //case StatusCodes.Status102Processing:
                //case StatusCodes.Status201Created:
                //case StatusCodes.Status202Accepted:
                //case StatusCodes.Status203NonAuthoritative:
                //case StatusCodes.Status204NoContent:
                //case StatusCodes.Status205ResetContent:
                //case StatusCodes.Status206PartialContent:
                //case StatusCodes.Status207MultiStatus:
                //case StatusCodes.Status208AlreadyReported:
                //case StatusCodes.Status226IMUsed:
                //case StatusCodes.Status300MultipleChoices:
                //case StatusCodes.Status301MovedPermanently:
                //case StatusCodes.Status302Found:
                //case StatusCodes.Status303SeeOther:
                //case StatusCodes.Status304NotModified:
                //case StatusCodes.Status305UseProxy:
                //case StatusCodes.Status306SwitchProxy:
                //case StatusCodes.Status307TemporaryRedirect:
                //case StatusCodes.Status308PermanentRedirect:
                //case StatusCodes.Status402PaymentRequired:
                //case StatusCodes.Status403Forbidden:
                //case StatusCodes.Status405MethodNotAllowed:
                //case StatusCodes.Status406NotAcceptable:
                //case StatusCodes.Status407ProxyAuthenticationRequired:
                //case StatusCodes.Status408RequestTimeout:
                //case StatusCodes.Status410Gone:
                //case StatusCodes.Status411LengthRequired:
                //case StatusCodes.Status412PreconditionFailed:
                //case StatusCodes.Status413PayloadTooLarge:
                //case StatusCodes.Status414RequestUriTooLong:
                //case StatusCodes.Status415UnsupportedMediaType:
                //case StatusCodes.Status416RangeNotSatisfiable:
                //case StatusCodes.Status417ExpectationFailed:
                //case StatusCodes.Status418ImATeapot:
                //case StatusCodes.Status419AuthenticationTimeout:
                //case StatusCodes.Status421MisdirectedRequest:
                //case StatusCodes.Status422UnprocessableEntity:
                //case StatusCodes.Status423Locked:
                //case StatusCodes.Status424FailedDependency:
                //case StatusCodes.Status426UpgradeRequired:
                //case StatusCodes.Status428PreconditionRequired:
                //case StatusCodes.Status429TooManyRequests:
                //case StatusCodes.Status431RequestHeaderFieldsTooLarge:
                //case StatusCodes.Status451UnavailableForLegalReasons:
                //case StatusCodes.Status500InternalServerError:
                //case StatusCodes.Status501NotImplemented:
                //case StatusCodes.Status502BadGateway:
                //case StatusCodes.Status503ServiceUnavailable:
                //case StatusCodes.Status504GatewayTimeout:
                //case StatusCodes.Status505HttpVersionNotsupported:
                //case StatusCodes.Status506VariantAlsoNegotiates:
                //case StatusCodes.Status507InsufficientStorage:
                //case StatusCodes.Status508LoopDetected:
                //case StatusCodes.Status510NotExtended:
                //case StatusCodes.Status511NetworkAuthenticationRequired:

                case StatusCodes.Status200OK:
                    result = new Microsoft.AspNetCore.Mvc.OkObjectResult(content);
                    break;

                case StatusCodes.Status400BadRequest:
                    result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(content);
                    break;

                case StatusCodes.Status401Unauthorized:
                    result = new Microsoft.AspNetCore.Mvc.UnauthorizedObjectResult(content);
                    break;

                case StatusCodes.Status404NotFound:
                    result = new Microsoft.AspNetCore.Mvc.NotFoundObjectResult(content);
                    break;

                case StatusCodes.Status409Conflict:
                    result = new Microsoft.AspNetCore.Mvc.ConflictObjectResult(content);
                    break;
            }

            if (result is null)
            {
                if (content is not null)
                {
                    result = new Microsoft.AspNetCore.Mvc.ObjectResult(content)
                    {
                        StatusCode = 500
                    };
                }
                else
                {
                    result = new Microsoft.AspNetCore.Mvc.StatusCodeResult(500);
                }
            }

            return result;
        }

        #endregion RESPONSE UTILITY METHODS

    }
}
