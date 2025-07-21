using RequestResponseFramework;
using RequestResponseFramework.Client;

namespace SuperPlay.GameX.Frontend.GameClient.ApiLayer
{
    public interface IGameClient : IRequestExecutor, IAsyncDisposable
    {
        void SetClientRequestExecutor(IClientRequestExecutor clientRequestExecutor);
        public Task StartAsync();
        bool IsRunning { get; }
    }

}
