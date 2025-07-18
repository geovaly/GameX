using RequestResponseFramework;
using RequestResponseFramework.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{

    public record AlreadyConnectedException : RequestException;

    public record LoginCommand(DeviceId DeviceId) : CommandBase<LoginCommand, PlayerId>;
}
