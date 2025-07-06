using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Requests;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared
{
    public abstract record LoggedInCommand<TResult>(PlayerLoggedInContext Context) : Command<TResult>(), ILoggedInRequest where TResult : RequestResult
    {
    }
}
