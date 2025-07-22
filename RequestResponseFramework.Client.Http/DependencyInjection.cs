using Microsoft.Extensions.DependencyInjection;

namespace RequestResponseFramework.Client.Http
{

    public static class RequestResponseFrameworkServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpRequestResponseClient(this IServiceCollection services, HttpRequestClientSettings clientSettings)
        {
            services.AddTransient<IRequestExecutor, HttpRequestResponseClient>();
            services.AddSingleton<IHttpRequestClientSettingsProvider>((_) => new HttpRequestClientSettingsProviderImpl(clientSettings));
            return services;
        }

        private class HttpRequestClientSettingsProviderImpl(HttpRequestClientSettings clientSettings) : IHttpRequestClientSettingsProvider
        {
            public HttpRequestClientSettings ClientSettings { get; } = clientSettings;
        }


    }
}
