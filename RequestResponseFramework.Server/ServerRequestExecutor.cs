using RequestResponseFramework.Shared;

namespace RequestResponseFramework.Server
{
    internal class ServerRequestExecutor(
        IRequestScopeFactory requestScopeFactory) : IServerRequestExecutor
    {

        public async Task<IResponse> TryExecuteAsync(IRequest request, IClientConnection? clientConnection = null)
        {
            var visitor = new RequestVisitor(requestScopeFactory, clientConnection);
            request.Accept(visitor);
            var response = await visitor.GetResponseTask();
            return response;
        }

        public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request, IClientConnection? clientConnection = null)
        {
            return (await TryExecuteAsync(request as IRequest, clientConnection) as Response<TResult>)!;
        }

        private class RequestVisitor(IRequestScopeFactory requestScopeFactory, IClientConnection? clientConnection) : IRequestVisitor
        {
            private Task<IResponse>? _responseTask;

            public Task<IResponse> GetResponseTask() =>
                _responseTask ?? throw new InvalidOperationException();

            public void Visit<TRequest, TResult>(TRequest request) where TRequest : Request<TResult>
            {
                _responseTask = HandleRequestAsync<TRequest, TResult>(request);
            }

            private async Task<IResponse> HandleRequestAsync<TRequest, TResult>(TRequest request) where TRequest : Request<TResult>
            {
                await using var requestScope = requestScopeFactory.Create<TRequest, TResult>(request, clientConnection);
                var requestHandler = requestScope.RequestHandler;
                var middlewareExecutors = requestScope.MiddlewareExecutors;

                MiddlewareNextTryExecuteAsync<TRequest, TResult> currentTryExecuteAsync = requestHandler.HandleAsync;

                foreach (var middlewareExecutor in middlewareExecutors.AsEnumerable().Reverse())
                {
                    var next = currentTryExecuteAsync;
                    currentTryExecuteAsync = (r) => middlewareExecutor.TryExecuteAsync(r, next);
                }

                var response = await currentTryExecuteAsync(request);
                return response;
            }

        }



    }
}

