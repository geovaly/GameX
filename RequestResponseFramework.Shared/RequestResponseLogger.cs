namespace RequestResponseFramework.Shared
{

    public interface IRequestResponseLogger
    {
        void Debug(string message, params object[] args);
        void Information(string message, params object[] args);
        void Warning(string message, params object[] args);
        void Error(string message, params object[] args);
        void Error(Exception ex, string message, params object[] args);
        void Fatal(string message, params object[] args);
        void Fatal(Exception ex, string message, params object[] args);
    }

}
