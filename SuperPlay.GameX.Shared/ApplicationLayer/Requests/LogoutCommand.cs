using RequestResponseFramework.Shared;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{
    public record LogoutResult() : RequestResult
    {
    }
    public record LogoutCommand(PlayerLoggedInContext Context) : LoggedInCommand<LogoutResult>(Context)
    {
        public override void Accept(IRequestVisitor visitor) => visitor.Visit<LogoutCommand, LogoutResult>(this);
    }
}
