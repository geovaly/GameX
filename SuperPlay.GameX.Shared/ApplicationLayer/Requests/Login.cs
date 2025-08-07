using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{

    public record AlreadyConnectedException : RequestException;

    public record Login(DeviceId DeviceId) : CommandBase<Login, PlayerId>;
}
