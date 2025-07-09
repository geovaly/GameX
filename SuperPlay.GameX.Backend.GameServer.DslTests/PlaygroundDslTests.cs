using SuperPlay.GameX.Backend.GameServer.DslTests.Base;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;
using Xunit;

namespace SuperPlay.GameX.Backend.GameServer.DslTests
{
    public class PlaygroundDslTests : GameServerDsl
    {
        [Fact]
        public async Task Playground1()
        {
            await GivenGameServer();
            var player1 = await GivenOldPlayer();
            var player2 = await GivenOldPlayer();
            var player3 = await GivenOldPlayer();

            await Login(player1);
            await Login(player2);
            await Login(player3);

            await UpdateResources(player1, ResourceType.Coin, 20);
            await UpdateResources(player1, ResourceType.Coin, -10);
            player1.ShouldHaveCoins(10);
            player2.ShouldHaveCoins(0);
            player3.ShouldHaveCoins(0);

            await Logout(player3);
            await SendGift(player1, player2, ResourceType.Coin, 1);
            await SendGift(player1, player3, ResourceType.Coin, 1);

            player2.ReceivedEventsLastShouldBe(new GiftEvent(
                SenderId: player1.PlayerId,
                ReceiverId: player2.PlayerId, ResourceType.Coin, 1));
            player3.ReceivedEventsShouldBeEmpty();

            await Login(player3);
            player1.ShouldHaveCoins(8);
            player2.ShouldHaveCoins(1);
            player3.ShouldHaveCoins(1);

            await Logout(player1);
            await Logout(player2);
            await Logout(player3);

            await SendGiftShouldThrow<PlayerNotConnectedException>(player1, player2, ResourceType.Coin, 1);

        }
    }
}
