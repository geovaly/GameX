namespace RequestResponseFramework.Shared.SystemExceptions
{
    public class NetworkSystemException(Exception? innerException = null, string? message = null) : Exception(message, innerException);
}
