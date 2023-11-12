using ArduinoConnectWeb.Models.Base;
using Newtonsoft.Json;

namespace ArduinoConnectWeb.Models.Users
{
    public class UserDataModel : BaseUniqueDataModel, ICloneable
    {

        //  VARIABLES

        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public UserPermissionLevel PermissionLevel { get; set; } = UserPermissionLevel.Guest;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UserDataModel class constructor. </summary>
        /// <param name="id"> Identifier. </param>
        /// <param name="userName"> User name. </param>
        /// <param name="passwordHash"> Password hash. </param>
        /// <param name="createdAt"> Created date time in UTC. </param>
        /// <param name="lastModifiedAt"> Last modified date time in UTC. </param>
        [JsonConstructor]
        public UserDataModel(string? id, string userName, string passwordHash,
            UserPermissionLevel? permissionLevel = null,
            DateTime? createdAt = null, DateTime? lastModifiedAt = null) : base(id)
        {
            UserName = userName;
            PasswordHash = passwordHash;
            PermissionLevel = permissionLevel.HasValue ? permissionLevel.Value : UserPermissionLevel.Guest;
            CreatedAt = createdAt.HasValue ? createdAt.Value : GetCreatedDateTimeInUtc();
            LastModifiedAt = lastModifiedAt.HasValue ? lastModifiedAt.Value : GetCreatedDateTimeInUtc();
        }

        #endregion CLASS METHODS

        #region CLONE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates a new object that is a copy of the current instance. </summary>
        /// <returns> A new object that is a copy of this instance. </returns>
        public object Clone()
        {
            var serializedObject = JsonConvert.SerializeObject(this);
            var clonedObject = JsonConvert.DeserializeObject<UserDataModel>(serializedObject);

            return clonedObject;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Creates a new object that is a copy of the current instance. </summary>
        /// <returns> A new object that is a copy of this instance. </returns>
        public UserDataModel? CloneWithType()
        {
            return (UserDataModel) Clone();
        }

        #endregion CLONE METHODS

        #region OVERWRITTEN OPERATOR METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Determines whether the specified object is equal to the current object. </summary>
        /// <param name="obj"> Object to compare. </param>
        /// <returns> True - if the specified object is equal to the current object; False - otherwise. </returns>
        public override bool Equals(object? obj)
        {
            if (obj is UserDataModel userDataModel)
            {
                return base.Equals(obj) && UserName == userDataModel.UserName;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Serves as the default hash function. </summary>
        /// <returns> A hash code for the current object. </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ UserName.GetHashCode();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if the objects are equal. </summary>
        /// <param name="left"> Left side object. </param>
        /// <param name="right"> Right side object. </param>
        /// <returns> True - objects are equal; False - otherwise. </returns>
        public static bool operator ==(UserDataModel left, UserDataModel right)
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
        public static bool operator !=(UserDataModel left, UserDataModel right)
        {
            return !(left == right);
        }

        #endregion OVERWRITTEN OPERATOR METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get created date time in UTC. </summary>
        /// <returns> Created date time in UTC. </returns>
        private DateTime GetCreatedDateTimeInUtc()
        {
            return DateTime.UtcNow;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update last modified at date time. </summary>
        public void UpdateLastModifiedAt()
        {
            LastModifiedAt = GetCreatedDateTimeInUtc();
        }

        #endregion UTILITY METHODS

    }
}
