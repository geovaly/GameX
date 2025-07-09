using RequestResponseFramework.Shared;

namespace RequestResponseFramework.Backend.MiddlewareExecutors
{
    public class HandleSystemExceptionMiddlewareExecutor : IMiddlewareExecutor
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
                return new NotOk<TResult>(new GenericRequestException($"{e.GetType().Name}: {e.Message}"));
            }

        }
    }
}
