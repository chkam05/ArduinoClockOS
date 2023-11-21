namespace ArduinoConnectWeb.Models.Exceptions
{
    public class ProcessingException : Exception
    {

        //  VARIABLES

        public int StatusCode { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ProcessingException class constructor. </summary>
        /// <param name="message"> Error message. </param>
        /// <param name="statuCode"> Error code. </param>
        public ProcessingException(string message, int statuCode) : base(message)
        {
            StatusCode = statuCode;
        }

        #endregion CLASS METHODS

    }
}
