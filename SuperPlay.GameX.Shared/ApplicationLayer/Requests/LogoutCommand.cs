using RequestResponseFramework.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{
    public record LogoutCommand(LoggedInContext Context) : CommandBase<LogoutCommand, bool>;
}
