using RequestResponseFramework.Server.MiddlewareExecutors;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.MiddlewareExecutors;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer.MiddlewareExecutors
{
    internal static class MiddlewareExecutorTypesProvider
    {
        internal static readonly IReadOnlyList<Type> OrderedTypes =
            [
                typeof(HandleSystemExceptionMiddlewareExecutor),
                typeof(EnsurePlayerIsLoggedInMiddlewareExecutor),
                typeof(RetryOnConcurrencyExceptionMiddlewareExecutor)
            ];
    }
}
