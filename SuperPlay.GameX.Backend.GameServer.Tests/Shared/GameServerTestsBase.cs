using SuperPlay.GameX.Backend.GameServer.ApplicationLayer;

namespace SuperPlay.GameX.Backend.GameServer.Tests.Shared
{
    public class GameServerTestsBase
    {
        protected async Task<IGameServer> StartGameServer()
        {
            var gameServer = new CompositionRoot().GetGameServer();
            await gameServer.StartAsync();
            return gameServer;
        }


    }
}
