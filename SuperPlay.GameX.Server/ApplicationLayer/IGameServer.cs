using RequestResponseFramework.Server;

namespace SuperPlay.GameX.Backend.ApplicationLayer;

public interface IGameServer : IServerRequestExecutor
{
    bool IsRunning { get; }
    Task StartAsync();
}