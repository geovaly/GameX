using RequestResponseFramework.Backend.UsingWebSockets;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer;
using SuperPlay.GameX.Shared.ApplicationLayer;
using SuperPlay.GameX.Shared.GenericLayer.Logging;

namespace SuperPlay.GameX.Backend.GameServer.ApiLayer.UsingWebSockets
{

    public class WebSocketGameServer(IGameServer gameServer)
    {
        private const string Prefix = "http://localhost:5000/ws/";

        private readonly WebSocketServer _server = new(
            new RequestResponseLoggerAdapter(Log.Logger),
            gameServer,
            ApplicationJsonSerializerOptions.Options,
            prefix: Prefix);


        public bool IsRunning => _server.IsRunning;

        public async Task StartAsync()
        {
            if (!gameServer.IsRunning)
            {
                await gameServer.StartAsync();
            }

            await _server.StartAsync();
        }

    }
}
