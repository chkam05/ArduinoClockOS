using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base;

namespace ArduinoConnectWeb.Services.Auth
{
    public interface IAuthService
    {

        //  METHODS

        public Task<ResponseBaseModel<ResponseSessionsListModel>> GetSessions(string? accessToken);
        public Task<ResponseBaseModel<SessionDataModel>> Login(RequestLoginModel requestLoginModel);
        public Task<ResponseBaseModel<string>> Logout(RequestLogoutModel requestLogoutModel);
        public Task<ResponseBaseModel<SessionDataModel>> Refresh(RequestRefreshModel requestRefreshModel);

    }
}
