namespace RequestResponseFramework.Shared
{
    public interface IRequestExecutor
    {
        Task<IResponse> TryExecuteAsync(IRequest request);

        public Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request);
    }


    public static class RequestExecutorExtensions
    {
        public static async Task<object> ExecuteAsync(this IRequestExecutor requestExecutor, IRequest request)
        {
            var result = await requestExecutor.TryExecuteAsync(request);
            return result.GetResult();
        }
        public static async Task<TResult> ExecuteAsync<TResult>(this IRequestExecutor requestExecutor, Request<TResult> request)
        {
            var result = await requestExecutor.TryExecuteAsync(request);
            return result.GetResult();
        }
    }

}
