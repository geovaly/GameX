using RequestResponseFramework.Server.WebSockets;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer;

namespace SuperPlay.GameX.Backend.GameServer.ApiLayer
{

    public class WebSocketsGameServer(IGameServer gameServer, WebSocketsRequestServer webSocketsRequestServer)
    {

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
