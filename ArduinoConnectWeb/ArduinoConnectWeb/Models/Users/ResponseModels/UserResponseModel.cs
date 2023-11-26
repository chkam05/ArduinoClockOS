namespace ArduinoConnectWeb.Models.Users.ResponseModels
{
    public class UserResponseModel
    {

        //  VARIABLES

        public string? Id { get; set; }
        public string? UserName { get; set; }
        public UserPermissionLevel PermissionLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UserResponseModel class constructor. </summary>
        public UserResponseModel()
        {
            //
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

    }
}
