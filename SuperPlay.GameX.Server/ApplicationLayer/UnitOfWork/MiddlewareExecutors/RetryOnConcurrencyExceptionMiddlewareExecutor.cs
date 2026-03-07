using RequestResponseFramework.Shared;
using RequestResponseFramework.Server;
using Serilog;
using System.Diagnostics;
using SuperPlay.GameX.Server.ApplicationLayer.UnitOfWork;

namespace SuperPlay.GameX.Server.ApplicationLayer.UnitOfWork.MiddlewareExecutors
{
    internal class RetryOnConcurrencyExceptionMiddlewareExecutor(IUnitOfWork unitOfWork) : IMiddlewareExecutor
    {
        private const int RetryMaxCount = 10;

        public async Task<Response<TResult>> TryExecuteAsync<TRequest, TResult>(TRequest request, MiddlewareNextTryExecuteAsync<TRequest, TResult> nextTryExecuteAsync)
            where TRequest : Request<TResult>
        {
            for (var i = 0; i <= RetryMaxCount; i++)
            {
                try
                {
                    return await nextTryExecuteAsync(request);
                }
                catch (UnitOfWorkConcurrencyException e)
                {
                    Log.Error(e, "RetryOnConcurrencyExceptionMiddlewareExecutor Catch UnitOfWorkConcurrencyException");
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