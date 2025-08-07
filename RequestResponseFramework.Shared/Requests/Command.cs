namespace RequestResponseFramework.Shared.Requests
{
    public abstract record Command<TResult> : Request<TResult>
    {
    }

    public abstract record CommandBase<TCommand, TResult> : Command<TResult> where TCommand : CommandBase<TCommand, TResult>
    {
        public override void Accept(IRequestVisitor visitor) => visitor.Visit<TCommand, TResult>((TCommand)this);
    }
}
