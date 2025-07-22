namespace RequestResponseFramework
{
    public interface IRequest
    {
        void Accept(IRequestVisitor visitor);
        Type GetResultType();
    }

    public abstract record Request<TResult> : IRequest
    {
        public abstract void Accept(IRequestVisitor visitor);

        public Type GetResultType() => typeof(TResult);

    }

}
