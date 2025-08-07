namespace RequestResponseFramework.Shared.RequestExceptions
{
    public record BadRequestException(string Message) : RequestException
    {
    }
}