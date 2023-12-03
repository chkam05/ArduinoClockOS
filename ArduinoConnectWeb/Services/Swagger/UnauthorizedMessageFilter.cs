using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ArduinoConnectWeb.Services.Swagger
{
    public class UnauthorizedMessageFilter : IDocumentFilter
    {

        //  CONST

        private const string UNAUTHORIZED_MESSAGE = "Error: response status is 401.";


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Apply swagger document filter. </summary>
        /// <param name="swaggerDoc"> Swagger document. </param>
        /// <param name="context"> Document filter context. </param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Znajdź odpowiedź Unauthorized
            var unauthorizedResponse = swaggerDoc.Paths
                .SelectMany(path => path.Value.Operations)
                .SelectMany(operation => operation.Value.Responses)
                .Where(response => response.Key == "401")
                .Select(response => response.Value)
                .FirstOrDefault();

            if (unauthorizedResponse != null)
            {
                unauthorizedResponse.Description = UNAUTHORIZED_MESSAGE;
            }
        }

    }
}
