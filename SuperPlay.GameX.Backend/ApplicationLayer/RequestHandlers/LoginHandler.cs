using RequestResponseFramework.Shared;
using RequestResponseFramework.Server;
using SuperPlay.GameX.Backend.DomainLayer;
using SuperPlay.GameX.Backend.DomainLayer.Data;
using SuperPlay.GameX.Backend.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Backend.ApplicationLayer.RequestHandlers
{
    internal class LoginHandler(OnlinePlayerService onlinePlayerService, IClientConnectionProvider clientConnectionProvider, IUnitOfWork unitOfWork) : CommandHandler<Login, PlayerId>
    {
        public override async Task<Response<PlayerId>> HandleAsync(Login command)
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

            return CreateOk(player.PlayerId);
        }

        private static MutablePlayer CreateNewPlayer(Login command)
        {
            return MutablePlayer.CreateNewPlayer(command.DeviceId);
        }
    }
}
