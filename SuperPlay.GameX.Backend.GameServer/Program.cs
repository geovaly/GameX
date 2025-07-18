
using RequestResponseFramework.Server.WebSockets;
using Serilog;
using SuperPlay.GameX.Shared.GenericLayer.Disposable;

namespace SuperPlay.GameX.Backend.GameServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        await using var logging = InitLogging();
        var server = new CompositionRoot(new WebSocketsRequestServerSettings(UriPrefix: "http://localhost:5000/ws/")).GetWebSocketGameServer();
        await server.StartAsync();
    }

    private static IAsyncDisposable InitLogging()
    {
        Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Debug()
             .WriteTo.Console()
             .CreateLogger();
        return new DelegateAsyncDisposable(Log.CloseAndFlushAsync);
    }
}
