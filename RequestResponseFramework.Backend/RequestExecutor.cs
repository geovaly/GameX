using RequestResponseFramework.Shared;

namespace RequestResponseFramework.Backend
{

    public class RequestExecutor(
        IRequestScopeFactory requestScopeFactory, IEnumerable<Type> middlewareExecutorTypes) : IRequestExecutor
    {
        public IReadOnlyList<Type> MiddlewareExecutorTypes { get; } = middlewareExecutorTypes.ToList();

        public async Task<Response> TryExecuteAsync(Request request, IRequestContext? context = null)
        {
            var visitor = new RequestVisitor(requestScopeFactory, MiddlewareExecutorTypes, context);
            request.Accept(visitor);
            var result = await visitor.GetHandleRequestTask();
            return result;
        }

        public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request, IRequestContext? context = null) where TResult : RequestResult
        {
            return (await TryExecuteAsync(request as Request, context) as Response<TResult>)!;
        }

        private class RequestVisitor(IRequestScopeFactory requestScopeFactory, IReadOnlyList<Type> middlewareExecutorTypes, IRequestContext? scope) : IRequestVisitor
        {
            private Task<Response>? _handleRequestTask;

            public Task<Response> GetHandleRequestTask() =>
                _handleRequestTask ?? throw new InvalidOperationException();

            public void Visit<TRequest, TResult>(TRequest request) where TRequest : Request<TResult> where TResult : RequestResult
            {
                _handleRequestTask = HandleRequestAsync<TRequest, TResult>(request);
            }

            private async Task<Response> HandleRequestAsync<TRequest, TResult>(TRequest request) where TRequest : Request<TResult> where TResult : RequestResult
            {
                await using var requestScope = requestScopeFactory.CreateRequestScope<TRequest, TResult>(request);
                if (scope is IClientConnection clientConnection)
                {
                    requestScope.SetClientConnection(clientConnection);
                }
                var requestHandler = requestScope.GetRequestHandler();
                var middlewareExecutors = GetOrderedMiddlewareExecutors(requestScope);

                MiddlewareNextTryExecuteAsync<TRequest, TResult> currentTryExecuteAsync = requestHandler.HandleAsync;

                foreach (var middlewareExecutor in middlewareExecutors.AsEnumerable().Reverse())
                {
                    var next = currentTryExecuteAsync;
                    currentTryExecuteAsync = (r) => middlewareExecutor.TryExecuteAsync(r, next);
                }

                var response = await currentTryExecuteAsync(request);
                return response;
            }

            private List<IMiddlewareExecutor> GetOrderedMiddlewareExecutors<TRequest, TResult>(IRequestScope<TRequest, TResult> requestScope) where TResult : RequestResult
                where TRequest : Request<TResult>
            {
                var middlewareExecutorsByType = requestScope.GetMiddlewareExecutors().ToDictionary(x => x.GetType(), x => x);
                var middlewareExecutors = middlewareExecutorTypes.Select(x => middlewareExecutorsByType[x]).ToList();
                return middlewareExecutors;
            }
        }



    }
}

