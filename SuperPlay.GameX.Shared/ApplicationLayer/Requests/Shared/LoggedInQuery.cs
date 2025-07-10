using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Requests;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared
{
    public abstract record LoggedInQuery<TResult>(LoggedInContext Context) : Query<TResult>(), ILoggedInRequest where TResult : RequestResult
    {

    }
}
