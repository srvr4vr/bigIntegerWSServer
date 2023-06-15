using System.Net.WebSockets;
using BigBossApp;
using BigBossServer.WebSocket.Extensions;
using Google.Protobuf;
using MessageClasses;

namespace BigBossServer.WebSocket.Handlers
{
    public class WebSocketHandler
    {
        private readonly WebSocketConnectionManager _webSocketConnectionManager;
        private readonly IBigNumberSource _source;

        private static Response _userAlreadyConnectedErrorMessage = new Response 
        {
            Result = Result.Error,
            ErrorMessage = "User already online"
        };

        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager, IBigNumberSource source) 
        {
            _webSocketConnectionManager = webSocketConnectionManager;
            _source = source;
        }

        public async Task<bool> OnConnected(System.Net.WebSockets.WebSocket socket, HttpContext context) 
        {
            if (_webSocketConnectionManager.GetAll().TryGetValue(context.GetUserId(), out _)) {
                await SendMessageAsync(socket, _userAlreadyConnectedErrorMessage);
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    "Closed by the WebSocketManager",
                    CancellationToken.None);
                return false;

            }

            _webSocketConnectionManager.AddSocket(socket, context);
            return true;
        }

        public async Task OnDisconnected(System.Net.WebSockets.WebSocket socket)
        {
            await _webSocketConnectionManager.RemoveSocket(socket);
        }


        public async Task ReceiveAsync(string userId, ArraySegment<byte> data)
        {
            var request = Request.Parser.ParseFrom(data);

            var socket = _webSocketConnectionManager.GetSocketById(userId);

            if (socket != null) //WTF?
            {
                var response = new Response();
                
                if (request.Type == RequestType.GetNumber) 
                {
                    var number = await _source.GetNumberAsync();
                    response.Result = Result.Ok;
                    response.Data = ByteString.CopyFrom(number.ToByteArray());
                    
                    await SendMessageAsync(socket, response);
                }
                else 
                {
                    response.Result = Result.Error;
                    response.ErrorMessage = "Wrong Request";
                    await SendMessageAsync(socket, response);
                }
            }
        }

        public async Task SendMessageAsync(System.Net.WebSockets.WebSocket socket, IMessage message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(message.ToByteArray()),
                messageType: WebSocketMessageType.Binary,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageAsync(string socketId, IMessage message)
            => await SendMessageAsync(_webSocketConnectionManager.GetSocketById(socketId), message);
    }
}
