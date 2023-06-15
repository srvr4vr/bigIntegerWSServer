using System.Net;
using System.Net.WebSockets;
using BigBossServerSimple;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();
app.MapGet("/", () => "Hello World!");

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("connect new user");

        while (webSocket.State == WebSocketState.Open)
        {
            await WebSocketHandler.HandleWebSocket(webSocket);
        }
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

app.Run();