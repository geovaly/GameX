using SuperPlay.GameX.Server.DomainLayer.UnitOfWork.Repositories;

namespace SuperPlay.GameX.Server.DomainLayer.UnitOfWork
{
    internal interface IUnitOfWork : IAsyncDisposable
    {
        void ClearTrackedEntities();
        IPlayerRepository PlayerRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
