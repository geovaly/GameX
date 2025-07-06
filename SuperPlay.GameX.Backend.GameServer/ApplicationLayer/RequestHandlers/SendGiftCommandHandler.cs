using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Backend.GameServer.DomainLayer;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork.Repositories;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer.RequestHandlers
{

    internal class SendGiftCommandHandler(GameService gameService, IUnitOfWork unitOfWork) : CommandHandler<SendGiftCommand, SendGiftResult>
    {
        public override async Task<Response<SendGiftResult>> HandleAsync(SendGiftCommand command)
        {
            if (command.Context.PlayerId == command.FriendPlayerId)
            {
                return CreateNotOk(new GenericRequestException("You cannot send gift to yourself"));
            }

            if (command.ResourceValue <= 0)
            {
                return CreateNotOk(new GenericRequestException("Resource value should be greater than zero"));
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

            gameService.SendClientRequest(friendPlayer.PlayerId,
                new GiftEvent(player.PlayerId, friendPlayer.PlayerId, command.ResourceType, command.ResourceValue));

            return CreateOk(new SendGiftResult(player.GetResourceValue(command.ResourceType)));
        }
    }
}
