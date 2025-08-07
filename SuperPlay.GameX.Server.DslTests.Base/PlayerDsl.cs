using SuperPlay.GameX.Backend.DslTests.Base;
using SuperPlay.GameX.Shared.ApplicationLayer.Requests.Shared;
using SuperPlay.GameX.Shared.DomainLayer.Data;
using Xunit;

namespace SuperPlay.GameX.Backend.DslTests.Base;

public class PlayerDsl
{
    internal PlayerDsl()
    {
        ReceivedRequests = new ReceivedRequestsDsl(Connection);
    }

    public bool IsLoggedIn { get; internal set; }

    internal ClientConnection Connection { get; } = new();

    public ReceivedRequestsDsl ReceivedRequests { get; }

    public PlayerId? PlayerIdMaybe { get; internal set; }
    public DeviceId DeviceId { get; internal init; }
    public ResourceValue Coins { get; internal set; }
    public ResourceValue Rolls { get; internal set; }

    public PlayerId PlayerId => PlayerIdMaybe!.Value;

    internal LoggedInContext GetContext()
    {
        return new LoggedInContext(PlayerIdMaybe!.Value);
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



    internal void RemoveConnection() => Connection.RemoveConnection();
}