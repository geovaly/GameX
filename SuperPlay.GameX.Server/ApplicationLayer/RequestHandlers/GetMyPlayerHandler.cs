using RequestResponseFramework.Shared;
using RequestResponseFramework.Server;
using SuperPlay.GameX.Server.DomainLayer.Data;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;
using SuperPlay.GameX.Server.ApplicationLayer.UnitOfWork;

namespace SuperPlay.GameX.Server.ApplicationLayer.RequestHandlers
{
    internal class GetMyPlayerHandler(IUnitOfWork unitOfWork) : QueryHandler<GetMyPlayer, Player>
    {
        public override async Task<Response<Player>> HandleAsync(GetMyPlayer query)
        {
            var player = await unitOfWork.PlayerRepository.LoadMaybeAsync(query.Context.PlayerId);
            if (player == null)
            {
                return CreateNotOk(new PlayerNotFoundException(query.Context.PlayerId));
            }

            var playerData = ToPlayer(player);
            return CreateOk(playerData);
        }

        public static Player ToPlayer(MutablePlayer player)
        {
            return new Player(PlayerId: player.PlayerId, Coins: player.Coins, Rolls: player.Rolls);
        }


    }
}
