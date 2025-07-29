using RequestResponseFramework.Server.WebSockets;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer;

namespace SuperPlay.GameX.Backend.GameServer.ApiLayer
{

    public class WebSocketGameServer(IGameServer gameServer, WebSocketRequestServer webSocketRequestServer)
    {

        public bool IsRunning => webSocketRequestServer.IsRunning;

        public async Task StartAsync()
        {
            if (!gameServer.IsRunning)
            {
                await gameServer.StartAsync();
            }

            await webSocketRequestServer.StartAsync();
        }

    }
}
