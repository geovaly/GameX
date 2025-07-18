using RequestResponseFramework;
using RequestResponseFramework.Server;
using SuperPlay.GameX.Backend.GameServer.DomainLayer;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer.RequestHandlers
{

    internal class LogoutCommandHandler(OnlinePlayerService onlinePlayerService) : CommandHandler<LogoutCommand, bool>
    {
        public override Task<Response<bool>> HandleAsync(LogoutCommand command)
        {
            return Task.FromResult(Handle(command));
        }

        private Response<bool> Handle(LogoutCommand command)
        {
            var playerId = command.Context.PlayerId;
            var playerWasLoggedIn = onlinePlayerService.RemoveOnlinePlayer(playerId);
            return CreateOk(playerWasLoggedIn);
        }
    }
}
