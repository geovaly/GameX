using SuperPlay.GameX.Backend.GameServer.DomainLayer.Data;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.Repositories
{
    internal interface IPlayerRepository
    {

        Task<Player?> LoadMaybeAsync(PlayerId playerId);
        Task<Player?> LoadMaybeByDeviceIdAsync(DeviceId deviceId);

        void AddOnSaveChanges(Player player);
    }

    internal static class PlayerRepositoryExtensions
    {

        public static async Task<Player> LoadAsync(this IPlayerRepository repository, PlayerId playerId)
            => await repository.LoadMaybeAsync(playerId) ?? throw new InvalidOperationException();

        public static async Task<Player> LoadByDeviceIdAsync(this IPlayerRepository repository, DeviceId deviceId)
            => await repository.LoadMaybeByDeviceIdAsync(deviceId) ?? throw new InvalidOperationException();


    }
}
