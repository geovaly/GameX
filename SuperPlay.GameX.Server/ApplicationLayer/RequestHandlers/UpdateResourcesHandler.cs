using RequestResponseFramework.Shared;
using RequestResponseFramework.Server;
using SuperPlay.GameX.Backend.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Backend.DomainLayer.UnitOfWork.Repositories;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Backend.ApplicationLayer.RequestHandlers
{
    internal class UpdateResourcesHandler(IUnitOfWork unitOfWork) : CommandHandler<UpdateResources, ResourceValue>
    {
        public override async Task<Response<ResourceValue>> HandleAsync(UpdateResources command)
        {
            var player = await unitOfWork.PlayerRepository.LoadAsync(command.Context.PlayerId);
            player.UpdateResourceValue(command.ResourceType, command.DeltaResourceValue);
            await unitOfWork.SaveChangesAsync();
            return CreateOk(player.GetResourceValue(command.ResourceType));

        }
    }
}
