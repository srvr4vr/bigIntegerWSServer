using BigBossServer.WebSocket.Extensions;
using DiceServer.WebSockets;

namespace BigBossServer
{
    public class WebSocketAddressFactory : IWebSocketAddressFactory
    {
        private readonly IConfiguration _config;

        private const string AddressPatternSectionName = "WebSocketServerAddressPattern";

        public WebSocketAddressFactory(IConfiguration config)
        {
            _config = config;
        }

        public string Create()
        {
            var hostString = GetHost();
            
            var pattern = _config.GetSection(AddressPatternSectionName).Value 
                ?? throw new ArgumentNullException(AddressPatternSectionName);

            return string.Format(pattern, hostString);
        }
        
        private string GetHost()
        {
            var hostSettings = new Models.Host();
            _config.GetSection("Host").Bind(hostSettings);

            var webSocketHostSettings = new Models.Host();
            _config.GetSection("WsHost").Bind(webSocketHostSettings);

            hostSettings.Bind(webSocketHostSettings);

            if (hostSettings.Address == "*")
            {
                hostSettings.Address = DnsExtension.GetIp();
            }

            return $"{hostSettings.Address}:{hostSettings.Port}";
        }
    }
}