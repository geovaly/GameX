using RequestResponseFramework;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared
{
    public record PlayerNotConnectedException() : RequestException
    {
    }
}
