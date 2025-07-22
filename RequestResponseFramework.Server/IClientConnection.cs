namespace RequestResponseFramework.Server;

public delegate void ConnectionRemovedHandler();

public interface IClientConnection
{
    event ConnectionRemovedHandler ConnectionRemoved;
    void SendClientRequest(IRequest request);
}