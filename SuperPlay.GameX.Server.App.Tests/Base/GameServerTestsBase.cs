using SuperPlay.GameX.Server.App.ApplicationLayer;

namespace SuperPlay.GameX.Server.App.Tests.Shared
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
