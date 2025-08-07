using SuperPlay.GameX.Backend.DomainLayer.Data;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Backend.DomainLayer.UnitOfWork.Repositories
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
