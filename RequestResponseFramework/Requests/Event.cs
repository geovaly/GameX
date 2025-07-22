namespace RequestResponseFramework.Requests
{
    public abstract record Event : Request<VoidResult>
    {
    }

    public abstract record EventBase<TEvent> : Event where TEvent : EventBase<TEvent>
    {
        public override void Accept(IRequestVisitor visitor) => visitor.Visit<TEvent, VoidResult>((TEvent)this);
    }
}
