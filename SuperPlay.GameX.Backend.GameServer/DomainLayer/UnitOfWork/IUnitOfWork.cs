using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.Repositories;

namespace SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork
{
    internal interface IUnitOfWork : IAsyncDisposable
    {
        void ClearTrackedEntities();
        IPlayerRepository PlayerRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
