using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Network.ResponseModels;

namespace ArduinoConnectWeb.Services.NetworkService
{
    public interface INetworkService
    {

        //  METHODS

        public Task<BaseResponseModel<HostInfoResponseModel>> GetHostInfoAsync(SessionDataModel session);
        public Task<BaseResponseModel<ExternalHostInfoResponseModel>> GetExternalHostInfoAsync(SessionDataModel session);

    }
}
