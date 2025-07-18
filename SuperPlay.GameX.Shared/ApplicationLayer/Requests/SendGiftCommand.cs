using RequestResponseFramework;
using RequestResponseFramework.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{

    public record NotEnoughResourcesException() : RequestException;


    public record SendGiftCommand(
        LoggedInContext Context,
        PlayerId FriendPlayerId,
        ResourceType ResourceType,
        ResourceValue ResourceValue)
        : CommandBase<SendGiftCommand, ResourceValue>, ILoggedInRequest;
}