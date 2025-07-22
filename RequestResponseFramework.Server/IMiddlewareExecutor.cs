namespace RequestResponseFramework.Server
{
    public delegate Task<Response<TResult>> MiddlewareNextTryExecuteAsync<in TRequest, TResult>(TRequest request) where TRequest : Request<TResult>;

    public interface IMiddlewareExecutor
    {
        Task<Response<TResult>> TryExecuteAsync<TRequest, TResult>(TRequest request, MiddlewareNextTryExecuteAsync<TRequest, TResult> nextTryExecuteAsync) where TRequest : Request<TResult>;
    }
}
