namespace SuperPlay.GameX.Shared.GenericLayer.Disposable
{
    public class DelegateAsyncDisposable(Func<ValueTask> func) : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => func();
    }

    public class DelegateDisposable(Action action) : IDisposable
    {
        public void Dispose() => action();
    }
}
