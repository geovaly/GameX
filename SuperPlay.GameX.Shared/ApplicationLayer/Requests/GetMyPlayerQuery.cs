using RequestResponseFramework.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{

    public record GetMyPlayerQuery(LoggedInContext Context) : QueryBase<GetMyPlayerQuery, PlayerData>, ILoggedInRequest;
}