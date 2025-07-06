namespace RequestResponseFramework.Shared.Requests
{
    public abstract record Command<TResult>() : Request<TResult>() where TResult : RequestResult
    {
    }
}
