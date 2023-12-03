namespace ArduinoConnectWeb.Models.Exceptions
{
    public class UnauthorizedException : ProcessingException
    {

        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ProcessingException class constructor. </summary>
        /// <param name="message"> Error message. </param>
        public UnauthorizedException(string message) : base(message, StatusCodes.Status401Unauthorized)
        {
            //
        }

        #endregion CLASS METHODS

    }
}
