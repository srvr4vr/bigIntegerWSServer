using BigBossServer.WebSocket.Handlers;

namespace BigBossServer.WebSocket.Extensions
{
    public static class WebSocketMiddlewareExtensions
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder applicationBuilder, PathString path, WebSocketHandler handler) => 
            applicationBuilder.Map(path, (app) => app.UseMiddleware<WebSocketManagerMiddleware>(handler));

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddSingleton<WebSocketConnectionManager>();
            services.AddSingleton<WebSocketHandler>();
            return services;
        }

        public static string GetParam(this HttpContext context, string paramName)
            => context.Request.Query[paramName];

        public static string GetUserId(this HttpContext context)
            => context.GetParam("userId");
    }
}
