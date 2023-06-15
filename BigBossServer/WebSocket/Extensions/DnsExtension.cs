using System.Net;
using System.Net.Sockets;

namespace BigBossServer.WebSocket.Extensions
{
    public static class DnsExtension
    {
        public static string GetIp() => 
            Dns.GetHostEntry(Dns.GetHostName())
            .AddressList
            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)
            ?.ToString() ?? "*";
    }
}