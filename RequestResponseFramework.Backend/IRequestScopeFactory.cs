using RequestResponseFramework.Shared;

namespace RequestResponseFramework.Backend;

public interface IRequestScopeFactory
{
    IRequestScope<TRequest, TResult> Create<TRequest, TResult>(TRequest request, IClientConnection? clientConnection) where TRequest : Request<TResult> where TResult : RequestResult;
}