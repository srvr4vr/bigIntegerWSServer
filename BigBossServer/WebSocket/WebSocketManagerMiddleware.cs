using System.Net.WebSockets;
using BigBossServer.WebSocket.Extensions;
using BigBossServer.WebSocket.Handlers;

namespace BigBossServer.WebSocket
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler WebSocketHandler { get; }

        public WebSocketManagerMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            WebSocketHandler = webSocketHandler;
        }
        

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            
            var connectionResult = await WebSocketHandler.OnConnected(socket, context);

            if(!connectionResult)
                return;

            await Receive(socket, async (result, message) =>
            {
                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    await WebSocketHandler.ReceiveAsync(context.GetUserId(), message);
                    return;
                }

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await WebSocketHandler.OnDisconnected(socket);
                }
            });

            _next?.Invoke(context);
        }

        private static async Task Receive(System.Net.WebSockets.WebSocket socket, Action<WebSocketReceiveResult, ArraySegment<byte>> handleMessage)
        {
            var webSocketPayload = new List<byte>(1024 * 4);
            var tempMessage = new byte[1024 * 4]; 
            WebSocketReceiveResult? result;

            while (socket.State == WebSocketState.Open) {
                webSocketPayload.Clear();
                
                do
                {
                    result = await socket.ReceiveAsync(tempMessage, CancellationToken.None);
                    webSocketPayload.AddRange(new ArraySegment<byte>(tempMessage, 0, result.Count));
                }
                while (result.EndOfMessage == false);

                handleMessage(result, webSocketPayload.ToArray());
            }
        }
    }
}
