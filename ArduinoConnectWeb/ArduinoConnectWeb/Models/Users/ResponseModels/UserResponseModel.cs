namespace ArduinoConnectWeb.Models.Users.ResponseModels
{
    public class UserResponseModel
    {

        //  VARIABLES

        public string Id { get; set; }
        public string UserName { get; set; }
        public UserPermissionLevel PermissionLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UserResponseModel class constructor. </summary>
        /// <param name="id"> User identifiers. </param>
        /// <param name="userName"> User name. </param>
        /// <param name="permissionLevel"> Persmission level. </param>
        /// <param name="createdAt"> User creation date time. </param>
        /// <param name="lastModifiedAt"> User modification date time. </param>
        public UserResponseModel(string id, string userName, UserPermissionLevel permissionLevel,
            DateTime createdAt, DateTime lastModifiedAt)
        {
            Id = id;
            UserName = userName;
            PermissionLevel = permissionLevel;
            CreatedAt = createdAt;
            LastModifiedAt = lastModifiedAt;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> UserResponseModel class constructor. </summary>
        /// <param name="user"> User data model. </param>
        public UserResponseModel(UserDataModel user)
        {
            Id = user.Id;
            UserName = user.UserName;
            PermissionLevel = user.PermissionLevel;
            CreatedAt = user.CreatedAt;
            LastModifiedAt = user.LastModifiedAt;
        }

        #endregion CLASS METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert to UserDataModel. </summary>
        /// <returns> User data model. </returns>
        public UserDataModel ConvertToUserDataModel()
        {
            return new UserDataModel(Id, UserName, null, PermissionLevel, CreatedAt, LastModifiedAt);
        }

        #endregion UTILITY METHODS

    }
}
