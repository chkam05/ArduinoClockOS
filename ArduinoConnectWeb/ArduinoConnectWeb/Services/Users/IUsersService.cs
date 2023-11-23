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

        public Task<BaseResponseModel<UserResponseModel>> CreateUserAsync(string? accessToken, IAuthService authService, CreateUserRequestModel requestUserCreateModel);
        public Task<BaseResponseModel<UserResponseModel>> GetUserByIdAsync(string? accessToken, IAuthService authService, string? id);
        public Task<BaseResponseModel<UserDataModel>> GetUserByIdAsync(string? id);
        public Task<BaseResponseModel<UserResponseModel>> GetUserByUserNameAsync(string? accessToken, IAuthService authService, string? userName);
        public Task<BaseResponseModel<UserListResponseModel>> GetUsersListAsync(string? accessToken, IAuthService authService);
        public Task<BaseResponseModel<string>> RemoveUserAsync(string? accessToken, IAuthService authService, string? id);
        public Task<BaseResponseModel<UserResponseModel>> UpdateUserAsync(string? accessToken, IAuthService authService, string? id, UpdateUserRequestModel requestUserUpdateModel);
        public Task<BaseResponseModel<UserDataModel>> ValidateUserAsync(string? userName, string? password);

    }
}
