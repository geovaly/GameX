using RequestResponseFramework;
using RequestResponseFramework.Server;
using SuperPlay.GameX.Backend.ApplicationLayer;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.DomainLayer.Data;
using Xunit;

namespace SuperPlay.GameX.Backend.DslTests.Base
{
    public class GameServerDsl : IDisposable
    {
        private IGameServer? _gameServer;
        private readonly List<PlayerDsl> _loggedInPlayers = new();

        public async Task GivenGameServer()
        {
            _gameServer = new CompositionRoot().GetGameServer();
            await _gameServer.StartAsync();
        }

        public Task<PlayerDsl> GivenNewPlayer(DeviceId? deviceId = null)
        {
            var player = new PlayerDsl { DeviceId = deviceId ?? DeviceId.GenerateNew() };
            return Task.FromResult(player);
        }
        public async Task<PlayerDsl> GivenOldPlayer(DeviceId? deviceId = null)
        {
            var player = await GivenNewPlayer(deviceId: deviceId);
            await Login(player);
            await Logout(player);
            return player;
        }


        public void RemoveConnection(PlayerDsl player)
        {
            _loggedInPlayers.RemoveAll(x => x == player);
            player.IsLoggedIn = false;
            player.RemoveConnection();
        }

        public async Task Login(PlayerDsl player)
        {
            _loggedInPlayers.Add(player);
            player.IsLoggedIn = true;
            player.PlayerIdMaybe = await ExecuteAsync(new Login(player.DeviceId), player.Connection);
        }

        public async Task Logout(PlayerDsl player)
        {
            _loggedInPlayers.RemoveAll(x => x == player);
            player.IsLoggedIn = false;
            await ExecuteAsync(new Logout(player.GetContext()), player.Connection);
        }

        public async Task LoginShouldThrow<TRequestException>(PlayerDsl player) where TRequestException : RequestException
        {
            var result = await TryExecuteAsync(new Login(player.DeviceId), player.Connection);
            Assert.True(result.IsNotOk());
            Assert.IsType<TRequestException>(result.GetException());
        }


        public async Task UpdateResources(PlayerDsl player, ResourceType resourceType, ResourceValue deltaValue)
        {
            await ExecuteAsync(new UpdateResources(player.GetContext(), resourceType, deltaValue), player.Connection);
        }

        public async Task UpdateResourcesShouldThrow<TRequestException>(PlayerDsl player, ResourceType resourceType, ResourceValue deltaValue) where TRequestException : RequestException
        {
            var result = await TryExecuteAsync(new UpdateResources(player.GetContext(), resourceType, deltaValue), player.Connection);
            Assert.True(result.IsNotOk());
            Assert.IsType<TRequestException>(result.GetException());
        }

        public async Task SendGift(PlayerDsl player, PlayerDsl friend, ResourceType resourceType, ResourceValue value)
        {
            await ExecuteAsync(new SendGift(player.GetContext(), friend.PlayerIdMaybe!.Value, resourceType, value), player.Connection);
        }

        public async Task SendGiftShouldThrow<TRequestException>(PlayerDsl player, PlayerDsl friend, ResourceType resourceType, ResourceValue value) where TRequestException : RequestException
        {
            var result = await TryExecuteAsync(new SendGift(player.GetContext(), friend.PlayerIdMaybe!.Value, resourceType, value), player.Connection);
            Assert.True(result.IsNotOk());
            Assert.IsType<TRequestException>(result.GetException());
        }

        private async Task RefreshPlayer(PlayerDsl player)
        {
            if (!player.IsLoggedIn) return;
            if (!player.PlayerIdMaybe.HasValue) return;
            var playerData = await _gameServer!.ExecuteAsync(new GetMyPlayer(player.GetContext()));
            player.Coins = playerData.Coins;
            player.Rolls = playerData.Rolls;
        }

        private async Task RefreshPlayers()
        {
            foreach (var player in _loggedInPlayers)
            {
                await RefreshPlayer(player);
            }
        }

        private async Task<TResult> ExecuteAsync<TResult>(Request<TResult> request, IClientConnection clientConnection)
        {
            var response = await _gameServer!.ExecuteAsync(request, clientConnection);
            await RefreshPlayers();
            return response;
        }

        private async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request, IClientConnection clientConnection)
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
