namespace RequestResponseFramework.Shared.Requests
{
    public abstract record Query<TResult>() : Request<TResult>() where TResult : RequestResult
    {
    }
}
