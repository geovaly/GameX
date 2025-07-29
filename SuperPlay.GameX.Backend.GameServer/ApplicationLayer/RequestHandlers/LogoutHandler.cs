using RequestResponseFramework;
using RequestResponseFramework.Server;
using SuperPlay.GameX.Backend.GameServer.DomainLayer;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer.RequestHandlers
{

    internal class LogoutHandler(OnlinePlayerService onlinePlayerService) : CommandHandler<Logout, bool>
    {
        public override Task<Response<bool>> HandleAsync(Logout command)
        {
            return Task.FromResult(Handle(command));
        }

        private Response<bool> Handle(Logout command)
        {
            var playerId = command.Context.PlayerId;
            var playerWasLoggedIn = onlinePlayerService.RemoveOnlinePlayer(playerId);
            return CreateOk(playerWasLoggedIn);
        }
    }
}
