using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.SystemExceptions;

namespace RequestResponseFramework.Backend.MiddlewareExecutors
{
    public class HandleSystemExceptionMiddlewareExecutor(IRequestResponseLogger requestResponseLogger) : IMiddlewareExecutor
    {

        public async Task<Response<TResult>> TryExecuteAsync<TRequest, TResult>(TRequest request, MiddlewareNextTryExecuteAsync<TRequest, TResult> nextTryExecuteAsync) where TRequest : Request<TResult> where TResult : RequestResult
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
                requestResponseLogger.Error(e, "HandleSystemExceptionMiddlewareExecutor Return Internal Server Error");
                return new NotOk<TResult>(new GenericRequestException("Internal Server Error"));
            }

        }
    }
}
