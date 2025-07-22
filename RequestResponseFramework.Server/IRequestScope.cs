namespace RequestResponseFramework.Server;

public interface IRequestScope<TRequest, TResult> : IAsyncDisposable where TRequest : Request<TResult>
{
    TRequest Request { get; }

    IClientConnection? ClientConnection { get; }

    IRequestHandler<TRequest, TResult> RequestHandler { get; }

    IEnumerable<IMiddlewareExecutor> MiddlewareExecutors { get; }
}