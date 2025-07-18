using RequestResponseFramework.RequestExceptions;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared
{
    public record PlayerNotFoundException(PlayerId PlayerId) : NotFoundException()
    {
    }
}
