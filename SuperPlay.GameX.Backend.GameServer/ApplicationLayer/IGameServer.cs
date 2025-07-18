using RequestResponseFramework.Server;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer;

public interface IGameServer : IServerRequestExecutor
{
    bool IsRunning { get; }
    Task StartAsync();
}