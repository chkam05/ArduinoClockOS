using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Auth.RequestModels;
using ArduinoConnectWeb.Models.Auth.ResponseModels;
using ArduinoConnectWeb.Models.Base.ResponseModels;

namespace ArduinoConnectWeb.Services.Auth
{
    public interface IAuthService
    {

        //  METHODS

        public Task<BaseResponseModel<SessionListResponseModel>> GetSessions(string? accessToken);
        public Task<BaseResponseModel<SessionDataModel>> Login(LoginRequestModel loginRequestModel);
        public Task<BaseResponseModel<string>> Logout(string? accessToken);
        public Task<BaseResponseModel<string>> LogoutAllSessions(string? accessToken);
        public Task<BaseResponseModel<SessionDataModel>> Refresh(RefreshRequestModel refreshRequestModel);

    }
}
