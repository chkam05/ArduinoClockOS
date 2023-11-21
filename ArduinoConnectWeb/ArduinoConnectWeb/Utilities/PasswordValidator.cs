namespace ArduinoConnectWeb.Utilities
{
    public class PasswordValidator : IDisposable
    {

        //  VARIABLES

        public int MinLength { get; set; }
        public bool RequireNumericChar { get; set; }
        public bool RequireLowercaseChar { get; set; }
        public bool RequireSpecialChar { get; set; }
        public bool RequireUppercaseChar { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PasswordValidator class constructor. </summary>
        /// <param name="minLength"> Min password length. </param>
        /// <param name="requireNumericChar"> Require numeric character. </param>
        /// <param name="requireLowercaseChar"> Require lowercase character. </param>
        /// <param name="requireSpecialChar"> Require special character. </param>
        /// <param name="requireUppercaseChar"> Require uppercase character. </param>
        public PasswordValidator(int minLength = 8, bool requireNumericChar = false,
            bool requireLowercaseChar = false, bool requireSpecialChar = false, bool requireUppercaseChar = false)
        {
            MinLength = minLength;
            RequireNumericChar = requireNumericChar;
            RequireLowercaseChar = requireLowercaseChar;
            RequireSpecialChar = requireSpecialChar;
            RequireUppercaseChar = requireUppercaseChar;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get PasswordValidator with weak configuration. </summary>
        /// <returns> PasswordValidator class instance. </returns>
        public static PasswordValidator GetWeakConfiguration()
        {
            return new PasswordValidator(8, false, false, false, false);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get PasswordValidator with medium configuration. </summary>
        /// <returns> PasswordValidator class instance. </returns>
        public static PasswordValidator GetMediumConfiguration()
        {
            return new PasswordValidator(8, true, true, false, true);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get PasswordValidator with strong configuration. </summary>
        /// <returns> PasswordValidator class instance. </returns>
        public static PasswordValidator GetStrongConfiguration()
        {
            return new PasswordValidator(8, true, true, true, true);
        }

        #endregion CLASS METHODS

        #region VALIDATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Validate password. </summary>
        /// <param name="password"> Password to validate. </param>
        /// <param name="reason"> Failure reason. </param>
        /// <returns> True - password is valid; False - otherwise. </returns>
        public bool ValidatePassword(string? password, out string? reason)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                reason = "Password is empty";
                return false;
            }

            if (password.Length < MinLength)
            {
                reason = $"Password length is less than {MinLength} characters";
            }

            bool hasNumericChar = !RequireNumericChar;
            bool hasLowercaseChar = !RequireLowercaseChar;
            bool hasSpecialChar = !RequireSpecialChar;
            bool hasUppercasChar = !RequireUppercaseChar;

            foreach (char c in password)
            {
                if (char.IsNumber(c))
                    hasNumericChar = true;

                if (char.IsLower(c))
                    hasLowercaseChar = true;

                if (char.IsUpper(c))
                    hasUppercasChar = true;

                if (!char.IsLetterOrDigit(c))
                    hasSpecialChar = true;
            }

            if (!hasNumericChar)
            {
                reason = "Password does not contain numeric character";
                return false;
            }

            if (!hasLowercaseChar)
            {
                reason = "Password does not contain lowercase character";
                return false;
            }

            if (!hasSpecialChar)
            {
                reason = "Password does not contain special character";
                return false;
            }

            if (!hasUppercasChar)
            {
                reason = "Password does not contain uppercase character";
                return false;
            }

            reason = null;
            return true;
        }

        #endregion VALIDATION METHODS

    }
}
