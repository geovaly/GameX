using Microsoft.Extensions.DependencyInjection;

namespace RequestResponseFramework.Client.WebSockets
{

    public static class RequestResponseFrameworkServiceCollectionExtensions
    {
        public static IServiceCollection AddWebSocketsRequestClient(this IServiceCollection services, WebSocketRequestClientSettings clientSettings)
        {
            services.AddSingleton<WebSocketRequestClient>();
            services.AddSingleton<IRequestExecutor>(s => s.GetRequiredService<WebSocketRequestClient>());
            services.AddSingleton<WebSocketRequestClientSettings>((_) => clientSettings);
            return services;
        }
    }
}