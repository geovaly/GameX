using SuperPlay.GameX.Backend.ApplicationLayer;

namespace SuperPlay.GameX.Backend.Tests.Shared
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
