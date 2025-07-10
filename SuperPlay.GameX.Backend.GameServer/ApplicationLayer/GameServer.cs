using RequestResponseFramework.Backend;
using RequestResponseFramework.Backend.MiddlewareExecutors;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer.MiddlewareExecutors;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.MiddlewareExecutors;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer
{
    public interface IGameServer : IServerRequestExecutor
    {
        bool IsRunning { get; }
        Task StartAsync();
    }

    internal class GameServer(IRequestScopeFactory requestScopeFactory) : IGameServer
    {
        private static readonly IReadOnlyList<Type> InOrderMiddlewareExecutorTypes = [
            typeof(HandleSystemExceptionMiddlewareExecutor), typeof(UnitOfWorkConcurrencyMiddlewareExecutor), typeof(EnsurePlayerIsLoggedInMiddlewareExecutor)];

        private readonly IServerRequestExecutor _serverRequestExecutor = new ServerRequestExecutor(requestScopeFactory, InOrderMiddlewareExecutorTypes);

        public bool IsRunning { get; private set; }

        public async Task<Response> TryExecuteAsync(Request request, IClientConnection? clientConnection = null)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException();
            }


            var result = await _serverRequestExecutor.TryExecuteAsync(request, clientConnection);
            return result;
        }

        public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request, IClientConnection? clientConnection = null) where TResult : RequestResult
        {
            return (await TryExecuteAsync(request as Request, clientConnection) as Response<TResult>)!;
        }

        public Task StartAsync()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException();
            }

            IsRunning = true;
            return Task.CompletedTask;
        }

    }
}
