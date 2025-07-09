using RequestResponseFramework.Shared.ClientServer;

namespace RequestResponseFramework.Backend;

public interface IClientConnectionProvider
{
    IClientConnection? ClientConnection { get; }
}