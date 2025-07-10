namespace RequestResponseFramework.Shared
{
    public abstract record Request
    {
        public abstract void Accept(IRequestVisitor visitor);
        public abstract Type GetResultType();
    }

    public abstract record Request<TResult> : Request where TResult : RequestResult
    {
        public override Type GetResultType() => typeof(TResult);

    }

}
