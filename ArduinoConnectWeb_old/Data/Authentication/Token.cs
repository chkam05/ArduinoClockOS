namespace ArduinoConnectWeb.Data.Authentication
{
    public class Token
    {

        //  VARIABLES

        protected string _token;
        protected DateTime _validateFromUTC;
        protected DateTime _validateToUTC;
        protected string _userId;


        //  GETTERS & SETTERS

        public string TokenContent
        {
            get => _token;
        }

        public DateTime ValidateFromUTC
        {
            get => _validateFromUTC;
        }

        public DateTime ValidateToUTC
        {
            get => _validateToUTC;
        }

        public string UserId
        {
            get => _userId;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        public Token(string token, string userId, TimeSpan tokenValidityTime)
        {
            _token = token;
            _validateFromUTC = DateTime.UtcNow;
            _validateToUTC = DateTime.UtcNow + tokenValidityTime;
            _userId = userId;
        }

        #endregion CLASS METHODS

        #region CHECK METHODS

        //  --------------------------------------------------------------------------------
        public bool IsExpired()
        {
            return DateTime.UtcNow >= ValidateToUTC;
        }

        #endregion CHECK METHODS

    }
}
