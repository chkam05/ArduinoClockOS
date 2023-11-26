using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Serial.RequestModels;
using ArduinoConnectWeb.Models.Serial.ResponseModels;

namespace ArduinoConnectWeb.Services.Serial
{
    public interface ISerialPortService
    {

        //  METHODS

        public Task<BaseResponseModel<PortListResponseModel>> GetAllPortsAsync(SessionDataModel session);
        public Task<BaseResponseModel<PortListResponseModel>> GetAvailablePortsAsync(SessionDataModel session);
        public Task<BaseResponseModel<ConnectionResponseModel>> OpenConnectionAsync(SessionDataModel session, OpenConnectionRequestModel requestModel);
        public Task<BaseResponseModel<string>> CloseConnectionAsync(SessionDataModel session, string? id);
        public Task<BaseResponseModel<ConnectionResponseModel>> GetConnectionByIdAsync(SessionDataModel session, string? id);
        public Task<BaseResponseModel<ConnectionListResponseModel>> GetAllConnectionsAsync(SessionDataModel session);
        public Task<BaseResponseModel<ConnectionListResponseModel>> GetOwnConnectionsAsync(SessionDataModel session);

        public Task<BaseResponseModel<SendAndReceiveMessageResponseModel>> SendAndReceiveMessageAsync(
            SessionDataModel session, string? id, SendAndReceiveMessageRequestModel sendAndReceiveMessageRequestModel);

        public Task<BaseResponseModel<string>> SendMessageAsync(
            SessionDataModel session, string? id, SendMessageRequestModel sendMessageRequestModel);

        public Task<BaseResponseModel<MessageResponseModel>> GetMessageAsync(SessionDataModel session, string? id);
        public Task<BaseResponseModel<MessageResponseModel>> GetLastMessageAsync(SessionDataModel session, string? id);

    }
}
