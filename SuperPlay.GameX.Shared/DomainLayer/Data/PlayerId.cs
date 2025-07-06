namespace SuperPlay.GameX.Shared.DomainLayer.Data
{
    public readonly record struct PlayerId(int Value)
    {
        public static readonly PlayerId Empty = new(0);

        public override string ToString() => Value.ToString();

    }
}
