namespace RequestResponseFramework.Shared
{
    public interface IRequestExecutor
    {
        Task<Response> TryExecuteAsync(Request request);

        public Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request)
            where TResult : RequestResult;
    }


    public static class RequestExecutorExtensions
    {
        public static async Task<RequestResult> ExecuteAsync(this IRequestExecutor requestExecutor, Request request)
        {
            var result = await requestExecutor.TryExecuteAsync(request);
            return result.GetResult();
        }
        public static async Task<TResult> ExecuteAsync<TResult>(this IRequestExecutor requestExecutor, Request<TResult> request) where TResult : RequestResult
        {
            var result = await requestExecutor.TryExecuteAsync(request);
            return result.GetResult();
        }
    }

}
