using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{
    public record LoginResult(PlayerId PlayerId) : RequestResult
    {
    }

    public record AlreadyConnectedException : RequestException
    {
    }



    public record LoginCommand(DeviceId DeviceId) : Command<LoginResult>
    {
        public override void Accept(IRequestVisitor visitor) => visitor.Visit<LoginCommand, LoginResult>(this);
    }
}
