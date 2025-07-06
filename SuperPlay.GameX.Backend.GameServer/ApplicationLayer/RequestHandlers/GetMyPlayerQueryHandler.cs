using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.Data;
using SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Backend.GameServer.ApplicationLayer.RequestHandlers
{
    internal class GetMyPlayerQueryHandler(IUnitOfWork unitOfWork) : QueryHandler<GetMyPlayerQuery, GetMyPlayerResult>
    {
        public override async Task<Response<GetMyPlayerResult>> HandleAsync(GetMyPlayerQuery query)
        {
            var player = await unitOfWork.PlayerRepository.LoadMaybeAsync(query.Context.PlayerId);
            if (player == null)
            {
                return CreateNotOk(new PlayerNotFoundException(query.Context.PlayerId));
            }

            var playerData = ToPlayerData(player);
            return CreateOk(new GetMyPlayerResult(playerData));
        }

        public static PlayerData ToPlayerData(Player player)
        {
            return new PlayerData(PlayerId: player.PlayerId, Coins: player.Coins, Rolls: player.Rolls);
        }


    }
}
