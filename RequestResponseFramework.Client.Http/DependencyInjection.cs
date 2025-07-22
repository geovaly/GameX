using Microsoft.Extensions.DependencyInjection;

namespace RequestResponseFramework.Client.Http
{

    public static class RequestResponseFrameworkServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpRequestResponseClient(this IServiceCollection services, RequestResponseClientSettings clientSettings)
        {
            services.AddTransient<IRequestExecutor, HttpRequestResponseClient>();
            services.AddSingleton<IRequestResponseClientSettingsProvider>((_) => new RequestResponseClientSettingsProviderImpl(clientSettings));
            return services;
        }

        private class RequestResponseClientSettingsProviderImpl(RequestResponseClientSettings clientSettings) : IRequestResponseClientSettingsProvider
        {
            public RequestResponseClientSettings ClientSettings { get; } = clientSettings;
        }


    }
}
