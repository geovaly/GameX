namespace RequestResponseFramework.RequestExceptions
{
    public record BadRequestException(String Message) : RequestException
    {
    }
}