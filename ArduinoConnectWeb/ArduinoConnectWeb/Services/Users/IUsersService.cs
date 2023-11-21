using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Users;
using ArduinoConnectWeb.Models.Users.RequestModels;
using ArduinoConnectWeb.Models.Users.ResponseModels;

namespace ArduinoConnectWeb.Services.Users
{
    public interface IUsersService
    {

        //  METHODS

        public Task<BaseResponseModel<UserResponseModel>> CreateUser(string? accessToken, CreateUserRequestModel requestUserCreateModel);
        public Task<BaseResponseModel<UserResponseModel>> GetUserById(string? accessToken, string? id);
        public Task<BaseResponseModel<UserResponseModel>> GetUserById(string? id);
        public Task<BaseResponseModel<UserResponseModel>> GetUserByUserName(string? accessToken, string? userName);
        public Task<BaseResponseModel<UserListResponseModel>> GetUsersList(string? accessToken);
        public Task<BaseResponseModel<string>> RemoveUser(string? accessToken, string? id);
        public Task<BaseResponseModel<UserResponseModel>> UpdateUser(string? accessToken, string? id, UpdateUserRequestModel requestUserUpdateModel);
        public Task<BaseResponseModel<UserDataModel>> ValidateUser(string? userName, string? password);

    }
}
