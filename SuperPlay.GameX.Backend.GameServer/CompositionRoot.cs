using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RequestResponseFramework.Server;
using RequestResponseFramework.Server.MiddlewareExecutors;
using RequestResponseFramework.Server.WebSockets;
using Serilog;
using SuperPlay.GameX.Backend.GameServer.ApiLayer;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer.MiddlewareExecutors;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer.RequestHandlers;
using SuperPlay.GameX.Backend.GameServer.DomainLayer;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.MiddlewareExecutors;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.Repositories;
using SuperPlay.GameX.Backend.GameServer.PersistenceLayer.UsingEntityFrameworkCore;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;

namespace SuperPlay.GameX.Backend.GameServer
{
    public class CompositionRoot
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly string _databaseName = Guid.NewGuid().ToString();
        private WebSocketsRequestServerSettings _webSocketsRequestServerSettings;
        public CompositionRoot(WebSocketsRequestServerSettings webSocketsRequestServerSettings)
        {
            _webSocketsRequestServerSettings = webSocketsRequestServerSettings;
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public IGameServer GetGameServer()
        {
            return _serviceProvider.GetRequiredService<IGameServer>();
        }

        public WebSocketsGameServer GetWebSocketGameServer()
        {
            return _serviceProvider.GetRequiredService<WebSocketsGameServer>();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {

            serviceCollection
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog(Log.Logger, dispose: true);
                })
                .AddRequestResponseFramework(cfg =>
                    {
                        cfg.RegisterContractsFromAssemblyContaining<LoginCommand>();
                        cfg.RegisterHandlersFromAssemblyContaining<LoginCommandHandler>();
                        cfg.AddMiddlewareExecutor<HandleSystemExceptionMiddlewareExecutor>();
                        cfg.AddMiddlewareExecutor<EnsurePlayerIsLoggedInMiddlewareExecutor>();
                        cfg.AddMiddlewareExecutor<RetryOnConcurrencyExceptionMiddlewareExecutor>();
                    })
                .AddSingleton<ApplicationLayer.GameServer>()
                .AddSingleton<IGameServer, ApplicationLayer.GameServer>()
                .AddSingleton<IServerRequestExecutor>(x =>
                    (ApplicationLayer.GameServer)x.GetRequiredService<IGameServer>())
                .AddSingleton<WebSocketsRequestServer>()
                .AddSingleton<WebSocketsGameServer>()
                .AddSingleton<WebSocketsRequestServerSettings>(_ => _webSocketsRequestServerSettings)
                .AddSingleton<OnlinePlayerService>()
                .AddScoped<GameXDbContext>(_ => CreateInMemoryDbContext())
                .AddScoped<IPlayerRepository, PlayerRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private GameXDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<GameXDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;
            return new GameXDbContext(options);
        }
    }



}
