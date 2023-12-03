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
        public Task<BaseResponseModel<SessionListResponseModel>> GetSessionsAsync(SessionDataModel session);
        public Task<BaseResponseModel<SessionDataModel>> LoginAsync(LoginRequestModel loginRequestModel);
        public Task<BaseResponseModel<string>> LogoutAsync(SessionDataModel session);
        public Task<BaseResponseModel<string>> LogoutAllSessionsAsync(SessionDataModel session);

        public Task<BaseResponseModel<T>> ProcessAsyncWithAuthorization<T>(string? accessToken,
            Func<SessionDataModel, BaseResponseModel<T>> func) where T : class;

        public Task<BaseResponseModel<T>> ProcessTaskAsyncWithAuthorization<T>(string? accessToken,
            Func<SessionDataModel, Task<BaseResponseModel<T>>> func) where T : class;

    }
}
