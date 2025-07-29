
using RequestResponseFramework.Server.WebSockets;
using Serilog;
using SuperPlay.GameX.Shared.GenericLayer.Disposable;

namespace SuperPlay.GameX.Backend.GameServer;

public class Program
{
    private const string UriPrefix = "http://localhost:5000/ws/";

    public static async Task Main(string[] args)
    {
        await using var logging = InitLogging();
        var compositeRoot = new CompositionRoot(new WebSocketRequestServerSettings(UriPrefix));
        var server = compositeRoot.GetWebSocketGameServer();
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
