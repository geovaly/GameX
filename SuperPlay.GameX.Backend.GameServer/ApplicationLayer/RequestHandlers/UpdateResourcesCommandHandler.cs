using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.Repositories;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer.RequestHandlers
{
    internal class UpdateResourcesCommandHandler(IUnitOfWork unitOfWork) : CommandHandler<UpdateResourcesCommand, UpdateResourcesResult>
    {
        public override async Task<Response<UpdateResourcesResult>> HandleAsync(UpdateResourcesCommand command)
        {
            var player = await unitOfWork.PlayerRepository.LoadAsync(command.Context.PlayerId);
            player.UpdateResourceValue(command.ResourceType, command.DeltaResourceValue);
            await unitOfWork.SaveChangesAsync();
            return CreateOk(new UpdateResourcesResult(player.GetResourceValue(command.ResourceType)));

        }
    }
}
