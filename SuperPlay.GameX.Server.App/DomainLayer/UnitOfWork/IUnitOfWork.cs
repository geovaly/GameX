using SuperPlay.GameX.Server.App.DomainLayer.UnitOfWork.Repositories;

namespace SuperPlay.GameX.Server.App.DomainLayer.UnitOfWork
{
    internal interface IUnitOfWork : IAsyncDisposable
    {
        void ClearTrackedEntities();
        IPlayerRepository PlayerRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
