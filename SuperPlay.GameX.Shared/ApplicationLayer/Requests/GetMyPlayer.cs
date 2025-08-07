using RequestResponseFramework.Shared.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{

    public record GetMyPlayer(LoggedInContext Context) : QueryBase<GetMyPlayer, Player>, ILoggedInRequest;
}