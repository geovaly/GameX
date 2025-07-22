namespace RequestResponseFramework.Server;

public interface IRequestHandler<in TRequest, TResult> where TRequest : Request<TResult>
{
    Task<Response<TResult>> HandleAsync(TRequest request);
}