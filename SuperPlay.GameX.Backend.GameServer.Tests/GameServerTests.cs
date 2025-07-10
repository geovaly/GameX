using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Backend.GameServer.Tests.Shared;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;

namespace SuperPlay.GameX.Backend.GameServer.Tests
{
    public class GameServerTests : GameServerTestsBase
    {

        [Fact]
        public async Task LoginCommand_ReturnOk()
        {
            var gameServer = await StartGameServer();

            var loginResponse = await gameServer.TryExecuteAsync(new LoginCommand(new DeviceId("device1")));

            Assert.IsType<Ok<LoginResult>>(loginResponse);
        }


        [Fact]
        public async Task LoginCommand_ReturnNotOkWithAlreadyConnectedCheckedException()
        {
            var gameServer = await StartGameServer();
            await gameServer.TryExecuteAsync(new LoginCommand(new DeviceId("device1")));

            var loginResponse = await gameServer.TryExecuteAsync(new LoginCommand(new DeviceId("device1")));

            Assert.IsType<NotOk<LoginResult>>(loginResponse);
            Assert.IsType<AlreadyConnectedException>(loginResponse.GetException());
        }


        [Fact]
        public async Task UpdateResourcesCommand_ReturnNotOkWithPlayerNotConnectedCheckedException()
        {
            var gameServer = await StartGameServer();
            var player1 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()))).PlayerId;
            var loggedInContext = new PlayerLoggedInContext(player1);
            await gameServer.ExecuteAsync(new LogoutCommand(loggedInContext));

            var updateResourcesResponse = await gameServer.TryExecuteAsync(new UpdateResourcesCommand(loggedInContext, ResourceType.Coin, new ResourceValue(1)));

            Assert.IsType<NotOk<UpdateResourcesResult>>(updateResourcesResponse);
            Assert.IsType<PlayerNotConnectedException>(updateResourcesResponse.GetException());
        }

        [Theory]
        [InlineData(ResourceType.Coin)]
        [InlineData(ResourceType.Roll)]
        public async Task UpdateResourcesCommand_AddResources(ResourceType resourceType)
        {
            var gameServer = await StartGameServer();
            var player1 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()))).PlayerId;
            var loggedInContext = new PlayerLoggedInContext(player1);

            await gameServer.ExecuteAsync(new UpdateResourcesCommand(loggedInContext, resourceType, new ResourceValue(1)));
            var updateResourcesResponse = await gameServer.TryExecuteAsync(new UpdateResourcesCommand(loggedInContext, resourceType, new ResourceValue(2)));

            Assert.IsType<Ok<UpdateResourcesResult>>(updateResourcesResponse);
            Assert.Equal(new ResourceValue(3), updateResourcesResponse.GetResult().NewResourceValue);
        }

        [Theory]
        [InlineData(ResourceType.Coin)]
        [InlineData(ResourceType.Roll)]
        public async Task UpdateResourcesCommand_RemoveResources(ResourceType resourceType)
        {
            var gameServer = await StartGameServer();
            var player1 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()))).PlayerId;
            var loggedInContext = new PlayerLoggedInContext(player1);

            await gameServer.ExecuteAsync(new UpdateResourcesCommand(loggedInContext, resourceType, new ResourceValue(3)));
            var updateResourcesResponse = await gameServer.TryExecuteAsync(new UpdateResourcesCommand(loggedInContext, resourceType, new ResourceValue(-2)));

            Assert.IsType<Ok<UpdateResourcesResult>>(updateResourcesResponse);
            Assert.Equal(new ResourceValue(1), updateResourcesResponse.GetResult().NewResourceValue);
        }

        [Theory]
        [InlineData(ResourceType.Coin)]
        [InlineData(ResourceType.Roll)]
        public async Task SendGiftCommand_ReturnOk(ResourceType resourceType)
        {
            var gameServer = await StartGameServer();
            var player1 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()))).PlayerId;
            var player2 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()))).PlayerId;
            var player1Context = new PlayerLoggedInContext(player1);
            var player2Context = new PlayerLoggedInContext(player2);
            await gameServer.ExecuteAsync(new UpdateResourcesCommand(player1Context, resourceType, new ResourceValue(10)));
            await gameServer.ExecuteAsync(new UpdateResourcesCommand(player2Context, resourceType, new ResourceValue(10)));

            var sendGiftResponse = await gameServer.TryExecuteAsync(new SendGiftCommand(player1Context, player2, resourceType, new ResourceValue(3)));

            Assert.IsType<Ok<SendGiftResult>>(sendGiftResponse);
            Assert.Equal(new ResourceValue(7), sendGiftResponse.GetResult().NewResourceValue);

            var playerData1 = await gameServer.ExecuteAsync(new GetMyPlayerQuery(player1Context));
            var playerData2 = await gameServer.ExecuteAsync(new GetMyPlayerQuery(player2Context));

            Assert.Equal(new ResourceValue(7), playerData1.PlayerData.GetResourceValue(resourceType));
            Assert.Equal(new ResourceValue(13), playerData2.PlayerData.GetResourceValue(resourceType));
        }


        [Theory]
        [InlineData(ResourceType.Coin)]
        [InlineData(ResourceType.Roll)]
        public async Task SendGiftCommand_ReturnNotEnoughResourcesCheckedException(ResourceType resourceType)
        {
            var gameServer = await StartGameServer();
            var player1 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()))).PlayerId;
            var player2 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()))).PlayerId;
            var player1Context = new PlayerLoggedInContext(player1);
            await gameServer.ExecuteAsync(new UpdateResourcesCommand(player1Context, resourceType, new ResourceValue(10)));

            var sendGiftResponse = await gameServer.TryExecuteAsync(new SendGiftCommand(player1Context, player2, resourceType, new ResourceValue(11)));

            Assert.IsType<NotOk<SendGiftResult>>(sendGiftResponse);
            Assert.IsType<NotEnoughResourcesException>(sendGiftResponse.GetException());
        }

        [Theory]
        [InlineData(ResourceType.Coin)]
        [InlineData(ResourceType.Roll)]
        public async Task SendGiftCommand_ReturnGenericUncheckedException_WhenSendingToHimself(ResourceType resourceType)
        {
            var gameServer = await StartGameServer();
            var player1 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()))).PlayerId;
            var player1Context = new PlayerLoggedInContext(player1);

            await gameServer.ExecuteAsync(new UpdateResourcesCommand(player1Context, resourceType, new ResourceValue(10)));

            var sendGiftResponse = await gameServer.TryExecuteAsync(new SendGiftCommand(player1Context, player1, resourceType, new ResourceValue(1)));
            Assert.IsType<NotOk<SendGiftResult>>(sendGiftResponse);
            Assert.IsType<GenericRequestException>(sendGiftResponse.GetException());
        }

        [Fact]
        public async Task SendGiftCommand_SendGiftEvent_IfFriendIsOnline()
        {
            var gameServer = await StartGameServer();
            var player1 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()))).PlayerId;
            var player2Connection = new ClientConnection();
            var player2 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()), player2Connection)).PlayerId;
            var player1Context = new PlayerLoggedInContext(player1);

            await gameServer.ExecuteAsync(new UpdateResourcesCommand(player1Context, ResourceType.Coin, new ResourceValue(2)));
            await gameServer.ExecuteAsync(new SendGiftCommand(player1Context, player2, ResourceType.Coin, new ResourceValue(1)));

            Assert.Contains(player2Connection.ReceivedRequests, x => x is GiftEvent);
            var giftEvent = (GiftEvent)player2Connection.ReceivedRequests.Single(x => x is GiftEvent);
            Assert.Equal(player1, giftEvent.SenderId);
            Assert.Equal(ResourceType.Coin, giftEvent.ResourceType);
            Assert.Equal(new ResourceValue(1), giftEvent.ResourceValue);
        }

        [Fact]
        public async Task SendGiftCommand_DoNotSendGiftEvent_IfFriendIsOffline__UsingRemoveConnection()
        {
            var gameServer = await StartGameServer();
            var player1 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()))).PlayerId;
            var player2Connection = new ClientConnection();
            var player2 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()), player2Connection)).PlayerId;
            var player1Context = new PlayerLoggedInContext(player1);
            await gameServer.ExecuteAsync(new UpdateResourcesCommand(player1Context, ResourceType.Coin, new ResourceValue(2)));

            player2Connection.RemoveConnection();
            await gameServer.ExecuteAsync(new SendGiftCommand(player1Context, player2, ResourceType.Coin, new ResourceValue(1)));

            Assert.DoesNotContain(player2Connection.ReceivedRequests, x => x is GiftEvent);
        }

        [Fact]
        public async Task SendGiftCommand_DoNotSendGiftEvent_IfFriendIsOffline__UsingLogoutCommand()
        {
            var gameServer = await StartGameServer();
            var player1 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()))).PlayerId;
            var player2Connection = new ClientConnection();
            var player2 = (await gameServer.ExecuteAsync(new LoginCommand(DeviceId.GenerateNew()), player2Connection)).PlayerId;
            var player1Context = new PlayerLoggedInContext(player1);
            var player2Context = new PlayerLoggedInContext(player2);
            await gameServer.ExecuteAsync(new UpdateResourcesCommand(player1Context, ResourceType.Coin, new ResourceValue(2)));

            await gameServer.ExecuteAsync(new LogoutCommand(player2Context), player2Connection);
            await gameServer.ExecuteAsync(new SendGiftCommand(player1Context, player2, ResourceType.Coin, new ResourceValue(1)));

            Assert.DoesNotContain(player2Connection.ReceivedRequests, x => x is GiftEvent);
        }
    }
}