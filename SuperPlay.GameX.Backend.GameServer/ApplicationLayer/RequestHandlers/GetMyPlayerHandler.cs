using RequestResponseFramework;
using RequestResponseFramework.Server;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.Data;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer.RequestHandlers
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
