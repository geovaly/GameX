using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Backend.GameServer.ApiLayer.UsingWebSockets;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer;
using SuperPlay.GameX.Backend.GameServer.DomainLayer;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.Repositories;
using SuperPlay.GameX.Backend.GameServer.PersistenceLayer.UsingEntityFrameworkCore;
using SuperPlay.GameX.Shared.GenericLayer.Logging;
using System.Reflection;

namespace SuperPlay.GameX.Backend.GameServer
{
    public class CompositionRoot
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly string _databaseName = Guid.NewGuid().ToString();

        public CompositionRoot()
        {

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public IGameServer GetGameServer()
        {
            return _serviceProvider.GetRequiredService<IGameServer>();
        }

        public WebSocketGameServer GetWebSocketGameServer()
        {
            return _serviceProvider.GetRequiredService<WebSocketGameServer>();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IRequestScopeFactory, RequestScopeFactory>()
                .AddSingleton<ApplicationLayer.GameServer>()
                .AddSingleton<IGameServer, ApplicationLayer.GameServer>()
                .AddSingleton<IServerRequestExecutor>(x =>
                    (ApplicationLayer.GameServer)x.GetRequiredService<IGameServer>())
                .AddSingleton<WebSocketGameServer>()
                .AddSingleton<OnlinePlayerService>()
                .AddSingleton<IRequestResponseLogger>(_ => new RequestResponseLoggerAdapter(Log.Logger))
                .AddScoped<GameXDbContext>(_ => CreateInMemoryDbContext())
                .AddScoped<IPlayerRepository, PlayerRepository>()
                .AddScoped<IClientConnectionProvider, ClientConnectionProviderImpl>()
                .AddScoped<IUnitOfWork, UnitOfWork>();


            AddScopedGenericTypesUsingReflection(serviceCollection, typeof(CompositionRoot).Assembly, typeof(IRequestHandler<,>));
            AddScopedTypesUsingReflection(serviceCollection, typeof(CompositionRoot).Assembly, typeof(IMiddlewareExecutor));
            AddScopedTypesUsingReflection(serviceCollection, typeof(IMiddlewareExecutor).Assembly, typeof(IMiddlewareExecutor));
        }

        private class Logger : ILogger
        {
            public void Debug(string message, params object[] args) => Log.Debug(message, args);
            public void Information(string message, params object[] args) => Log.Information(message, args);
            public void Warning(string message, params object[] args) => Log.Warning(message, args);
            public void Error(string message, params object[] args) => Log.Error(message, args);
            public void Error(Exception ex, string message, params object[] args) => Log.Error(ex, message, args);
            public void Fatal(string message, params object[] args) => Log.Fatal(message, args);
            public void Fatal(Exception ex, string message, params object[] args) => Log.Fatal(ex, message, args);
            public ValueTask CloseAndFlushAsync() => Log.CloseAndFlushAsync();
        }


        private class RequestScopeFactory(IServiceProvider serviceProvider) : IRequestScopeFactory
        {
            public IRequestScope<TRequest, TResult> Create<TRequest, TResult>(TRequest request, IClientConnection? clientConnection) where TRequest : Request<TResult> where TResult : RequestResult
            {
                return new RequestScopeImpl<TRequest, TResult>(request, clientConnection, serviceProvider.CreateAsyncScope());
            }
        }

        private record RequestScopeImpl<TRequest, TResult> : IRequestScope<TRequest, TResult> where TRequest : Request<TResult> where TResult : RequestResult
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

        private GameXDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<GameXDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;
            return new GameXDbContext(options);
        }

        private static void AddScopedTypesUsingReflection(IServiceCollection services, Assembly assembly,
            Type interfaceType)
        {

            var types = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
                .Where(x => x.Interface == interfaceType)
                .ToList();

            foreach (var pair in types)
            {
                services.AddScoped(pair.Interface, pair.Type);
            }
        }
        private static void AddScopedGenericTypesUsingReflection(IServiceCollection services, Assembly assembly,
            Type interfaceGenericType)
        {

            var types = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
                .Where(x => x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == interfaceGenericType)
                .ToList();

            foreach (var pair in types)
            {
                services.AddScoped(pair.Interface, pair.Type);
            }
        }

    }



}
