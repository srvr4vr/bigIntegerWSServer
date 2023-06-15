using BigBossApp;
using BigBossServer.WebSocket.Extensions;
using BigBossServer.WebSocket.Handlers;
using DiceServer.WebSockets;

namespace BigBossServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<IWebSocketAddressFactory, WebSocketAddressFactory>();
            services.AddSingleton<IBigNumberSource, BigNumberSource>();
                
                
            services.AddTransient<BigNumberSource>();
            services.AddSingleton<IBigNumberSource>(provider => 
                new UniqNumberDecorator(provider.GetRequiredService<BigNumberSource>()));;
            
            
            services.AddWebSocketManager();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } 
            app.UseRouting();
            app.UseWebSockets();

            app.MapWebSocketManager("/ws", serviceProvider.GetService<WebSocketHandler>()!);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("I'm a BigBoss");
                });

                endpoints.MapControllers();
            });
        }
    }
}
