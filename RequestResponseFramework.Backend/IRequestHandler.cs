using RequestResponseFramework.Shared;

namespace RequestResponseFramework.Backend;

public interface IRequestHandler<in TRequest, TResult> where TRequest : Request<TResult> where TResult : RequestResult
{
    Task<Response<TResult>> HandleAsync(TRequest request);
}