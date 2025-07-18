
using Serilog;
using SuperPlay.GameX.Shared.GenericLayer.Disposable;
using Log = SuperPlay.GameX.Shared.GenericLayer.Logging.Log;

namespace SuperPlay.GameX.Backend.GameServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        await using var logging = InitLogging();
        var server = new CompositionRoot().GetWebSocketGameServer();
        await server.StartAsync();
    }

    private static IAsyncDisposable InitLogging()
    {
        Log.Logger = new SerilogAdapter();
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
        return new DelegateAsyncDisposable(Log.CloseAndFlushAsync);
    }
}
