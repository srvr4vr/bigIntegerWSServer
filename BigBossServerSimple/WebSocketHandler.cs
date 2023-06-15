using System.Net.WebSockets;
using BigBossApp;
using Google.Protobuf;
using MessageClasses;

namespace BigBossServerSimple; 

public static class WebSocketHandler {

    private static IBigNumberSource _numberSource = new UniqNumberDecorator(new BigNumberSource());
    
    public static async Task HandleWebSocket(WebSocket socket)
    {
        var webSocketPayload = new List<byte>(1024 * 4);
        var tempMessage = new byte[1024 * 4]; 

        while (socket.State == WebSocketState.Open)
        {
            webSocketPayload.Clear();

            WebSocketReceiveResult? webSocketResponse;

            do
            {
                webSocketResponse = await socket.ReceiveAsync(tempMessage, CancellationToken.None);
                webSocketPayload.AddRange(new ArraySegment<byte>(tempMessage, 0, webSocketResponse.Count));
            }
            while (webSocketResponse.EndOfMessage == false);
            
            if (webSocketResponse.MessageType == WebSocketMessageType.Binary) 
            {
                var request = Request.Parser.ParseFrom(webSocketPayload.ToArray());
                var response = new Response();

                if (request.Type == RequestType.GetNumber) 
                {
                    var number = await _numberSource.getNumberAsync();
                    response.Result = Result.Ok;
                    response.Data = ByteString.CopyFrom(number.ToByteArray());
                    
                    await SendAsync(socket, response);
                }
                else 
                {
                    response.Result = Result.Error;
                    response.ErrorMessage = "Wrong Request";
                    await SendAsync(socket, response);
                }
            }
        }
    }

    private static async Task SendAsync(WebSocket socket, IMessage response) {
        await socket.SendAsync(buffer: new ArraySegment<byte>(response.ToByteArray()),
            messageType: WebSocketMessageType.Binary,
            endOfMessage: true,
            cancellationToken: CancellationToken.None);
    }
}