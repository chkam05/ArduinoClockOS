using Microsoft.AspNetCore.Mvc;

namespace ArduinoConnectWeb.Models.Base.ResponseModels
{
    public class BaseResponseModel<T> where T : class
    {

        //  VARIABLES

        public T? Content { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public int StatusCode { get; set; } = StatusCodes.Status200OK;


        //  GETTERS & SETTERS

        public bool HasErrors
        {
            get => ErrorMessages != null && ErrorMessages.Any();
        }

        public bool IsSuccess
        {
            get => !HasErrors && Content != null;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ResponseBaseModel class constructor. </summary>
        public BaseResponseModel()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ResponseBaseModel class constructor. </summary>
        /// <param name="content"> Response data content. </param>
        /// <param name="statusCode"> Status code. </param>
        public BaseResponseModel(T? content, int statusCode = StatusCodes.Status200OK)
        {
            Content = content;
            StatusCode = statusCode;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ResponseBaseModel class constructor. </summary>
        /// <param name="errorMessages"> Enumerable error messages. </param>
        /// <param name="statusCode"> Status code. </param>
        public BaseResponseModel(IEnumerable<string> errorMessages, int statusCode = StatusCodes.Status400BadRequest)
        {
            if (errorMessages?.Any() ?? false)
                ErrorMessages = errorMessages.ToList();

            StatusCode = statusCode;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ResponseBaseModel class constructor. </summary>
        /// <param name="errorMessage"> Error message. </param>
        /// <param name="statusCode"> Status code. </param>
        public BaseResponseModel(string errorMessage, int statusCode = StatusCodes.Status400BadRequest)
        {
            if (ErrorMessages == null)
                ErrorMessages = new List<string>();

            ErrorMessages.Add(errorMessage);
            StatusCode = statusCode;
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
                return string.Join(joinString ?? "; ", ErrorMessages);

            return null;
        }

        #endregion ERROR METHODS

    }
}
