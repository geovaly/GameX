namespace RequestResponseFramework.Shared.Requests
{
    public abstract record Query<TResult> : Request<TResult>
    {
    }

    public abstract record QueryBase<TQuery, TResult> : Query<TResult> where TQuery : QueryBase<TQuery, TResult>
    {
        public override void Accept(IRequestVisitor visitor) => visitor.Visit<TQuery, TResult>((TQuery)this);
    }
}
