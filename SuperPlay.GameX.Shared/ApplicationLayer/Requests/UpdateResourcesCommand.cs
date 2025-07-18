using RequestResponseFramework.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{
    public record UpdateResourcesCommand(
        LoggedInContext Context,
        ResourceType ResourceType,
        ResourceValue DeltaResourceValue)
        : CommandBase<UpdateResourcesCommand, ResourceValue>, ILoggedInRequest;
}