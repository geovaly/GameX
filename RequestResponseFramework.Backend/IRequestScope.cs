using RequestResponseFramework.Shared;

namespace RequestResponseFramework.Backend;

public interface IRequestScope<TRequest, TResult> : IAsyncDisposable where TRequest : Request<TResult> where TResult : RequestResult
{
    TRequest Request { get; }

    IRequestHandler<TRequest, TResult> RequestHandler { get; }

    IEnumerable<IMiddlewareExecutor> MiddlewareExecutors { get; }
}


public interface IRequestScopeFactory
{
    IRequestScope<TRequest, TResult> Create<TRequest, TResult>(TRequest request, IClientConnection? clientConnection) where TRequest : Request<TResult> where TResult : RequestResult;
}