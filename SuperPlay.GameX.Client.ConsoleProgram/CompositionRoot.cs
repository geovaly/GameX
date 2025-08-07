using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RequestResponseFramework.Client;
using RequestResponseFramework.Client.WebSockets;
using Serilog;
using SuperPlay.GameX.Client.ConsoleProgram.ApplicationLayer;
using SuperPlay.GameX.Client.GameClient.ApiLayer;
using SuperPlay.GameX.Client.GameClient.ApplicationLayer;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Json;

namespace SuperPlay.GameX.Client.ConsoleProgram
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

        public ConsoleGame GetConsoleGame()
        {
            return _serviceProvider.GetRequiredService<ConsoleGame>();
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
                .AddSingleton<ConsoleGame>();
        }

    }



}
