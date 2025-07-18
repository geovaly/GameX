using RequestResponseFramework;
using RequestResponseFramework.RequestExceptions;
using RequestResponseFramework.Server;
using SuperPlay.GameX.Backend.GameServer.DomainLayer;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.Repositories;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer.RequestHandlers
{

    internal class SendGiftCommandHandler(OnlinePlayerService onlinePlayerService, IUnitOfWork unitOfWork) : CommandHandler<SendGiftCommand, ResourceValue>
    {
        public override async Task<Response<ResourceValue>> HandleAsync(SendGiftCommand command)
        {
            if (command.Context.PlayerId == command.FriendPlayerId)
            {
                return CreateNotOk(new BadRequestException("You cannot send gift to yourself"));
            }

            if (command.ResourceValue <= 0)
            {
                return CreateNotOk(new BadRequestException("Resource value should be greater than zero"));
            }

            var player = await unitOfWork.PlayerRepository.LoadAsync(command.Context.PlayerId);

            var friendPlayer = await unitOfWork.PlayerRepository.LoadMaybeAsync(command.FriendPlayerId);
            if (friendPlayer == null)
            {
                return CreateNotOk(new PlayerNotFoundException(command.FriendPlayerId));
            }

            var currentResourceValue = player.GetResourceValue(command.ResourceType);
            if (currentResourceValue < command.ResourceValue)
            {
                return CreateNotOk(new NotEnoughResourcesException());
            }

            player.UpdateResourceValue(command.ResourceType, command.ResourceValue.Inverse());
            friendPlayer.UpdateResourceValue(command.ResourceType, command.ResourceValue);
            await unitOfWork.SaveChangesAsync();

            onlinePlayerService.SendClientRequest(friendPlayer.PlayerId,
                new GiftEvent(player.PlayerId, friendPlayer.PlayerId, command.ResourceType, command.ResourceValue));

            return CreateOk(player.GetResourceValue(command.ResourceType));
        }
    }
}
