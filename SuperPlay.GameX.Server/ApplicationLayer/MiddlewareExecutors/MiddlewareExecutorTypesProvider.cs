using RequestResponseFramework.Server.MiddlewareExecutors;
using SuperPlay.GameX.Server.ApplicationLayer.UnitOfWork.MiddlewareExecutors;

namespace SuperPlay.GameX.Server.ApplicationLayer.MiddlewareExecutors
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
