using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using BigBossClient.Services.Models;
using Google.Protobuf;
using MessageClasses;

namespace BigBossClient.Services;

public class BigBossService : IBigBossService {
    public event Action<ServiceEventData>? OnReceiveMessage;
    
    private readonly ClientWebSocket _ws = new ();
    private string _userId;

    private CancellationTokenSource _cancellationTokenSource;

    public async Task Connect(string userId, string host) {
        _userId = userId;
        try {
            await _ws.ConnectAsync(new Uri($"ws://{host}/ws?userId={userId}"), CancellationToken.None);

            if (_ws.State == WebSocketState.Open) {
                OnReceiveMessage?.Invoke(ServiceEventData.CreateResult("Success connection"));
            }
        }
        catch (Exception e) {
            OnReceiveMessage?.Invoke(ServiceEventData.CreateError(e.Message));
        }
       
        _cancellationTokenSource = new CancellationTokenSource(); 
        handleConnection(_cancellationTokenSource.Token);
    }

    private async Task handleConnection(CancellationToken cancellationToken) {
        var webSocketPayload = new List<byte>(1024 * 4);
        var tempMessage = new byte[1024 * 4]; 

        while (_ws.State == WebSocketState.Open) {
            try {
                webSocketPayload.Clear();
                WebSocketReceiveResult? result;
                do
                {
                    result = await _ws.ReceiveAsync(tempMessage, CancellationToken.None);
                    webSocketPayload.AddRange(new ArraySegment<byte>(tempMessage, 0, result.Count));
                }
                while (result.EndOfMessage == false);
               
                ServiceEventData eventData;

                if (result.MessageType == WebSocketMessageType.Close) {
                    await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);

                    eventData = ServiceEventData.CreateError(result.CloseStatusDescription ?? "Connection close");
                }
                else {
                    var response = Response.Parser.ParseFrom(webSocketPayload.ToArray());

                    eventData = response.Result == Result.Error
                        ? ServiceEventData.CreateError(response.ErrorMessage)
                        : ServiceEventData.CreateResult(new BigInteger(response.Data.ToByteArray()).ToString());
                }

                OnReceiveMessage?.Invoke(eventData);
            }
            catch (Exception e) {
                OnReceiveMessage?.Invoke(ServiceEventData.CreateError(e.Message));
            }
        }
    }

    public async Task RequestNextNumber() {
        if (_ws.State == WebSocketState.Open) {
            var request = new Request() {
                Type = RequestType.GetNumber
            };

            await _ws.SendAsync(new ReadOnlyMemory<byte>(request.ToByteArray()),
                WebSocketMessageType.Binary,
                true,
                _cancellationTokenSource.Token);
        }
        else {
            OnReceiveMessage?.Invoke(ServiceEventData.CreateError("Connection closed"));
        }
    }

    public async Task Disconect() {
        if (_ws.State == WebSocketState.Open) {
            if (!_cancellationTokenSource.IsCancellationRequested) {
                _cancellationTokenSource.Cancel();
            }
            
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }
    }
}