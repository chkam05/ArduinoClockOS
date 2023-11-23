using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Auth.RequestModels;
using ArduinoConnectWeb.Models.Auth.ResponseModels;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Services.Base;

namespace ArduinoConnectWeb.Services.Auth
{
    public interface IAuthService : IService
    {

        //  METHODS

        public Task<BaseResponseModel<SessionDataModel>> AuthorizeAsync(string? accessToken);
        public Task<BaseResponseModel<SessionDataModel>> RefreshAsync(RefreshRequestModel refreshRequestModel);
        public Task<BaseResponseModel<SessionListResponseModel>> GetSessionsAsync(string? accessToken);
        public Task<BaseResponseModel<SessionDataModel>> LoginAsync(LoginRequestModel loginRequestModel);
        public Task<BaseResponseModel<string>> LogoutAsync(string? accessToken);
        public Task<BaseResponseModel<string>> LogoutAllSessionsAsync(string? accessToken);

    }
}
