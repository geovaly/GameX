using RequestResponseFramework.Client.WebSockets;
using Serilog;
using SuperPlay.GameX.Client.ConsoleApp.UserInterfaceLayer;
using SuperPlay.GameX.Shared.GenericLayer.Disposable;

namespace SuperPlay.GameX.Client.ConsoleApp;

public class Program
{
    private static readonly Uri ServerUri = new("ws://localhost:5000/ws/");

    public static async Task Main()
    {
        await using var logging = InitLogging();
        var compositeRoot = new CompositionRoot(new WebSocketRequestClientSettings(ServerUri));
        await using var gameConsole = compositeRoot.GetGameConsole();
        DisposeOnAppExiting(gameConsole);
        await gameConsole.RunAsync();
    }

    private static IAsyncDisposable InitLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
        return new DelegateAsyncDisposable(Log.CloseAndFlushAsync);
    }

    private static void DisposeOnAppExiting(GameConsole consoleGame)
    {
        System.Console.CancelKeyPress += (_, e) =>
        {
            OnAppExiting(consoleGame);
            e.Cancel = true;
        };


        AppDomain.CurrentDomain.ProcessExit += (_, _) => OnAppExiting(consoleGame);
    }

    private static void OnAppExiting(GameConsole consoleGame)
    {
        if (!consoleGame.IsRunning) return;
        System.Console.WriteLine("Exiting ...");
        consoleGame.DisposeAsync().GetAwaiter().GetResult();
    }


}