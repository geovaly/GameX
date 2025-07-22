namespace RequestResponseFramework.Server;

public interface IClientConnectionProvider
{
    IClientConnection? ClientConnection { get; }
}