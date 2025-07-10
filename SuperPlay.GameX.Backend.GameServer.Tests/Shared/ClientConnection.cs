using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;

namespace SuperPlay.GameX.Backend.GameServer.Tests.Shared
{

    public class ClientConnection : IClientConnection
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
            ConnectionRemoved = null;
        }
    }
}
