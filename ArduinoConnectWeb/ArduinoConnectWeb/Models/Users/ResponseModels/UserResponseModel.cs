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
        public UserResponseModel(string id, string userName, UserPermissionLevel permissionLevel,
            DateTime createdAt, DateTime lastModifiedAt)
        {
            Id = id;
            UserName = userName;
            PermissionLevel = permissionLevel;
            CreatedAt = createdAt;
            LastModifiedAt = lastModifiedAt;
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
