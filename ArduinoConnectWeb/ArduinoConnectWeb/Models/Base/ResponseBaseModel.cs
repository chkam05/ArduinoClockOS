namespace ArduinoConnectWeb.Models.Base
{
    public class ResponseBaseModel<T> where T : class
    {

        //  VARIABLES

        public T? ResponseData { get; set; }
        public List<string>? ErrorMessages { get; set; }


        //  GETTERS & SETTERS

        public bool HasErrors
        {
            get => ErrorMessages != null && ErrorMessages.Any();
        }

        public bool IsSuccess
        {
            get => !HasErrors && ResponseData != null;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ResponseBaseModel class constructor. </summary>
        public ResponseBaseModel()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ResponseBaseModel class constructor. </summary>
        /// <param name="responseData"> Response data. </param>
        public ResponseBaseModel(T? responseData)
        {
            ResponseData = responseData;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ResponseBaseModel class constructor. </summary>
        /// <param name="errorMessages"> Enumerable error messages. </param>
        public ResponseBaseModel(IEnumerable<string> errorMessages)
        {
            if (errorMessages?.Any() ?? false)
                ErrorMessages = errorMessages.ToList();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ResponseBaseModel class constructor. </summary>
        /// <param name="errorMessage"> Error message. </param>
        public ResponseBaseModel(string errorMessage)
        {
            if (ErrorMessages == null)
                ErrorMessages = new List<string>();

            ErrorMessages.Add(errorMessage);
        }

        #endregion CLASS METHODS

        #region ERROR METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get error messages as one message. </summary>
        /// <param name="joinString"> Messages join string. </param>
        /// <returns> Error messages as one message or null. </returns>
        public string? GetErrorMessagesAsOne(string? joinString = null)
        {
            if (ErrorMessages?.Any() ?? false)
                return string.Join((joinString ?? "; "), ErrorMessages);

            return null;
        }

        #endregion ERROR METHODS

    }
}
