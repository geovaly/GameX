namespace RequestResponseFramework.Shared
{
    public abstract record RequestException
    {
    }

    public record GenericRequestException(string Message) : RequestException
    {
    }

}
