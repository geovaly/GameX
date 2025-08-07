using RequestResponseFramework.Server;

namespace SuperPlay.GameX.Server.App.ApplicationLayer;

public interface IGameServer : IServerRequestExecutor
{
    bool IsRunning { get; }
    Task StartAsync();
}