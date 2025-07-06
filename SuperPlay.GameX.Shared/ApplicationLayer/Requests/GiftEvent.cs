using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{
    public record GiftEvent(PlayerId SenderId, PlayerId ReceiverId, ResourceType ResourceType, ResourceValue ResourceValue) : Event
    {
        public override void Accept(IRequestVisitor visitor) => visitor.Visit<GiftEvent, VoidResult>(this);
    }
}
