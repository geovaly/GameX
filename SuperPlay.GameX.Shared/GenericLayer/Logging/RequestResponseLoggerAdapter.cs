using RequestResponseFramework.Shared;

namespace SuperPlay.GameX.Shared.GenericLayer.Logging
{
    public class RequestResponseLoggerAdapter(ILogger logger) : IRequestResponseLogger
    {
        public void Debug(string message, params object[] args) => logger.Debug(message, args);
        public void Information(string message, params object[] args) => logger.Information(message, args);
        public void Warning(string message, params object[] args) => logger.Warning(message, args);
        public void Error(string message, params object[] args) => logger.Error(message, args);
        public void Error(Exception ex, string message, params object[] args) => logger.Error(ex, message, args);
        public void Fatal(string message, params object[] args) => logger.Fatal(message, args);
        public void Fatal(Exception ex, string message, params object[] args) => logger.Fatal(ex, message, args);

    }
}
