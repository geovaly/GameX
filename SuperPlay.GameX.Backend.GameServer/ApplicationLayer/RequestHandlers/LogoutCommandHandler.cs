using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Backend.GameServer.DomainLayer;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer.RequestHandlers
{

    internal class LogoutCommandHandler(OnlinePlayerService onlinePlayerService) : CommandHandler<LogoutCommand, LogoutResult>
    {
        public override Task<Response<LogoutResult>> HandleAsync(LogoutCommand command)
        {
            return Task.FromResult(Handle(command));
        }

        private Response<LogoutResult> Handle(LogoutCommand command)
        {
            var playerId = command.Context.PlayerId;
            var playerWasLoggedIn = onlinePlayerService.RemoveOnlinePlayer(playerId);
            return CreateOk(new LogoutResult(playerWasLoggedIn));
        }
    }
}
