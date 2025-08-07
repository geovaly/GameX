using RequestResponseFramework.Shared;
using RequestResponseFramework.Server;

namespace SuperPlay.GameX.Server.App.Tests.Shared
{

    public class ClientConnection : IClientConnection
    {
        private readonly List<IRequest> _receivedRequests = new();
        public IReadOnlyList<IRequest> ReceivedRequests => _receivedRequests;

        public event ConnectionRemovedHandler? ConnectionRemoved;

        public void SendClientRequest(IRequest request)
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
