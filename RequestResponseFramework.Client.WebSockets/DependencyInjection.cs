using Microsoft.Extensions.DependencyInjection;

namespace RequestResponseFramework.Client.WebSockets
{

    public static class RequestResponseFrameworkServiceCollectionExtensions
    {
        public static IServiceCollection AddWebSocketsRequestClient(this IServiceCollection services, WebSocketsRequestClientSettings clientSettings)
        {
            services.AddSingleton<WebSocketsRequestClient>();
            services.AddSingleton<WebSocketsRequestClientSettings>((_) => clientSettings);
            return services;
        }
    }
}