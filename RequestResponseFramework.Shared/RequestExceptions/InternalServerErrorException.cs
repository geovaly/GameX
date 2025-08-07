namespace RequestResponseFramework.Shared.RequestExceptions
{
    public record InternalServerErrorException(Guid Guid) : RequestException
    {
    }
}
