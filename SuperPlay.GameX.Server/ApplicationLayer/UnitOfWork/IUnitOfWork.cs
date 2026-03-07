using SuperPlay.GameX.Server.ApplicationLayer.UnitOfWork.Repositories;

namespace SuperPlay.GameX.Server.ApplicationLayer.UnitOfWork
{
    internal interface IUnitOfWork : IAsyncDisposable
    {
        void ClearTrackedEntities();
        IPlayerRepository PlayerRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
