using RequestResponseFramework.Shared;

namespace RequestResponseFramework.Backend;

public interface IRequestScope<TRequest, TResult> : IAsyncDisposable where TRequest : Request<TResult> where TResult : RequestResult
{
    TRequest Request { get; }

    IClientConnection? ClientConnection { get; }

    IRequestHandler<TRequest, TResult> RequestHandler { get; }

    IEnumerable<IMiddlewareExecutor> MiddlewareExecutors { get; }
}