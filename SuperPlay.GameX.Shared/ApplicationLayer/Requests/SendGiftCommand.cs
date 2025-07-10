using RequestResponseFramework.Shared;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{

    public record NotEnoughResourcesException() : RequestException;


    public record SendGiftResult(ResourceValue NewResourceValue) : RequestResult
    {
    }

    public record SendGiftCommand(LoggedInContext Context, PlayerId FriendPlayerId, ResourceType ResourceType, ResourceValue ResourceValue)
        : LoggedInCommand<SendGiftResult>(Context)
    {
        public override void Accept(IRequestVisitor visitor) => visitor.Visit<SendGiftCommand, SendGiftResult>(this);
    }
}