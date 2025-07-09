namespace RequestResponseFramework.Shared.ClientServer;

public delegate void ConnectionRemovedHandler();

public interface IClientConnection
{
    event ConnectionRemovedHandler ConnectionRemoved;
    void SendClientRequest(Request request);
}

public interface IClientConnectionProvider
{
    IClientConnection? ClientConnection { get; }
}


