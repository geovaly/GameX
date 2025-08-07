using RequestResponseFramework.Shared;
using RequestResponseFramework.Client;

namespace SuperPlay.GameX.Client.GameClient.ApplicationLayer
{
    public interface IGameClient : IRequestExecutor, IAsyncDisposable
    {
        void SetClientRequestExecutor(IClientRequestExecutor clientRequestExecutor);
        public Task StartAsync();
        bool IsRunning { get; }
    }

}
