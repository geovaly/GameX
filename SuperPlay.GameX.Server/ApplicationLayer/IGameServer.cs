using RequestResponseFramework.Server;

namespace SuperPlay.GameX.Server.ApplicationLayer;

public interface IGameServer : IServerRequestExecutor
{
    bool IsRunning { get; }
    Task StartAsync();
}