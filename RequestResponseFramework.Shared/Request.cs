namespace RequestResponseFramework.Shared
{
    public abstract record Request<TResult> : IRequest
    {
        public abstract void Accept(IRequestVisitor visitor);

        public Type GetResultType() => typeof(TResult);

    }

}
