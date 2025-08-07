using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RequestResponseFramework.Server;
using RequestResponseFramework.Server.WebSockets;
using Serilog;
using SuperPlay.GameX.Server.App.ApiLayer;
using SuperPlay.GameX.Server.App.ApplicationLayer;
using SuperPlay.GameX.Server.App.ApplicationLayer.MiddlewareExecutors;
using SuperPlay.GameX.Server.App.ApplicationLayer.RequestHandlers;
using SuperPlay.GameX.Server.App.DomainLayer;
using SuperPlay.GameX.Server.App.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Server.App.DomainLayer.UnitOfWork.Repositories;
using SuperPlay.GameX.Server.App.PersistenceLayer.UsingEntityFrameworkCore;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Json;
using SuperPlay.GameX.Shared.GenericLayer.Enumerable;

namespace SuperPlay.GameX.Server.App
{
    public class CompositionRoot
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly string _databaseName = Guid.NewGuid().ToString();
        private readonly WebSocketRequestServerSettings _webSocketRequestServerSettings;

        public CompositionRoot(WebSocketRequestServerSettings webSocketRequestServerSettings)
        {
            _webSocketRequestServerSettings = webSocketRequestServerSettings;
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }


        public WebSocketGameServer GetWebSocketGameServer()
        {
            return _serviceProvider.GetRequiredService<WebSocketGameServer>();
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
                        cfg.RegisterContractsFromAssemblyContaining<Login>();
                        cfg.RegisterHandlersFromAssemblyContaining<LoginHandler>();
                        MiddlewareExecutorTypesProvider.OrderedTypes.ForEach(cfg.AddMiddlewareExecutor);
                        cfg.ConfigureJsonSerializerOptions(options => options.ConfigureDomainData());
                    })
                .AddSingleton<ApplicationLayer.GameServer>()
                .AddSingleton<IGameServer, ApplicationLayer.GameServer>()
                .AddSingleton<WebSocketRequestServer>()
                .AddSingleton<WebSocketGameServer>()
                .AddSingleton<WebSocketRequestServerSettings>(_ => _webSocketRequestServerSettings)
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
