using RequestResponseFramework.Shared;
using Serilog;
using SuperPlay.GameX.Frontend.GameClient.ApiLayer.UsingWebSockets;
using SuperPlay.GameX.Shared.GenericLayer.Disposable;
using SuperPlay.GameX.Shared.GenericLayer.Logging.UsingSerilog;
using Log = SuperPlay.GameX.Shared.GenericLayer.Logging.Log;

namespace SuperPlay.GameX.Frontend.ConsoleGameClient;

public class Program
{
    public static async Task Main()
    {
        await using var logging = InitLogging();
        await using var client = new WebSocketGameClient();
        if (!await TryStartAsync(client)) return;
        DisposeOnAppExiting(client);
        await new GameProgram(client).Run();
    }

    private static async Task<bool> TryStartAsync(WebSocketGameClient client)
    {
        try
        {
            await client.StartAsync();
            return true;
        }
        catch (NetworkSystemException)
        {
            Console.WriteLine("[Client] Cannot connect to server. Press any key to exit.");
            Console.ReadKey();
            return false;
        }
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

    private static void DisposeOnAppExiting(WebSocketGameClient client)
    {
        Console.CancelKeyPress += (_, e) =>
        {
            OnAppExiting(client);
            e.Cancel = true;
        };


        AppDomain.CurrentDomain.ProcessExit += (_, _) => OnAppExiting(client);
    }

    private static void OnAppExiting(WebSocketGameClient client)
    {
        if (!client.IsRunning) return;
        Console.WriteLine("Exiting ...");
        client.DisposeAsync().GetAwaiter().GetResult();
    }


}