namespace RequestResponseFramework.Shared
{
    public interface IRequestExecutor
    {
        Task<Response> TryExecuteAsync(Request request, IRequestContext? context = null);

        public Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request, IRequestContext? context = null)
            where TResult : RequestResult;
    }


    public static class RequestExecutorExtensions
    {
        public static async Task<RequestResult> ExecuteAsync(this IRequestExecutor requestExecutor, Request request, IRequestContext? context = null)
        {
            var result = await requestExecutor.TryExecuteAsync(request, context);
            return result.GetResult();
        }
        public static async Task<TResult> ExecuteAsync<TResult>(this IRequestExecutor requestExecutor, Request<TResult> request, IRequestContext? context = null) where TResult : RequestResult
        {
            var result = await requestExecutor.TryExecuteAsync(request, context);
            return result.GetResult();
        }
    }

}
