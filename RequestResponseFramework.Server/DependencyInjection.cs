using Microsoft.Extensions.DependencyInjection;
using RequestResponseFramework.Json;
using System.Reflection;
using System.Text.Json;

namespace RequestResponseFramework.Server
{

    public class RequestResponseFrameworkFeatureOptions
    {
        internal readonly HashSet<Assembly> HandlerAssembliesToScan = [];
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
        public void RegisterHandlersFromAssemblyContaining<T>()
        {
            HandlerAssembliesToScan.Add(typeof(T).Assembly);
        }

        public void RegisterHandlersFromAssembly(Assembly assembly)
        {
            HandlerAssembliesToScan.Add(assembly);
        }

        public void RegisterContractsFromAssemblyContaining<T>()
        {
            ContractsAssembliesToScan.Add(typeof(T).Assembly);
        }

        public void RegisterContractsFromAssembly(Assembly assembly)
        {
            ContractsAssembliesToScan.Add(assembly);
        }
        public void AddMiddlewareExecutor<T>() where T : class, IMiddlewareExecutor
        {
            _services.AddTransient<IMiddlewareExecutor, T>();
        }
    }


    public static class RequestResponseFrameworkServiceCollectionExtensions
    {
        public static IServiceCollection AddRequestResponseFramework(this IServiceCollection services, Action<RequestResponseFrameworkFeatureOptions> configure)
        {
            var options = new RequestResponseFrameworkFeatureOptions(services);
            configure(options);

            foreach (var assembly in options.HandlerAssembliesToScan)
            {
                AddTransientGenericTypesUsingReflection(services, assembly, typeof(IRequestHandler<,>));
            }
            options.PolymorphicJsonConverterFactory.AddContracts(options.ContractsAssembliesToScan);

            services.AddSingleton<IRequestScopeFactory, RequestScopeFactory>();
            services.AddSingleton<IJsonSerializerOptionsProvider>((_) => new JsonSerializerOptionsProviderImpl(options.JsonSerializerOptions));
            services.AddScoped<IClientConnectionProvider, ClientConnectionProviderImpl>();
            services.AddSingleton<IServerRequestExecutor, ServerRequestExecutor>();
            return services;
        }

        private class JsonSerializerOptionsProviderImpl(JsonSerializerOptions options) : IJsonSerializerOptionsProvider
        {
            public JsonSerializerOptions Options { get; } = options;
        }

        private class RequestScopeFactory(IServiceProvider serviceProvider) : IRequestScopeFactory
        {
            public IRequestScope<TRequest, TResult> Create<TRequest, TResult>(TRequest request, IClientConnection? clientConnection) where TRequest : Request<TResult>
            {
                return new RequestScopeImpl<TRequest, TResult>(request, clientConnection, serviceProvider.CreateAsyncScope());
            }
        }

        private record RequestScopeImpl<TRequest, TResult> : IRequestScope<TRequest, TResult> where TRequest : Request<TResult>
        {
            public RequestScopeImpl(TRequest request, IClientConnection? clientConnection, AsyncServiceScope serviceScope)
            {
                Request = request;
                ServiceScope = serviceScope;
                ClientConnection = clientConnection;
                var clientConnectionProvider = (ClientConnectionProviderImpl)ServiceScope.ServiceProvider.GetRequiredService<IClientConnectionProvider>();
                clientConnectionProvider.ClientConnection = clientConnection;
                RequestHandler = ServiceScope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResult>>();
                MiddlewareExecutors = ServiceScope.ServiceProvider.GetRequiredService<IEnumerable<IMiddlewareExecutor>>();
            }
            public ValueTask DisposeAsync() => ServiceScope.DisposeAsync();
            public TRequest Request { get; }
            public IClientConnection? ClientConnection { get; }
            private AsyncServiceScope ServiceScope { get; }
            public IRequestHandler<TRequest, TResult> RequestHandler { get; }
            public IEnumerable<IMiddlewareExecutor> MiddlewareExecutors { get; }
        }

        private class ClientConnectionProviderImpl : IClientConnectionProvider
        {
            public IClientConnection? ClientConnection { get; set; }
        }

        private static void AddTransientTypesUsingReflection(IServiceCollection services, Assembly assembly,
            Type interfaceType)
        {

            var types = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
                .Where(x => x.Interface == interfaceType)
                .ToList();

            foreach (var pair in types)
            {
                services.AddTransient(pair.Interface, pair.Type);
            }
        }
        private static void AddTransientGenericTypesUsingReflection(IServiceCollection services, Assembly assembly,
            Type interfaceGenericType)
        {

            var types = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
                .Where(x => x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == interfaceGenericType)
                .ToList();

            foreach (var pair in types)
            {
                services.AddTransient(pair.Interface, pair.Type);
            }
        }
    }
}
