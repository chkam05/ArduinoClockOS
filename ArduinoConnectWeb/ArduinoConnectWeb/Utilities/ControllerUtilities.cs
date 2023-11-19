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

    }
}
