using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RequestResponseFramework.Client;
using RequestResponseFramework.Client.WebSockets;
using Serilog;
using SuperPlay.GameX.Client.ApiLayer;
using SuperPlay.GameX.Client.ApplicationLayer;
using SuperPlay.GameX.Client.ConsoleUI.UserInterfaceLayer;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Json;

namespace SuperPlay.GameX.Client.ConsoleUI
{
    public class CompositionRoot
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly WebSocketRequestClientSettings _webSocketRequestClientSettings;

        public CompositionRoot(WebSocketRequestClientSettings webSocketRequestClientSettings)
        {
            _webSocketRequestClientSettings = webSocketRequestClientSettings;
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public GameConsole GetGameConsole()
        {
            return _serviceProvider.GetRequiredService<GameConsole>();
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
                    cfg.ConfigureJsonSerializerOptions(options => options.ConfigureDomainData());
                })
                .AddWebSocketsRequestClient(_webSocketRequestClientSettings)
                .AddSingleton<IGameClient, WebSocketGameClient>()
                .AddSingleton<GameConsole>();
        }

    }



}
