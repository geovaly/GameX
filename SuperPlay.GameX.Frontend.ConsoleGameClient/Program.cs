using RequestResponseFramework.Client.WebSockets;
using Serilog;
using SuperPlay.GameX.Frontend.ConsoleGameClient.ApplicationLayer;
using SuperPlay.GameX.Shared.GenericLayer.Disposable;


namespace SuperPlay.GameX.Frontend.ConsoleGameClient;

public class Program
{
    private static readonly Uri ServerUri = new("ws://localhost:5000/ws/");

    public static async Task Main()
    {
        await using var logging = InitLogging();
        var compositeRoot = new CompositionRoot(new WebSocketRequestClientSettings(ServerUri));
        await using var consoleGame = compositeRoot.GetConsoleGame();
        DisposeOnAppExiting(consoleGame);
        await consoleGame.RunAsync();
    }

    private static IAsyncDisposable InitLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
        return new DelegateAsyncDisposable(Log.CloseAndFlushAsync);
    }

    private static void DisposeOnAppExiting(ConsoleGame consoleGame)
    {
        Console.CancelKeyPress += (_, e) =>
        {
            OnAppExiting(consoleGame);
            e.Cancel = true;
        };


        AppDomain.CurrentDomain.ProcessExit += (_, _) => OnAppExiting(consoleGame);
    }

    private static void OnAppExiting(ConsoleGame consoleGame)
    {
        if (!consoleGame.IsRunning) return;
        Console.WriteLine("Exiting ...");
        consoleGame.DisposeAsync().GetAwaiter().GetResult();
    }


}