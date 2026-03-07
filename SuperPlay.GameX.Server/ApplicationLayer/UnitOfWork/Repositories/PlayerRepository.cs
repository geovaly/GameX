using SuperPlay.GameX.Server.DomainLayer.Data;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Server.ApplicationLayer.UnitOfWork.Repositories
{
    internal interface IPlayerRepository
    {

        Task<MutablePlayer?> LoadMaybeAsync(PlayerId playerId);
        Task<MutablePlayer?> LoadMaybeByDeviceIdAsync(DeviceId deviceId);

        void AddOnSaveChanges(MutablePlayer mutablePlayer);
    }

    internal static class PlayerRepositoryExtensions
    {

        public static async Task<MutablePlayer> LoadAsync(this IPlayerRepository repository, PlayerId playerId)
            => await repository.LoadMaybeAsync(playerId) ?? throw new InvalidOperationException();

        public static async Task<MutablePlayer> LoadByDeviceIdAsync(this IPlayerRepository repository, DeviceId deviceId)
            => await repository.LoadMaybeByDeviceIdAsync(deviceId) ?? throw new InvalidOperationException();


    }
}
