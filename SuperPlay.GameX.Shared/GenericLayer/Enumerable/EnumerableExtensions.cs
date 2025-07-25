namespace SuperPlay.GameX.Shared.GenericLayer.Enumerable
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var x in enumerable)
            {
                action(x);
            }
        }
    }
}
