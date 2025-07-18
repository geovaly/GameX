using RequestResponseFramework.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{
    public record GiftEvent(
        PlayerId SenderId,
        PlayerId ReceiverId,
        ResourceType ResourceType,
        ResourceValue ResourceValue) : EventBase<GiftEvent>;
}
