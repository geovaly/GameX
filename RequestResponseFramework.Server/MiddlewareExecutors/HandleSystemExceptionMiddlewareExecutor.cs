using Microsoft.Extensions.Logging;
using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.RequestExceptions;
using RequestResponseFramework.Shared.SystemExceptions;

namespace RequestResponseFramework.Server.MiddlewareExecutors
{
    public class HandleSystemExceptionMiddlewareExecutor(ILogger<HandleSystemExceptionMiddlewareExecutor> logger) : IMiddlewareExecutor
    {

        public async Task<Response<TResult>> TryExecuteAsync<TRequest, TResult>(TRequest request, MiddlewareNextTryExecuteAsync<TRequest, TResult> nextTryExecuteAsync) where TRequest : Request<TResult>
        {
            try
            {
                return await nextTryExecuteAsync(request);
            }
            catch (RequestSystemException e)
            {
                return new NotOk<TResult>(e.RequestException);
            }
            catch (Exception e)
            {
                var guid = Guid.NewGuid();
                logger.LogError(e, "Internal Server Error {guid}", guid);
                return new NotOk<TResult>(new InternalServerErrorException(guid));
            }

        }
    }
}
