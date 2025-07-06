using SuperPlay.GameX.Backend.GameServer.DslTests.Base;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;
using Xunit;

namespace SuperPlay.GameX.Backend.GameServer.DslTests
{
    public class GameServerDslTests : GameServerDsl
    {
        [Fact]
        public async Task Login_IsOk()
        {
            await StartTheGameServer();
            var player = await CreateNewPlayer();
            await Login(player);
        }

        [Fact]
        public async Task CheckDefaultStateForNewPlayer()
        {
            await StartTheGameServer();
            var player = await CreateNewPlayer();
            await Login(player);
            player.ShouldHaveCoins(0);
            player.ShouldHaveRolls(0);
        }


        [Fact]
        public async Task Login_MakeSureThePlayerIsNotConnectedAlready()
        {
            await StartTheGameServer();
            var player1 = await CreateNewPlayer();
            await Login(player1);

            var player2 = await CreateNewPlayer(player1.DeviceId, isFirstTime: true);
            await LoginShouldThrow<AlreadyConnectedException>(player2);
        }

        [Fact]
        public async Task UpdateResources_MakeSureThePlayerIsConnected()
        {
            await StartTheGameServer();
            var player = await CreateNewPlayer();
            await UpdateResourcesShouldThrow<PlayerNotConnectedException>(player, ResourceType.Coin, 1);
        }

        [Fact]
        public async Task UpdateResources_IsOk()
        {
            await StartTheGameServer();
            var player = await CreateNewPlayer();
            await Login(player);
            await UpdateResources(player, ResourceType.Coin, 1);
            await UpdateResources(player, ResourceType.Roll, 2);

            player.ShouldHaveCoins(1);
            player.ShouldHaveRolls(2);

            await UpdateResources(player, ResourceType.Coin, -1);
            await UpdateResources(player, ResourceType.Roll, -1);

            player.ShouldHaveCoins(0);
            player.ShouldHaveRolls(1);
        }



        [Fact]
        public async Task SendGift_IsOk()
        {
            await StartTheGameServer();
            var player1 = await CreateNewPlayer();
            var player2 = await CreateNewPlayer();
            await Login(player1);
            await Login(player2);
            await UpdateResources(player1, ResourceType.Coin, 10);
            await SendGift(player1, player2, ResourceType.Coin, 3);

            player1.ShouldHaveCoins(7);
            player2.ShouldHaveCoins(3);
        }


        [Fact]
        public async Task SendGift_MakeSureThePlayerHasEnoughResources()
        {
            await StartTheGameServer();
            var player1 = await CreateNewPlayer();
            var player2 = await CreateNewPlayer();
            await Login(player1);
            await Login(player2);

            await UpdateResources(player1, ResourceType.Coin, 1);

            await SendGiftShouldThrow<NotEnoughResourcesException>(player1, player2, ResourceType.Coin, 2);

            player1.ShouldHaveCoins(1);
            player2.ShouldHaveCoins(0);
        }

        [Fact]
        public async Task SendGift_IfFriendIsOnlineThenSendAGiftEvent()
        {
            await StartTheGameServer();
            var player1 = await CreateNewPlayer();
            var player2 = await CreateNewPlayer();
            await Login(player1);
            await Login(player2);

            await UpdateResources(player1, ResourceType.Coin, 1);

            await SendGift(player1, player2, ResourceType.Coin, 1);

            player2.ReceivedEventsLastShouldBe(new GiftEvent(
                SenderId: player1.PlayerId,
                ReceiverId: player2.PlayerId, ResourceType.Coin, 1));
        }


        [Fact]
        public async Task SendGift_IfFriendIsOfflineThenDoNotSendAGiftEvent()
        {
            await StartTheGameServer();
            var player1 = await CreateNewPlayer();
            var player2 = await CreateNewPlayer();
            await Login(player1);

            await UpdateResources(player1, ResourceType.Coin, 1);

            await SendGift(player1, player2, ResourceType.Coin, 1);

            player2.ReceivedEventsShouldBeEmpty();
        }

    }
}