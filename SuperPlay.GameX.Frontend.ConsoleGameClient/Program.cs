using RequestResponseFramework.Client.WebSockets;
using Serilog;
using SuperPlay.GameX.Shared.GenericLayer.Disposable;


namespace SuperPlay.GameX.Frontend.ConsoleGameClient;

public class Program
{
    private static readonly Uri ServerUri = new("ws://localhost:5000/ws/");

    public static async Task Main()
    {
        await using var logging = InitLogging();
        var compositeRoot = new CompositionRoot(new WebSocketsRequestClientSettings(ServerUri));
        await using var gameProgram = compositeRoot.GetGameProgram();
        DisposeOnAppExiting(gameProgram);
        await gameProgram.Run();
    }


    private static IAsyncDisposable InitLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
        return new DelegateAsyncDisposable(Log.CloseAndFlushAsync);
    }

    private static void DisposeOnAppExiting(GameProgram gameProgram)
    {
        Console.CancelKeyPress += (_, e) =>
        {
            OnAppExiting(gameProgram);
            e.Cancel = true;
        };


        AppDomain.CurrentDomain.ProcessExit += (_, _) => OnAppExiting(gameProgram);
    }

    private static void OnAppExiting(GameProgram gameProgram)
    {
        if (!gameProgram.IsRunning) return;
        Console.WriteLine("Exiting ...");
        gameProgram.DisposeAsync().GetAwaiter().GetResult();
    }


}