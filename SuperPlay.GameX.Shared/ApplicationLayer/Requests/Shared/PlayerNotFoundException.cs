using RequestResponseFramework.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared
{
    public record PlayerNotFoundException(PlayerId PlayerId) : RequestException()
    {
    }
}
