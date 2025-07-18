using RequestResponseFramework.Server.WebSockets;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer;

namespace SuperPlay.GameX.Backend.GameServer.ApiLayer
{
    //private const string Prefix = "http://localhost:5000/ws/";

    public class WebSocketsGameServer(IGameServer gameServer, WebSocketsRequestServer webSocketsRequestServer)
    {
        private const string Prefix = "http://localhost:5000/ws/";

        public bool IsRunning => webSocketsRequestServer.IsRunning;

        public async Task StartAsync()
        {
            if (!gameServer.IsRunning)
            {
                await gameServer.StartAsync();
            }

            await webSocketsRequestServer.StartAsync();
        }

    }
}
