using Microsoft.EntityFrameworkCore;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.Repositories;

namespace SuperPlay.GameX.Backend.GameServer.PersistenceLayer.UsingEntityFrameworkCore
{
    internal class UnitOfWork(GameXDbContext dbContext, IPlayerRepository playerRepository) : IUnitOfWork
    {
        public void ClearTrackedEntities() => dbContext.ChangeTracker.Clear();

        public IPlayerRepository PlayerRepository { get; } = playerRepository;

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new UnitOfWorkConcurrencyException(e);
            }
        }

        public ValueTask DisposeAsync() => dbContext.DisposeAsync();
    }
}
