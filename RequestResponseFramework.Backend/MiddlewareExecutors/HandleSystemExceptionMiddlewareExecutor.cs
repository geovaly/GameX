using RequestResponseFramework.Shared;

namespace RequestResponseFramework.Backend.MiddlewareExecutors
{
    public class HandleSystemExceptionMiddlewareExecutor : IMiddlewareExecutor
    {
        public async Task<Response<TResult>> ExecuteAsync<TRequest, TResult>(TRequest request, MiddlewareNextExecute<TRequest, TResult> nextExecute) where TRequest : Request<TResult> where TResult : RequestResult
        {
            try
            {
                return await nextExecute(request);
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
