using RequestResponseFramework.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{
    public record Logout(LoggedInContext Context) : CommandBase<Logout, bool>;
}
