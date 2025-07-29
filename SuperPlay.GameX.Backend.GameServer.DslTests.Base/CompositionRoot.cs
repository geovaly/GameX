using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RequestResponseFramework.Server;
using RequestResponseFramework.Server.MiddlewareExecutors;
using Serilog;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer.MiddlewareExecutors;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer.RequestHandlers;
using SuperPlay.GameX.Backend.GameServer.DomainLayer;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.MiddlewareExecutors;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.Repositories;
using SuperPlay.GameX.Backend.GameServer.PersistenceLayer.UsingEntityFrameworkCore;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Json;

namespace SuperPlay.GameX.Backend.GameServer.DslTests.Base
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
                        cfg.AddMiddlewareExecutor<HandleSystemExceptionMiddlewareExecutor>();
                        cfg.AddMiddlewareExecutor<EnsurePlayerIsLoggedInMiddlewareExecutor>();
                        cfg.AddMiddlewareExecutor<RetryOnConcurrencyExceptionMiddlewareExecutor>();
                        cfg.ConfigureJsonSerializerOptions(options => options.ConfigureDomainData());
                    })
                .AddSingleton<ApplicationLayer.GameServer>()
                .AddSingleton<IGameServer, ApplicationLayer.GameServer>()
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
