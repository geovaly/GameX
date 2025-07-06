namespace RequestResponseFramework.Shared
{
    public abstract record Request
    {
        public abstract void Accept(IRequestVisitor visitor);
        public abstract Type GetResultType();
    }

    public interface IRequestVisitor
    {
        void Visit<TRequest, TResult>(TRequest request) where TRequest : Request<TResult> where TResult : RequestResult;
    }

    public abstract record Request<TResult> : Request where TResult : RequestResult
    {

        public override Type GetResultType() => typeof(TResult);

    }

}
