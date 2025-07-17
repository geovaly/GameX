using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Backend.GameServer.DomainLayer;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer.MiddlewareExecutors
{
    internal class EnsurePlayerIsLoggedInMiddlewareExecutor(OnlinePlayerService onlinePlayerService, IClientConnectionProvider clientConnectionProvider, IUnitOfWork unitOfWork) : IMiddlewareExecutor
    {
        public async Task<Response<TResult>> TryExecuteAsync<TRequest, TResult>(TRequest request, MiddlewareNextTryExecuteAsync<TRequest, TResult> nextTryExecuteAsync)
            where TRequest : Request<TResult> where TResult : RequestResult
        {
            if (request is LoginCommand or LogoutCommand) return await nextTryExecuteAsync(request);
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