using BigBossServer.WebSocket.Extensions;

namespace BigBossServer.Models
{
    public class Host
    {
        private string _address;

        public string Address
        {
            get => _address;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _address = value == "*" ? DnsExtension.GetIp() : value;
                }
            }
        }

        public int Port { get; set; }

        public void Bind(Host host)
        {
            if(!string.IsNullOrEmpty(host.Address))
            {
                Address = Address == "*" ? DnsExtension.GetIp() : host.Address;
            }

            if(host.Port != 0)
            {
                Port = host.Port;
            }
        }

        public string GetHostString() => $"{Address}:{Port}";

        public override string ToString() => GetHostString();
    }
}
