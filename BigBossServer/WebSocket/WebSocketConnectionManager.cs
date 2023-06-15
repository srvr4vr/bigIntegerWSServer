using System.Collections.Concurrent;
using System.Net.WebSockets;
using BigBossServer.WebSocket.Extensions;

namespace BigBossServer.WebSocket
{
    public class WebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> _sockets = new();

        public System.Net.WebSockets.WebSocket? GetSocketById(string token) 
            => _sockets.FirstOrDefault(p => p.Key == token).Value;

        public ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> GetAll() => _sockets;

        public string GetId(System.Net.WebSockets.WebSocket socket) => _sockets.FirstOrDefault(p => p.Value == socket).Key;

        public void AddSocket(System.Net.WebSockets.WebSocket socket, HttpContext context)
        {
            _sockets.TryAdd(context.GetUserId(), socket);
        }

        public async Task RemoveSocket(string token)
        {
            _sockets.TryRemove(token, out var socket);

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                "Closed by the WebSocketManager",
                CancellationToken.None);
        }

        public async Task RemoveSocket(System.Net.WebSockets.WebSocket socket)
        {
            var token = _sockets.FirstOrDefault(p => p.Value == socket).Key;

            _sockets.TryRemove(token, out _);

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                "Closed by the WebSocketManager",
                CancellationToken.None);
        }
    }
}