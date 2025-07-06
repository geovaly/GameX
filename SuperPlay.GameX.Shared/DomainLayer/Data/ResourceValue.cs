namespace SuperPlay.GameX.Shared.DomainLayer.Data
{
    public readonly record struct ResourceValue(int Value) : IComparable<ResourceValue>
    {
        public ResourceValue Update(ResourceValue delta)
        {
            var newValue = Value + delta.Value;
            if (newValue < 0)
            {
                newValue = 0;
            }

            return new ResourceValue(newValue);
        }

        public ResourceValue Inverse() => new(Value * -1);

        public static implicit operator ResourceValue(int value) => new(value);

        public static implicit operator int(ResourceValue resourceValue) => resourceValue.Value;

        public int CompareTo(ResourceValue other) => Value.CompareTo(other.Value);

        public static bool operator <(ResourceValue left, ResourceValue right)
            => left.CompareTo(right) < 0;

        public static bool operator >(ResourceValue left, ResourceValue right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(ResourceValue left, ResourceValue right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(ResourceValue left, ResourceValue right)
            => left.CompareTo(right) >= 0;

        public override string ToString() => Value.ToString();
    }
}
