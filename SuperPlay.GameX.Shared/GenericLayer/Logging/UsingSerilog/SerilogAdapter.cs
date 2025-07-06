namespace SuperPlay.GameX.Shared.GenericLayer.Logging.UsingSerilog
{

    public class SerilogAdapter : ILogger
    {
        public void Debug(string message, params object[] args)
            => Serilog.Log.Debug(message, args);

        public void Information(string message, params object[] args)
            => Serilog.Log.Information(message, args);

        public void Warning(string message, params object[] args)
            => Serilog.Log.Warning(message, args);

        public void Error(string message, params object[] args)
            => Serilog.Log.Error(message, args);

        public void Error(Exception ex, string message, params object[] args)
            => Serilog.Log.Error(ex, message, args);

        public void Fatal(string message, params object[] args)
            => Serilog.Log.Fatal(message, args);

        public void Fatal(Exception ex, string message, params object[] args)
            => Serilog.Log.Fatal(ex, message, args);

        public ValueTask CloseAndFlushAsync()
            => Serilog.Log.CloseAndFlushAsync();
    }
}
