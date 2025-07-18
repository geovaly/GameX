using RequestResponseFramework;

using RequestResponseFramework.Server;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer
{
    internal class GameServer(IServerRequestExecutor serverRequestExecutor) : IGameServer
    {

        public bool IsRunning { get; private set; }

        public async Task<IResponse> TryExecuteAsync(IRequest request, IClientConnection? clientConnection = null)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException();
            }


            var result = await serverRequestExecutor.TryExecuteAsync(request, clientConnection);
            return result;
        }

        public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request, IClientConnection? clientConnection = null)
        {
            return (await TryExecuteAsync(request as IRequest, clientConnection) as Response<TResult>)!;
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
