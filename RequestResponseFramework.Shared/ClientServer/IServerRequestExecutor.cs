namespace RequestResponseFramework.Shared.ClientServer
{
    public interface IServerRequestExecutor
    {
        Task<Response> TryExecuteAsync(Request request, IClientConnection? clientConnection = null);

        Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request, IClientConnection? clientConnection = null) where TResult : RequestResult;
    }


    public static class ServerRequestExecutorExtensions
    {
        public static async Task<RequestResult> ExecuteAsync(this IServerRequestExecutor executor, Request request, IClientConnection? clientConnection = null)
        {
            var result = await executor.TryExecuteAsync(request, clientConnection);
            return result.GetResult();
        }
        public static async Task<TResult> ExecuteAsync<TResult>(this IServerRequestExecutor executor, Request<TResult> request, IClientConnection? clientConnection = null) where TResult : RequestResult
        {
            var result = await executor.TryExecuteAsync(request, clientConnection);
            return result.GetResult();
        }
    }
}
