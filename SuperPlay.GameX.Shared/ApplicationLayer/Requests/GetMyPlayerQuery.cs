using RequestResponseFramework.Shared;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{
    public record GetMyPlayerResult(PlayerData PlayerData) : RequestResult
    {
    }

    public record GetMyPlayerQuery(PlayerLoggedInContext Context) : LoggedInQuery<GetMyPlayerResult>(Context)
    {
        public override void Accept(IRequestVisitor visitor) => visitor.Visit<GetMyPlayerQuery, GetMyPlayerResult>(this);
    }
}