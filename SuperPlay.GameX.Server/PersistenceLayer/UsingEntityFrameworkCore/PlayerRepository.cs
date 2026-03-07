using Microsoft.EntityFrameworkCore;
using SuperPlay.GameX.Server.ApplicationLayer.UnitOfWork.Repositories;
using SuperPlay.GameX.Server.DomainLayer.Data;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Server.PersistenceLayer.UsingEntityFrameworkCore
{
    internal class PlayerRepository(GameXDbContext dbContext) : IPlayerRepository
    {
        public Task<MutablePlayer?> LoadMaybeAsync(PlayerId playerId)
        {
            return dbContext.Players.SingleOrDefaultAsync(x => x.PlayerId == playerId);
        }

        public Task<MutablePlayer?> LoadMaybeByDeviceIdAsync(DeviceId deviceId)
        {
            return dbContext.Players.SingleOrDefaultAsync(x => x.DeviceId == deviceId);
        }

        public void AddOnSaveChanges(MutablePlayer mutablePlayer)
        {
            dbContext.Players.Add(mutablePlayer);
        }
    }
}
