using RequestResponseFramework.Frontend.UsingWebSockets;
using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Requests;
using SuperPlay.GameX.Shared.ApiLayer;
using SuperPlay.GameX.Shared.ApplicationLayer;
using SuperPlay.GameX.Shared.GenericLayer.Logging;

namespace SuperPlay.GameX.Frontend.GameClient.ApiLayer.UsingWebSockets;

public class WebSocketGameClient : IGameClient
{
    private const string Uri = "ws://localhost:5000/ws/";

    private readonly WebSocketClient _client;

    public event EventHandler<Event>? EventsReceived;

    public WebSocketGameClient()
    {
        _client =
            new WebSocketClient(new ClientRequestExecutor(this),
                new RequestResponseLoggerAdapter(Log.Logger),
                uri: Uri,
                ApplicationJsonSerializerOptions.Options);
    }

    public bool IsRunning => _client.IsRunning;
    public ValueTask DisposeAsync() => _client.DisposeAsync();

    public Task StartAsync() => _client.StartAsync();

    public Task<Response> TryExecuteAsync(Request request) => _client.TryExecuteAsync(request);

    public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request)
        where TResult : RequestResult
        => (await TryExecuteAsync(request as Request) as Response<TResult>)!;

    private class ClientRequestExecutor(WebSocketGameClient client) : IRequestExecutor
    {
        public Task<Response> TryExecuteAsync(Request request)
        {
            if (request is Event e)
            {
                client.EventsReceived?.Invoke(client, e);
                return Task.FromResult<Response>(new Ok<VoidResult>(VoidResult.Instance));
            }
            else
            {
                throw new NotSupportedException("Only events are supported");
            }
        }

        public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request) where TResult : RequestResult
        {
            return (await TryExecuteAsync(request as Request) as Response<TResult>)!;
        }
    }
}


