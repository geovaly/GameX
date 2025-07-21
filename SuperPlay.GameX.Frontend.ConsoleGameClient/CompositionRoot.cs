using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RequestResponseFramework.Client;
using RequestResponseFramework.Client.WebSockets;
using Serilog;
using SuperPlay.GameX.Frontend.GameClient.ApiLayer;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;

namespace SuperPlay.GameX.Frontend.ConsoleGameClient
{
    public class CompositionRoot
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly WebSocketsRequestClientSettings _webSocketsRequestClientSettings;

        public CompositionRoot(WebSocketsRequestClientSettings webSocketsRequestClientSettings)
        {
            _webSocketsRequestClientSettings = webSocketsRequestClientSettings;
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public IGameClient GetGameClient()
        {
            return _serviceProvider.GetRequiredService<IGameClient>();
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
                })
                .AddSingleton<WebSocketsRequestClientSettings>(_ => _webSocketsRequestClientSettings)
                .AddSingleton<WebSocketsRequestClient>()
                .AddSingleton<IGameClient, WebSocketsGameClient>();
        }

    }



}
