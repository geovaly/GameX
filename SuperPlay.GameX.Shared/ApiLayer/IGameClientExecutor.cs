using RequestResponseFramework.Shared;

namespace SuperPlay.GameX.Shared.ApiLayer
{
    public interface IGameClientExecutor
    {
        Task<Response> TryExecuteAsync(Request request);
        Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request) where TResult : RequestResult;
    }

    public static class GameClientExecutorExtensions
    {
        public static async Task<RequestResult> ExecuteAsync(this IGameClientExecutor gameClientExecutor, Request request)
        {
            var result = await gameClientExecutor.TryExecuteAsync(request);
            return result.GetResult();
        }
        public static async Task<TResult> ExecuteAsync<TResult>(this IGameClientExecutor gameClientExecutor, Request<TResult> request) where TResult : RequestResult
        {
            var result = await gameClientExecutor.TryExecuteAsync(request);
            return result.GetResult();
        }
    }
}
