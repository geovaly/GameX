using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Shared.GenericLayer.Logging;
using System.Diagnostics;

namespace SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.MiddlewareExecutors
{
    internal class UnitOfWorkConcurrencyMiddlewareExecutor(IUnitOfWork unitOfWork) : IMiddlewareExecutor
    {
        private const int RetryMaxCount = 10;

        public async Task<Response<TResult>> ExecuteAsync<TRequest, TResult>(TRequest request, MiddlewareNextExecute<TRequest, TResult> nextExecute)
            where TRequest : Request<TResult> where TResult : RequestResult
        {
            for (var i = 0; i <= RetryMaxCount; i++)
            {
                try
                {
                    return await nextExecute(request);
                }
                catch (UnitOfWorkConcurrencyException e)
                {
                    Log.Error(e, "UnitOfWorkConcurrencyMiddlewareExecutor Catch UnitOfWorkConcurrencyException");
                    if (i == RetryMaxCount)
                    {
                        throw;
                    }
                    unitOfWork.ClearTrackedEntities();
                }
            }

            throw new UnreachableException();
        }
    }
}