using ArduinoConnectWeb.Enums;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace ArduinoConnectWeb.Utilities
{
    public static class SecurityUtilities
    {

        //  METHODS

        #region ENCRYPTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate SHA256 hash from string data. </summary>
        /// <param name="rawData"> String data. </param>
        /// <returns> SHA256 hash as string. </returns>
        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                //  ComputeHash - returns byte array.
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                //  Convert byte array to a string.
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Generate symmetric security key. </summary>
        /// <param name="keySize"> Key size. </param>
        /// <returns> Symetric security key. </returns>
        public static string GenerateSymmetricSecurityKey()
        {
            using (var hmac = new HMACSHA256())
            {
                var keyBytes = hmac.Key;
                var key = Convert.ToBase64String(keyBytes);
                return key;
            }
        }

        #endregion ENCRYPTION METHODS

    }
}
