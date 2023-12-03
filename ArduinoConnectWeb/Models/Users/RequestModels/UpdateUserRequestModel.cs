﻿namespace ArduinoConnectWeb.Models.Users.RequestModels
{
    public class UpdateUserRequestModel
    {

        //  VARIABLES

        public string? NewUserName { get; set; }
        public string? NewPassword { get; set; }
        public string? NewPasswordRepeat { get; set; }
        public UserPermissionLevel? NewPermissionLevel { get; set; }

    }
}