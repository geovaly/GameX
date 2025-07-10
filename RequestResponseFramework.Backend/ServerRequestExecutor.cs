using RequestResponseFramework.Shared;

namespace RequestResponseFramework.Backend
{

    public class ServerRequestExecutor(
        IRequestScopeFactory requestScopeFactory, IEnumerable<Type> middlewareExecutorTypes) : IServerRequestExecutor
    {
        public IReadOnlyList<Type> MiddlewareExecutorTypes { get; } = middlewareExecutorTypes.ToList();

        public async Task<Response> TryExecuteAsync(Request request, IClientConnection? clientConnection = null)
        {
            var visitor = new RequestVisitor(requestScopeFactory, MiddlewareExecutorTypes, clientConnection);
            request.Accept(visitor);
            var result = await visitor.GetHandleRequestTask();
            return result;
        }

        public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request, IClientConnection? clientConnection = null) where TResult : RequestResult
        {
            return (await TryExecuteAsync(request as Request, clientConnection) as Response<TResult>)!;
        }

        private class RequestVisitor(IRequestScopeFactory requestScopeFactory, IReadOnlyList<Type> middlewareExecutorTypes, IClientConnection? clientConnection) : IRequestVisitor
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
                await using var requestScope = requestScopeFactory.Create<TRequest, TResult>(request);
                if (clientConnection != null)
                {
                    requestScope.SetClientConnection(clientConnection);
                }
                var requestHandler = requestScope.RequestHandler;
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
                var middlewareExecutorsByType = requestScope.MiddlewareExecutors.ToDictionary(x => x.GetType(), x => x);
                var middlewareExecutors = middlewareExecutorTypes.Select(x => middlewareExecutorsByType[x]).ToList();
                return middlewareExecutors;
            }
        }



    }
}

