using Microsoft.Extensions.DependencyInjection;

namespace RequestResponseFramework.Client.Http
{

    public static class RequestResponseFrameworkServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpRequestResponseClient(this IServiceCollection services, HttpRequestClientSettings clientSettings)
        {
            services.AddSingleton<HttpRequestClient>();
            services.AddSingleton<HttpRequestClientSettings>((_) => clientSettings);
            return services;
        }
    }
}
