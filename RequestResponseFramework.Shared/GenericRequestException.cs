namespace RequestResponseFramework.Shared;

public record GenericRequestException(string Message) : RequestException
{
}