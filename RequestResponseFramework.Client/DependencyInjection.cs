using Microsoft.Extensions.DependencyInjection;
using RequestResponseFramework.Json;
using System.Reflection;
using System.Text.Json;

namespace RequestResponseFramework.Client
{

    public class RequestResponseFrameworkFeatureOptions
    {
        internal readonly HashSet<Assembly> ContractsAssembliesToScan = [];
        internal readonly JsonSerializerOptions JsonSerializerOptions = new();
        internal readonly PolymorphicJsonConverterFactory PolymorphicJsonConverterFactory = new();
        private readonly IServiceCollection _services;

        internal RequestResponseFrameworkFeatureOptions(IServiceCollection services)
        {
            _services = services;
            JsonSerializerOptions.Converters.Add(PolymorphicJsonConverterFactory);
        }

        public void ConfigureJsonSerializerOptions(Action<JsonSerializerOptions> action)
        {
            action(JsonSerializerOptions);
        }


        public void RegisterContractsFromAssemblyContaining<T>()
        {
            ContractsAssembliesToScan.Add(typeof(T).Assembly);
        }

        public void RegisterContractsFromAssembly(Assembly assembly)
        {
            ContractsAssembliesToScan.Add(assembly);
        }
    }

    public static class RequestResponseFrameworkServiceCollectionExtensions
    {
        public static IServiceCollection AddRequestResponseFramework(this IServiceCollection services, Action<RequestResponseFrameworkFeatureOptions> configure)
        {
            var options = new RequestResponseFrameworkFeatureOptions(services);
            configure(options);
            options.PolymorphicJsonConverterFactory.AddContracts(options.ContractsAssembliesToScan);
            services.AddSingleton<IJsonSerializerOptionsProvider>((_) => new JsonSerializerOptionsProvider(options.JsonSerializerOptions));
            return services;
        }

        private class JsonSerializerOptionsProvider(JsonSerializerOptions options) : IJsonSerializerOptionsProvider
        {
            public JsonSerializerOptions Options { get; } = options;
        }

    }
}
