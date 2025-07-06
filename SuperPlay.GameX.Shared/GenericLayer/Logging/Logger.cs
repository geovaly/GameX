namespace SuperPlay.GameX.Shared.GenericLayer.Logging
{
    public static class Log
    {
        public static ILogger Logger = new NullLogger();

        public static void Debug(string message, params object[] args) => Logger.Debug(message, args);
        public static void Information(string message, params object[] args) => Logger.Information(message, args);
        public static void Warning(string message, params object[] args) => Logger.Warning(message, args);
        public static void Error(string message, params object[] args) => Logger.Error(message, args);
        public static void Error(Exception ex, string message, params object[] args) => Logger.Error(ex, message, args);
        public static void Fatal(string message, params object[] args) => Logger.Fatal(message, args);
        public static void Fatal(Exception ex, string message, params object[] args) => Logger.Fatal(ex, message, args);
        public static ValueTask CloseAndFlushAsync() => Logger.CloseAndFlushAsync();
    }

    public interface ILogger
    {
        void Debug(string message, params object[] args);
        void Information(string message, params object[] args);
        void Warning(string message, params object[] args);
        void Error(string message, params object[] args);
        void Error(Exception ex, string message, params object[] args);
        void Fatal(string message, params object[] args);
        void Fatal(Exception ex, string message, params object[] args);
        ValueTask CloseAndFlushAsync();
    }

    public class NullLogger : ILogger
    {
        public void Debug(string message, params object[] args) { }
        public void Information(string message, params object[] args) { }
        public void Warning(string message, params object[] args) { }
        public void Error(string message, params object[] args) { }
        public void Error(Exception ex, string message, params object[] args) { }
        public void Fatal(string message, params object[] args) { }
        public void Fatal(Exception ex, string message, params object[] args) { }
        public ValueTask CloseAndFlushAsync() => ValueTask.CompletedTask;
    }
}
