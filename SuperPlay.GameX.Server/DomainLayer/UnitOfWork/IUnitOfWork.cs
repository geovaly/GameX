using SuperPlay.GameX.Backend.DomainLayer.UnitOfWork.Repositories;

namespace SuperPlay.GameX.Backend.DomainLayer.UnitOfWork
{
    internal interface IUnitOfWork : IAsyncDisposable
    {
        void ClearTrackedEntities();
        IPlayerRepository PlayerRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
