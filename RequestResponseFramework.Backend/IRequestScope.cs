using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.ClientServer;

namespace RequestResponseFramework.Backend;

public interface IRequestScope<TRequest, TResult> : IAsyncDisposable where TRequest : Request<TResult> where TResult : RequestResult
{
    TRequest Request { get; }

    void SetClientConnection(IClientConnection clientConnection);

    IRequestHandler<TRequest, TResult> RequestHandler { get; }

    IEnumerable<IMiddlewareExecutor> MiddlewareExecutors { get; }
}


public interface IRequestScopeFactory
{
    IRequestScope<TRequest, TResult> CreateRequestScope<TRequest, TResult>(TRequest request) where TRequest : Request<TResult> where TResult : RequestResult;
}