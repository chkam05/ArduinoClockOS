using ArduinoConnectWeb.Models.Base;
using Newtonsoft.Json;

namespace ArduinoConnectWeb.Models.Auth
{
    public class SessionDataModel : BaseUniqueDataModel, ICloneable
    {

        //  VARIABLES

        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenValidityTime { get; set; }
        public DateTime RefreshTokenValidityTime { get; set; }
        public DateTime SessionStart { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SessionDataModel class constructor. </summary>
        /// <param name="id"> Session identifier. </param>
        /// <param name="userId"> User identifier. </param>
        /// <param name="accessToken"> Access token. </param>
        /// <param name="refreshToken"> Refresh token. </param>
        /// <param name="accessTokenValidityTime"> Access token validity time. </param>
        /// <param name="refreshTokenValidityTime"> Refresh token validity time. </param>
        public SessionDataModel(string? id, string userId, string accessToken, string refreshToken,
            DateTime accessTokenValidityTime, DateTime refreshTokenValidityTime) : base(id)
        {
            UserId = userId;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            AccessTokenValidityTime = accessTokenValidityTime;
            RefreshTokenValidityTime = refreshTokenValidityTime;
            SessionStart = DateTime.UtcNow;
        }

        #endregion CLASS METHODS

        #region CLONE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates a new object that is a copy of the current instance. </summary>
        /// <returns> A new object that is a copy of this instance. </returns>
        public object Clone()
        {
            var serializedObject = JsonConvert.SerializeObject(this);
            var clonedObject = JsonConvert.DeserializeObject<SessionDataModel>(serializedObject);

            return clonedObject;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Creates a new object that is a copy of the current instance. </summary>
        /// <returns> A new object that is a copy of this instance. </returns>
        public SessionDataModel? CloneWithType()
        {
            return (SessionDataModel)Clone();
        }

        #endregion CLONE METHODS

        #region OVERWRITTEN OPERATOR METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Determines whether the specified object is equal to the current object. </summary>
        /// <param name="obj"> Object to compare. </param>
        /// <returns> True - if the specified object is equal to the current object; False - otherwise. </returns>
        public override bool Equals(object? obj)
        {
            if (obj is SessionDataModel sessionDataModel)
            {
                return base.Equals(obj) && UserId == sessionDataModel.UserId;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Serves as the default hash function. </summary>
        /// <returns> A hash code for the current object. </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ UserId.GetHashCode();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if the objects are equal. </summary>
        /// <param name="left"> Left side object. </param>
        /// <param name="right"> Right side object. </param>
        /// <returns> True - objects are equal; False - otherwise. </returns>
        public static bool operator ==(SessionDataModel left, SessionDataModel right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if the objects are not equal. </summary>
        /// <param name="left"> Left side object. </param>
        /// <param name="right"> Right side object. </param>
        /// <returns> True - objects are not equal; False - otherwise. </returns>
        public static bool operator !=(SessionDataModel left, SessionDataModel right)
        {
            return !(left == right);
        }

        #endregion OVERWRITTEN OPERATOR METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get session time. </summary>
        /// <returns> Session time time span. </returns>
        public TimeSpan GetSessionTime()
        {
            return DateTime.UtcNow - SessionStart;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if access token is valid. </summary>
        /// <returns> True - access token is valid; False - otherwise. </returns>
        public bool IsAccessTokenValid()
        {
            return AccessTokenValidityTime > DateTime.UtcNow;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if refresh token is valid. </summary>
        /// <returns> True - refresh token is valid; False - otherwise. </returns>
        public bool IsRefreshTokenValid()
        {
            return RefreshTokenValidityTime > DateTime.UtcNow;
        }

        #endregion UTILITY METHODS

    }
}
