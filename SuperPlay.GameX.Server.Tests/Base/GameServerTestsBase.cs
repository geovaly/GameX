using SuperPlay.GameX.Server.ApplicationLayer;

namespace SuperPlay.GameX.Server.Tests.Shared
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
