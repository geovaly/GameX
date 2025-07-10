namespace RequestResponseFramework.Backend;

public interface IClientConnectionProvider
{
    IClientConnection? ClientConnection { get; }
}