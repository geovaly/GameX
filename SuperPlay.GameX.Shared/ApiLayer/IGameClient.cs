using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Requests;

namespace SuperPlay.GameX.Shared.ApiLayer
{


    public interface IGameClient : IRequestExecutor, IAsyncDisposable
    {
        event EventHandler<Event>? EventsReceived;
        public Task StartAsync();
        bool IsRunning { get; }
    }

}
