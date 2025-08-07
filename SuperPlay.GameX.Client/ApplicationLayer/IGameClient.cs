using RequestResponseFramework.Client;
using RequestResponseFramework.Shared;

namespace SuperPlay.GameX.Client.ApplicationLayer
{
    public interface IGameClient : IRequestExecutor, IAsyncDisposable
    {
        void SetClientRequestExecutor(IClientRequestExecutor clientRequestExecutor);
        public Task StartAsync();
        bool IsRunning { get; }
    }

}
