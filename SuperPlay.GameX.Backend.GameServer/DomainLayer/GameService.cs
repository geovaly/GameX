using RequestResponseFramework.Backend;
using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.ClientServer;
using SuperPlay.GameX.Shared.DomainLayer.Data;
using SuperPlay.GameX.Shared.GenericLayer.Logging;

namespace SuperPlay.GameX.Backend.GameServer.DomainLayer
{

    internal record OnlinePlayer(PlayerId PlayerId, IClientConnection? ClientConnection)
    {
        public bool IsConnectionMismatch(IClientConnectionProvider clientConnectionProvider)
        {
            if (clientConnectionProvider.ClientConnection == null) return false;
            return !ReferenceEquals(ClientConnection, clientConnectionProvider.ClientConnection);
        }
    }

    internal class GameService
    {
        private readonly Dictionary<PlayerId, OnlinePlayer> _onlinePlayersById = new();
        private readonly Dictionary<IClientConnection, OnlinePlayer> _onlinePlayersByConnection = new(ReferenceEqualityComparer.Instance);
        private readonly object _lock = new();

        public bool TryAddOnlinePlayer(OnlinePlayer player)
        {
            lock (_lock)
            {

                if (!_onlinePlayersById.TryAdd(player.PlayerId, player))
                {
                    return false;
                }

                if (player.ClientConnection != null)
                {
                    if (_onlinePlayersByConnection.TryGetValue(player.ClientConnection,
                            out var connectedAsDifferentPlayer))
                    {
                        RemoveOnlinePlayer(connectedAsDifferentPlayer.PlayerId);
                    }
                    _onlinePlayersByConnection.Add(player.ClientConnection, player);

                    player.ClientConnection.ConnectionRemoved += () => RemoveOnlinePlayer(player.PlayerId);
                }

                Log.Information("[Server] Player {PlayerId} Is Online", player.PlayerId);
                return true;

            }
        }

        public bool RemoveOnlinePlayer(PlayerId playerId)
        {
            lock (_lock)
            {
                if (!_onlinePlayersById.Remove(playerId, out var player)) return false;
                if (player.ClientConnection != null)
                {
                    _onlinePlayersByConnection.Remove(player.ClientConnection);
                }
                Log.Information("[Server] Player {PlayerId} Is Offline", playerId);
                return true;
            }
        }

        public OnlinePlayer? GetOnlinePlayer(PlayerId playerId)
        {
            lock (_lock)
            {
                return _onlinePlayersById.GetValueOrDefault(playerId);
            }
        }

        public void SendClientRequest(PlayerId playerId, Request request)
        {
            var onlinePlayer = GetOnlinePlayer(playerId);
            onlinePlayer?.ClientConnection?.SendClientRequest(request);
        }
    }
}
