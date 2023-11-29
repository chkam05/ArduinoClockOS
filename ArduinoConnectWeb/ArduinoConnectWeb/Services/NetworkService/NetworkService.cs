using ArduinoConnectWeb.Models.Auth;
using ArduinoConnectWeb.Models.Base.ResponseModels;
using ArduinoConnectWeb.Models.Exceptions;
using ArduinoConnectWeb.Models.Network.ResponseModels;
using ArduinoConnectWeb.Services.Base;
using System.Net;

namespace ArduinoConnectWeb.Services.NetworkService
{
    public class NetworkService : DataProcessor, INetworkService
    {

        //  VARIABLES

        private readonly NetworkServiceConfig _config;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> NetworkService class constructor. </summary>
        /// <param name="config"> Network service config. </param>
        /// <param name="logger"> Application logger. </param>
        public NetworkService(NetworkServiceConfig config, ILogger<NetworkService> logger) : base(logger)
        {
            _config = config;
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get host info async </summary>
        /// <param name="session"> Session data model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<HostInfoResponseModel>> GetHostInfoAsync(SessionDataModel session)
        {
            return await ProcessTaskAsync(async () =>
            {
                var hostName = Dns.GetHostName();
                var hostEntry = await Dns.GetHostEntryAsync(hostName);

                if (hostEntry == null)
                    throw new ProcessingException("Unable to resolve hostname", StatusCodes.Status400BadRequest);

                var hostInfo = new HostInfoResponseModel()
                {
                    HostName = hostEntry.HostName,
                    Addresses = GetIpAddresses(hostEntry)
                };

                return new BaseResponseModel<HostInfoResponseModel>(hostInfo);
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get external host info async. </summary>
        /// <param name="session"> Session data model. </param>
        /// <returns> Response view model. </returns>
        public async Task<BaseResponseModel<ExternalHostInfoResponseModel>> GetExternalHostInfoAsync(SessionDataModel session)
        {
            return await ProcessTaskAsync(async () =>
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync("http://icanhazip.com");

                if (string.IsNullOrEmpty(response))
                    throw new ProcessingException("Unable to resolve external address", StatusCodes.Status400BadRequest);

                var extHostInfo = new ExternalHostInfoResponseModel()
                {
                    IpAddress = response.Trim()
                };

                return new BaseResponseModel<ExternalHostInfoResponseModel>(extHostInfo);
            });
        }

        #endregion INTERACTION METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get ip addresses from IPHostEntry. </summary>
        /// <param name="hostEntry"> Ip host entry. </param>
        /// <returns> List of IPAddressInfoResponseModel objects. </returns>
        private List<IPAddressInfoResponseModel> GetIpAddresses(IPHostEntry hostEntry)
        {
            List<IPAddressInfoResponseModel> result = new();

            bool isIPv4 = false;
            bool isIPv6 = false;

            IPAddressInfoResponseModel ipAddressInfo = new IPAddressInfoResponseModel();

            foreach (var ipAddress in hostEntry.AddressList)
            {
                var ip = ipAddress.ToString();

                if (ip.Contains(':'))
                {
                    if (isIPv6)
                    {
                        result.Add(ipAddressInfo);
                        isIPv4 = false;
                        isIPv6 = false;
                        ipAddressInfo = new IPAddressInfoResponseModel();
                    }

                    ipAddressInfo.IPv6 = ip;
                    ipAddressInfo.IPv6AddressFamily = ipAddress.AddressFamily.ToString();
                    ipAddressInfo.IPv6ScopeId = ipAddress.ScopeId;
                    isIPv6 = true;
                }

                if (ip.Contains('.'))
                {
                    if (isIPv4)
                    {
                        result.Add(ipAddressInfo);
                        isIPv4 = false;
                        isIPv6 = false;
                        ipAddressInfo = new IPAddressInfoResponseModel();
                    }

                    ipAddressInfo.IPv4 = ip;
                    ipAddressInfo.IPv4AddressFamily = ipAddress.AddressFamily.ToString();
                    isIPv4 = true;
                }

                if (isIPv6 && isIPv4)
                {
                    result.Add(ipAddressInfo);
                    isIPv4 = false;
                    isIPv6 = false;
                    ipAddressInfo = new IPAddressInfoResponseModel();
                }
            }

            return result;
        }

        #endregion UTILITY METHODS

    }
}
