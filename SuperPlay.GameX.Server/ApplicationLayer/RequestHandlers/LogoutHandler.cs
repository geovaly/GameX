using RequestResponseFramework.Shared;
using RequestResponseFramework.Server;
using SuperPlay.GameX.Server.DomainLayer;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;

namespace SuperPlay.GameX.Server.ApplicationLayer.RequestHandlers
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
