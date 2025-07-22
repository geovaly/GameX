namespace RequestResponseFramework.RequestExceptions
{
    public record InternalServerErrorException(Guid Guid) : RequestException
    {
    }
}
