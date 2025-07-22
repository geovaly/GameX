namespace RequestResponseFramework.Server
{
    public interface IServerRequestExecutor
    {
        Task<IResponse> TryExecuteAsync(IRequest request, IClientConnection? clientConnection = null);

        Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request, IClientConnection? clientConnection = null);
    }


    public static class ServerRequestExecutorExtensions
    {
        public static async Task<object> ExecuteAsync(this IServerRequestExecutor executor, IRequest request, IClientConnection? clientConnection = null)
        {
            var result = await executor.TryExecuteAsync(request, clientConnection);
            return result.GetResult();
        }
        public static async Task<TResult> ExecuteAsync<TResult>(this IServerRequestExecutor executor, Request<TResult> request, IClientConnection? clientConnection = null)
        {
            var result = await executor.TryExecuteAsync(request, clientConnection);
            return result.GetResult();
        }
    }
}
