using RequestResponseFramework.Shared;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Shared.ApplicationLayer.Requests
{
    public record UpdateResourcesResult(ResourceValue NewResourceValue) : RequestResult
    {
    }

    public record UpdateResourcesCommand(LoggedInContext Context, ResourceType ResourceType, ResourceValue DeltaResourceValue)
        : LoggedInCommand<UpdateResourcesResult>(Context)
    {
        public override void Accept(IRequestVisitor visitor) => visitor.Visit<UpdateResourcesCommand, UpdateResourcesResult>(this);
    }
}