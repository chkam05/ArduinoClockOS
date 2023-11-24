using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Users;
using ArduinoConnectWeb.Models.Users.RequestModels;
using ArduinoConnectWeb.Models.Users.ResponseModels;
using ArduinoConnectWeb.Services.Auth;
using ArduinoConnectWeb.Services.Base;

namespace ArduinoConnectWeb.Services.Users
{
    public interface IUsersService : IService
    {

        //  METHODS

        public Task<BaseResponseModel<UserResponseModel>> CreateUserAsync(CreateUserRequestModel requestUserCreateModel);
        public Task<BaseResponseModel<UserResponseModel>> GetUserByIdAsync(string? id);
        public Task<BaseResponseModel<UserDataModel>> GetFullUserByIdAsync(string? id);
        public Task<BaseResponseModel<UserResponseModel>> GetUserByUserNameAsync(string? userName);
        public Task<BaseResponseModel<UserListResponseModel>> GetUsersListAsync();
        public Task<BaseResponseModel<string>> RemoveUserAsync(string? id);
        public Task<BaseResponseModel<UserResponseModel>> UpdateUserAsync(string? id, UpdateUserRequestModel requestUserUpdateModel);
        public Task<BaseResponseModel<UserDataModel>> ValidateUserAsync(string? userName, string? password);

    }
}
