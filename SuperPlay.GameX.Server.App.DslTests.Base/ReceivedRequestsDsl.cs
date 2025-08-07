using RequestResponseFramework.Shared;
using SuperPlay.GameX.Server.App.DslTests.Base;
using Xunit;

namespace SuperPlay.GameX.Server.App.DslTests.Base;

public class ReceivedRequestsDsl
{
    private readonly ClientConnection _connection;

    internal ReceivedRequestsDsl(ClientConnection connection)
    {
        _connection = connection;
    }

    public void ShouldBeEmpty()
    {
        Assert.Empty(ConnectionReceivedRequests);
    }


    public void LastShouldBe<T>(T requestEvent)
    {
        Assert.NotEmpty(ConnectionReceivedRequests);
        var lastEvent = ConnectionReceivedRequests.Last();
        Assert.Equal<object>(requestEvent, lastEvent);
    }

    private IReadOnlyList<IRequest> ConnectionReceivedRequests => _connection.ReceivedRequests;

}