using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Backend.GameServer.ApplicationLayer;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Data;
using Xunit;

namespace SuperPlay.GameX.Backend.GameServer.DslTests.Base
{
    public class GameServerDsl : IDisposable
    {
        private IGameServer? _gameServer;
        private readonly List<PlayerDsl> _loggedInPlayers = new();

        public async Task StartTheGameServer()
        {
            _gameServer = new CompositionRoot().GetGameServer();
            await _gameServer.StartAsync();
        }

        public async Task<PlayerDsl> CreateNewPlayer(DeviceId? deviceId = null, bool isFirstTime = false)
        {
            var player = new PlayerDsl { DeviceId = deviceId ?? DeviceId.GenerateNew() };

            if (!isFirstTime)
            {
                await Login(player);
                await Logout(player);
            }

            return player;
        }

        public async Task Login(PlayerDsl playerDsl)
        {
            _loggedInPlayers.Add(playerDsl);
            playerDsl.IsLoggedIn = true;
            var response = await ExecuteAsync(new LoginCommand(playerDsl.DeviceId), playerDsl.Connection);
            playerDsl.PlayerIdMaybe = response.PlayerId;
        }

        public async Task Logout(PlayerDsl playerDsl)
        {
            _loggedInPlayers.RemoveAll(x => x == playerDsl);
            playerDsl.IsLoggedIn = false;
            await ExecuteAsync(new LogoutCommand(playerDsl.GetContext()), playerDsl.Connection);
        }

        public async Task LoginShouldThrow<TRequestException>(PlayerDsl playerDsl) where TRequestException : RequestException
        {
            var result = await TryExecuteAsync(new LoginCommand(playerDsl.DeviceId), playerDsl.Connection);
            Assert.True(result.GetIsNotOk());
            Assert.IsType<TRequestException>(result.GetException());
        }


        public async Task UpdateResources(PlayerDsl playerDsl, ResourceType resourceType, ResourceValue deltaValue)
        {
            await ExecuteAsync(new UpdateResourcesCommand(playerDsl.GetContext(), resourceType, deltaValue), playerDsl.Connection);
        }

        public async Task UpdateResourcesShouldThrow<TRequestException>(PlayerDsl playerDsl, ResourceType resourceType, ResourceValue deltaValue) where TRequestException : RequestException
        {
            var result = await TryExecuteAsync(new UpdateResourcesCommand(playerDsl.GetContext(), resourceType, deltaValue), playerDsl.Connection);
            Assert.True(result.GetIsNotOk());
            Assert.IsType<TRequestException>(result.GetException());
        }

        public async Task SendGift(PlayerDsl playerDsl, PlayerDsl friend, ResourceType resourceType, ResourceValue value)
        {
            await ExecuteAsync(new SendGiftCommand(playerDsl.GetContext(), friend.PlayerIdMaybe!.Value, resourceType, value), playerDsl.Connection);
        }

        public async Task SendGiftShouldThrow<TRequestException>(PlayerDsl playerDsl, PlayerDsl friend, ResourceType resourceType, ResourceValue value) where TRequestException : RequestException
        {
            var result = await TryExecuteAsync(new SendGiftCommand(playerDsl.GetContext(), friend.PlayerIdMaybe!.Value, resourceType, value), playerDsl.Connection);
            Assert.True(result.GetIsNotOk());
            Assert.IsType<TRequestException>(result.GetException());
        }

        private async Task RefreshPlayer(PlayerDsl playerDsl)
        {
            if (!playerDsl.IsLoggedIn) return;
            if (!playerDsl.PlayerIdMaybe.HasValue) return;
            var response = await _gameServer!.ExecuteAsync(new GetMyPlayerQuery(playerDsl.GetContext()));
            playerDsl.Coins = response.PlayerData.Coins;
            playerDsl.Rolls = response.PlayerData.Rolls;
        }

        private async Task RefreshPlayers()
        {
            foreach (var player in _loggedInPlayers)
            {
                await RefreshPlayer(player);
            }
        }

        private async Task<TResult> ExecuteAsync<TResult>(Request<TResult> request, IClientConnection clientConnection) where TResult : RequestResult
        {
            var response = await _gameServer!.ExecuteAsync(request, clientConnection);
            await RefreshPlayers();
            return response;
        }

        private async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request, IClientConnection clientConnection) where TResult : RequestResult
        {
            var response = await _gameServer!.TryExecuteAsync(request, clientConnection);
            await RefreshPlayers();
            return response;
        }

        public void Dispose()
        {
        }


    }
}
