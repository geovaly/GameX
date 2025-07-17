using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Backend.GameServer.DomainLayer;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.Data;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer.RequestHandlers
{
    internal class LoginCommandHandler(OnlinePlayerService onlinePlayerService, IClientConnectionProvider clientConnectionProvider, IUnitOfWork unitOfWork) : CommandHandler<LoginCommand, LoginResult>
    {
        public override async Task<Response<LoginResult>> HandleAsync(LoginCommand command)
        {
            var player = await unitOfWork.PlayerRepository.LoadMaybeByDeviceIdAsync(command.DeviceId);
            if (player == null)
            {
                player = CreateNewPlayer(command);
                unitOfWork.PlayerRepository.AddOnSaveChanges(player);
                await unitOfWork.SaveChangesAsync();
            }

            var clientConnection = clientConnectionProvider.ClientConnection;
            var onlinePlayer = new OnlinePlayer(player.PlayerId, clientConnection);
            if (!onlinePlayerService.TryAddOnlinePlayer(onlinePlayer))
            {
                return CreateNotOk(new AlreadyConnectedException());
            }

            return CreateOk(new LoginResult(player.PlayerId));
        }

        private static Player CreateNewPlayer(LoginCommand command)
        {
            return Player.CreateNewPlayer(command.DeviceId);
        }
    }
}
