using RequestResponseFramework.Shared;

namespace RequestResponseFramework.Backend
{
    public delegate Task<Response<TResult>> MiddlewareNextExecute<in TRequest, TResult>(TRequest request) where TRequest : Request<TResult> where TResult : RequestResult;

    public interface IMiddlewareExecutor
    {
        Task<Response<TResult>> ExecuteAsync<TRequest, TResult>(TRequest request, MiddlewareNextExecute<TRequest, TResult> nextExecute) where TRequest : Request<TResult> where TResult : RequestResult;
    }
}
