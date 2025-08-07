using RequestResponseFramework;
using RequestResponseFramework.Server;
using SuperPlay.GameX.Backend.DomainLayer;
using SuperPlay.GameX.Backend.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;

namespace SuperPlay.GameX.Backend.ApplicationLayer.MiddlewareExecutors
{
    internal class EnsurePlayerIsLoggedInMiddlewareExecutor(OnlinePlayerService onlinePlayerService, IClientConnectionProvider clientConnectionProvider, IUnitOfWork unitOfWork) : IMiddlewareExecutor
    {
        public async Task<Response<TResult>> TryExecuteAsync<TRequest, TResult>(TRequest request, MiddlewareNextTryExecuteAsync<TRequest, TResult> nextTryExecuteAsync)
            where TRequest : Request<TResult>
        {
            if (request is Login or Logout) return await nextTryExecuteAsync(request);
            var loggedInRequest = (ILoggedInRequest)request;
            var playerId = loggedInRequest.Context.PlayerId;

            var onlinePlayer = onlinePlayerService.GetOnlinePlayer(playerId);
            if (onlinePlayer == null)
            {
                return new NotOk<TResult>(new PlayerNotConnectedException());
            }

            if (onlinePlayer.IsConnectionMismatch(clientConnectionProvider))
            {
                return new NotOk<TResult>(new ConnectionMismatchException());
            }

            var player = await unitOfWork.PlayerRepository.LoadMaybeAsync(playerId);
            if (player == null)
            {
                return new NotOk<TResult>(new PlayerNotFoundException(playerId));
            }

            return await nextTryExecuteAsync(request);
        }
    }
}