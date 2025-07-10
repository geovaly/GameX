using RequestResponseFramework.Shared;

namespace RequestResponseFramework.Backend
{

    public class ServerRequestExecutor(
        IRequestScopeFactory requestScopeFactory, IEnumerable<Type> orderedMiddlewareExecutorTypes) : IServerRequestExecutor
    {
        public IReadOnlyList<Type> OrderedMiddlewareExecutorTypes { get; } = orderedMiddlewareExecutorTypes.ToList();

        public async Task<Response> TryExecuteAsync(Request request, IClientConnection? clientConnection = null)
        {
            var visitor = new RequestVisitor(requestScopeFactory, OrderedMiddlewareExecutorTypes, clientConnection);
            request.Accept(visitor);
            var response = await visitor.GetResponseTask();
            return response;
        }

        public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request, IClientConnection? clientConnection = null) where TResult : RequestResult
        {
            return (await TryExecuteAsync(request as Request, clientConnection) as Response<TResult>)!;
        }

        private class RequestVisitor(IRequestScopeFactory requestScopeFactory, IReadOnlyList<Type> middlewareExecutorTypes, IClientConnection? clientConnection) : IRequestVisitor
        {
            private Task<Response>? _responseTask;

            public Task<Response> GetResponseTask() =>
                _responseTask ?? throw new InvalidOperationException();

            public void Visit<TRequest, TResult>(TRequest request) where TRequest : Request<TResult> where TResult : RequestResult
            {
                _responseTask = HandleRequestAsync<TRequest, TResult>(request);
            }

            private async Task<Response> HandleRequestAsync<TRequest, TResult>(TRequest request) where TRequest : Request<TResult> where TResult : RequestResult
            {
                await using var requestScope = requestScopeFactory.Create<TRequest, TResult>(request);
                if (clientConnection != null)
                {
                    requestScope.SetClientConnection(clientConnection);
                }
                var requestHandler = requestScope.RequestHandler;
                var middlewareExecutors = ComputeOrderedMiddlewareExecutors(requestScope);

                MiddlewareNextTryExecuteAsync<TRequest, TResult> currentTryExecuteAsync = requestHandler.HandleAsync;

                foreach (var middlewareExecutor in middlewareExecutors.AsEnumerable().Reverse())
                {
                    var next = currentTryExecuteAsync;
                    currentTryExecuteAsync = (r) => middlewareExecutor.TryExecuteAsync(r, next);
                }

                var response = await currentTryExecuteAsync(request);
                return response;
            }

            private List<IMiddlewareExecutor> ComputeOrderedMiddlewareExecutors<TRequest, TResult>(IRequestScope<TRequest, TResult> requestScope) where TResult : RequestResult
                where TRequest : Request<TResult>
            {
                var middlewareExecutorsByType = requestScope.MiddlewareExecutors.ToDictionary(x => x.GetType(), x => x);
                var middlewareExecutors = middlewareExecutorTypes.Select(x => middlewareExecutorsByType[x]).ToList();
                return middlewareExecutors;
            }
        }



    }
}

