using Microsoft.EntityFrameworkCore;
using SuperPlay.GameX.Server.App.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Server.App.DomainLayer.UnitOfWork.Repositories;

namespace SuperPlay.GameX.Server.App.PersistenceLayer.UsingEntityFrameworkCore
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
