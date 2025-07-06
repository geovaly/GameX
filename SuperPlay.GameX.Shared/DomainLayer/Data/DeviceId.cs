namespace SuperPlay.GameX.Shared.DomainLayer.Data
{
    public readonly record struct DeviceId(string Value)
    {
        public static DeviceId GenerateNew() => new(Guid.NewGuid().ToString());

        public override string ToString() => Value;
    }
}
