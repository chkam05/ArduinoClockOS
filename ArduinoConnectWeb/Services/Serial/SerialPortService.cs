using ArduinoConnectWeb.DataContexts;
using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Exceptions;
using ArduinoConnectWeb.Models.Serial;
using ArduinoConnectWeb.Models.Serial.RequestModels;
using ArduinoConnectWeb.Models.Serial.ResponseModels;
using ArduinoConnectWeb.Services.Base;
using Microsoft.AspNetCore.Connections;

namespace ArduinoConnectWeb.Services.Serial
{
    public class SerialPortService : DataProcessor, ISerialPortService
    {

        //  VARIABLES

        private readonly SerialPortServiceConfig _config;
        private readonly SerialPortDataContext _serialPortDataContext;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SerialPortService class constructor. </summary>
        /// <param name="config"> SerialPort service config. </param>
        /// <param name="logger"> Application logger. </param>
        public SerialPortService(SerialPortServiceConfig config, ILogger<SerialPortService> logger) : base(logger)
        {
            _config = config;

            _serialPortDataContext = new SerialPortDataContext();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get all ports async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<PortListResponseModel>> GetAllPortsAsync(SessionDataModel session)
        {
            return await ProcessAsync(() =>
            {
                var ports = _serialPortDataContext.GetAvailableSerialPorts();

                return new BaseResponseModel<PortListResponseModel>(new PortListResponseModel()
                {
                    Ports = ports
                });
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get available ports async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<PortListResponseModel>> GetAvailablePortsAsync(SessionDataModel session)
        {
            return await ProcessAsync(() =>
            {
                var ports = _serialPortDataContext.GetAvailableSerialPorts();

                var usedPorts = _serialPortDataContext.Connections
                    .Select(c => c.Port)
                    .Distinct()
                    .ToList();

                return new BaseResponseModel<PortListResponseModel>(new PortListResponseModel()
                {
                    Ports = ports.Where(p => !usedPorts.Contains(p)).ToList()
                });
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Open serial port connection async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <param name="requestModel"> Open connection request model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<ConnectionResponseModel>> OpenConnectionAsync(SessionDataModel session, OpenConnectionRequestModel requestModel)
        {
            return await ProcessAsync(() =>
            {
                if (!BaudRates.IsCorrectBaudRate(requestModel.BaudRate))
                    throw new ProcessingException($"Invalid {nameof(requestModel.BaudRate)} value", StatusCodes.Status400BadRequest);

                var ports = _serialPortDataContext.GetAvailableSerialPorts();

                if (string.IsNullOrEmpty(requestModel.Port) || !ports.Any(p => p == requestModel.Port.ToUpper()))
                    throw new ProcessingException($"Invalid {nameof(requestModel.Port)} value", StatusCodes.Status400BadRequest);

                var connections = _serialPortDataContext.Connections.Where(c => c.Port == requestModel.Port.ToUpper());

                if (connections.Any(c => c.IsConnected))
                    throw new ProcessingException($"Port {requestModel.Port} already in use", StatusCodes.Status400BadRequest);

                if (connections.Any())
                    throw new ProcessingException($"There is an incorrectly terminated connection with {requestModel.Port} port", StatusCodes.Status400BadRequest);

                var connectionHandler = new SerialPortHandler(requestModel.Port, requestModel.BaudRate, session.UserId);

                connectionHandler.Connect();
                Logger.LogInformation($"Connection open on port {connectionHandler.Port} at {connectionHandler.BaudRate}bps");

                _serialPortDataContext.AddConnection(connectionHandler);

                return new BaseResponseModel<ConnectionResponseModel>(new ConnectionResponseModel(connectionHandler));
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Close serial port connection async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <param name="id"> Connection identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<string>> CloseConnectionAsync(SessionDataModel session, string? id)
        {
            return await ProcessAsync(() =>
            {
                if (string.IsNullOrEmpty(id))
                    throw new ProcessingException("Invalid connection identifier", StatusCodes.Status400BadRequest);

                var connectionHandler = _serialPortDataContext.Connections
                    .FirstOrDefault(c => c.Id == id.ToUpper());

                if (CloseConnection(connectionHandler))
                    return new BaseResponseModel<string>(content: $"Connection closed on port {connectionHandler.Port}");
                else
                    throw new ProcessingException($"Could not close connection on port {connectionHandler.Port}", StatusCodes.Status400BadRequest);
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get serial port connection by id async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <param name="id"> Connection identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<ConnectionResponseModel>> GetConnectionByIdAsync(SessionDataModel session, string? id)
        {
            return await ProcessAsync(() =>
            {
                if (string.IsNullOrEmpty(id))
                    throw new ProcessingException("Invalid connection identifier", StatusCodes.Status400BadRequest);

                var connectionHandler = _serialPortDataContext.Connections
                    .FirstOrDefault(c => c.Id == id.ToUpper());

                if (connectionHandler is null)
                    throw new ProcessingException($"Connection not found", StatusCodes.Status400BadRequest);

                return new BaseResponseModel<ConnectionResponseModel>(new ConnectionResponseModel(connectionHandler));
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get all serial port connections async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<ConnectionListResponseModel>> GetAllConnectionsAsync(SessionDataModel session)
        {
            return await ProcessAsync(() =>
            {
                var responseModels = _serialPortDataContext.Connections
                    .Select(c => new ConnectionResponseModel(c))
                    .ToList();

                return new BaseResponseModel<ConnectionListResponseModel>(new ConnectionListResponseModel()
                {
                    Connections = responseModels
                });
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get all own serial port connections async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<ConnectionListResponseModel>> GetOwnConnectionsAsync(SessionDataModel session)
        {
            return await ProcessAsync(() =>
            {
                var responseModels = _serialPortDataContext.Connections
                    .Where(c => c.UserId == session.UserId)
                    .Select(c => new ConnectionResponseModel(c))
                    .ToList();

                return new BaseResponseModel<ConnectionListResponseModel>(new ConnectionListResponseModel()
                {
                    Connections = responseModels
                });
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Send and receive message async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <param name="id"> Connection identifier. </param>
        /// <param name="sendAndReceiveMessageRequestModel"> Send and receive </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<SendAndReceiveMessageResponseModel>> SendAndReceiveMessageAsync(SessionDataModel session, string? id,
            SendAndReceiveMessageRequestModel sendAndReceiveMessageRequestModel)
        {
            return await ProcessAsync(() =>
            {
                if (string.IsNullOrEmpty(id))
                    throw new ProcessingException("Invalid connection identifier", StatusCodes.Status400BadRequest);

                if (string.IsNullOrEmpty(sendAndReceiveMessageRequestModel?.Message))
                    throw new ProcessingException("Invalid message", StatusCodes.Status400BadRequest);

                var connectionHandler = _serialPortDataContext.Connections
                    .FirstOrDefault(c => c.Id == id.ToUpper());

                if (connectionHandler is null)
                    throw new ProcessingException($"Connection not found", StatusCodes.Status400BadRequest);

                if (!connectionHandler.IsConnected)
                    throw new ProcessingException($"Connection is closed", StatusCodes.Status400BadRequest);

                var responseMessages = connectionHandler.SendAndReceiveMessage(
                    sendAndReceiveMessageRequestModel.Message,
                    sendAndReceiveMessageRequestModel.TimeOut);

                return new BaseResponseModel<SendAndReceiveMessageResponseModel>(new SendAndReceiveMessageResponseModel()
                {
                    Messages = responseMessages?.Select(m => new MessageListItemResponseModel(m))?.ToList(),
                    Port = connectionHandler.Port
                });
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Send message async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <param name="id"> Connection identifier. </param>
        /// <param name="sendMessageRequestModel"> Send message request model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<string>> SendMessageAsync(SessionDataModel session, string? id,
            SendMessageRequestModel sendMessageRequestModel)
        {
            return await ProcessAsync(() =>
            {
                if (string.IsNullOrEmpty(id))
                    throw new ProcessingException("Invalid connection identifier", StatusCodes.Status400BadRequest);

                if (string.IsNullOrEmpty(sendMessageRequestModel?.Message))
                    throw new ProcessingException("Invalid message", StatusCodes.Status400BadRequest);

                var connectionHandler = _serialPortDataContext.Connections
                    .FirstOrDefault(c => c.Id == id.ToUpper());

                if (connectionHandler is null)
                    throw new ProcessingException($"Connection not found", StatusCodes.Status400BadRequest);

                if (!connectionHandler.IsConnected)
                    throw new ProcessingException($"Connection is closed", StatusCodes.Status400BadRequest);

                connectionHandler.SendMessage(sendMessageRequestModel.Message);

                return new BaseResponseModel<string>(content: $"Message sent on port {connectionHandler.Port}");
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get message async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <param name="id"> Connection identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<MessageResponseModel>> GetMessageAsync(SessionDataModel session, string? id)
        {
            return await ProcessAsync(() =>
            {
                if (string.IsNullOrEmpty(id))
                    throw new ProcessingException("Invalid connection identifier", StatusCodes.Status400BadRequest);

                var connectionHandler = _serialPortDataContext.Connections
                   .FirstOrDefault(c => c.Id == id.ToUpper());

                if (connectionHandler is null)
                    throw new ProcessingException($"Connection not found", StatusCodes.Status400BadRequest);

                if (connectionHandler.IsAnyMessage)
                {
                    var message = connectionHandler.GetMessage();
                    return new BaseResponseModel<MessageResponseModel>(new MessageResponseModel(message, connectionHandler.Port));
                }

                return new BaseResponseModel<MessageResponseModel>(new MessageResponseModel(null, connectionHandler.Port));
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get last message async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <param name="id"> Connection identifier. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<MessageResponseModel>> GetLastMessageAsync(SessionDataModel session, string? id)
        {
            return await ProcessAsync(() =>
            {
                if (string.IsNullOrEmpty(id))
                    throw new ProcessingException("Invalid connection identifier", StatusCodes.Status400BadRequest);

                var connectionHandler = _serialPortDataContext.Connections
                   .FirstOrDefault(c => c.Id == id.ToUpper());

                if (connectionHandler is null)
                    throw new ProcessingException($"Connection not found", StatusCodes.Status400BadRequest);

                if (connectionHandler.IsAnyMessage)
                {
                    var message = connectionHandler.GetLastMessage();
                    return new BaseResponseModel<MessageResponseModel>(new MessageResponseModel(message, connectionHandler.Port));
                }

                return new BaseResponseModel<MessageResponseModel>(new MessageResponseModel(null, connectionHandler.Port));
            });
        }

        #endregion INTERACTION METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Close serial connection. </summary>
        /// <param name="connectionHandler"> Serial port connection handler. </param>
        /// <returns> True - connection closed; False - otherwise. </returns>
        /// <exception cref="ProcessingException"> Could not close connection or connection dose not exist. </exception>
        protected bool CloseConnection(SerialPortHandler connectionHandler)
        {
            if (connectionHandler is null)
                throw new ProcessingException($"Connection not found", StatusCodes.Status400BadRequest);

            if (!connectionHandler.IsConnected)
                throw new ProcessingException($"Connection already closed", StatusCodes.Status400BadRequest);

            connectionHandler.Disconnect();

            if (!connectionHandler.IsConnected)
            {
                _serialPortDataContext.RemoveConnection(connectionHandler);
                Logger.LogInformation($"Connection closed on port {connectionHandler.Port}");
                return true;
            }

            return false;
        }

        #endregion UTILITY METHODS

    }
}
