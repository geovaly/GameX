using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;
using Xunit;

namespace SuperPlay.GameX.Backend.GameServer.DslTests.Base;

public class PlayerDsl
{
    public bool IsLoggedIn { get; internal set; }

    internal ClientConnection Connection { get; } = new();

    public IReadOnlyList<Request> ReceivedRequests => Connection.ReceivedRequests;

    public PlayerId? PlayerIdMaybe { get; internal set; }
    public DeviceId DeviceId { get; internal init; }
    public ResourceValue Coins { get; internal set; }
    public ResourceValue Rolls { get; internal set; }

    public PlayerId PlayerId => PlayerIdMaybe!.Value;

    internal PlayerLoggedInContext GetContext()
    {
        return new PlayerLoggedInContext(PlayerIdMaybe!.Value);
    }

    public void ShouldHaveCoins(ResourceValue value)
    {
        if (!IsLoggedIn)
        {
            throw new InvalidOperationException();
        }
        Assert.Equal(value, Coins);
    }

    public void ShouldHaveRolls(ResourceValue value)
    {
        if (!IsLoggedIn)
        {
            throw new InvalidOperationException();
        }

        Assert.Equal(value, Rolls);
    }

    public void ReceivedEventsShouldBeEmpty()
    {
        Assert.Empty(Connection.ReceivedRequests);
    }

    public void ReceivedEventsLastShouldBe<T>(T requestEvent)
    {
        Assert.NotEmpty(Connection.ReceivedRequests);
        var lastEvent = Connection.ReceivedRequests.Last();
        Assert.Equal<object>(requestEvent, lastEvent);
    }
}


internal class ClientConnection : IClientConnection
{
    private readonly List<Request> _receivedRequests = new();
    public IReadOnlyList<Request> ReceivedRequests => _receivedRequests;

    public event ConnectionRemovedHandler? ConnectionRemoved;

    public void SendClientRequest(Request request)
    {
        _receivedRequests.Add(request);
    }

    public void RemoveConnection()
    {
        ConnectionRemoved?.Invoke();
    }
}