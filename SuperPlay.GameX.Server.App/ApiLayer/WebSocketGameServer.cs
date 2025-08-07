using RequestResponseFramework.Server.WebSockets;
using SuperPlay.GameX.Server.App.ApplicationLayer;

namespace SuperPlay.GameX.Server.App.ApiLayer
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
