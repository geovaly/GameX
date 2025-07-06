using Microsoft.EntityFrameworkCore;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.Data;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.Repositories;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Backend.GameServer.PersistenceLayer.UsingEntityFrameworkCore
{
    internal class PlayerRepository(GameXDbContext dbContext) : IPlayerRepository
    {
        public Task<Player?> LoadMaybeAsync(PlayerId playerId)
        {
            return dbContext.Players.SingleOrDefaultAsync(x => x.PlayerId == playerId);
        }

        public Task<Player?> LoadMaybeByDeviceIdAsync(DeviceId deviceId)
        {
            return dbContext.Players.SingleOrDefaultAsync(x => x.DeviceId == deviceId);
        }

        public void AddOnSaveChanges(Player player)
        {
            dbContext.Players.Add(player);
        }
    }
}
