using RequestResponseFramework.Server.MiddlewareExecutors;
using SuperPlay.GameX.Server.App.DomainLayer.UnitOfWork.MiddlewareExecutors;

namespace SuperPlay.GameX.Server.App.ApplicationLayer.MiddlewareExecutors
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
