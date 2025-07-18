using RequestResponseFramework;
using RequestResponseFramework.Client.WebSockets;
using RequestResponseFramework.Requests;
using SuperPlay.GameX.Shared.ApiLayer;

namespace SuperPlay.GameX.Frontend.GameClient.ApiLayer;

//private const string Uri = "ws://localhost:5000/ws/";

public class WebSocketsGameClient(WebSocketsRequestClient client) : IGameClient
{
    public event EventHandler<Event>? EventsReceived;

    public bool IsRunning => client.IsRunning;
    public ValueTask DisposeAsync() => client.DisposeAsync();

    public Task StartAsync() => client.StartAsync();

    public Task<IResponse> TryExecuteAsync(IRequest request) => client.TryExecuteAsync(request);

    public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request)
        => (await TryExecuteAsync(request as IRequest) as Response<TResult>)!;

    private class ClientRequestExecutor(WebSocketsGameClient client) : IRequestExecutor
    {
        public Task<IResponse> TryExecuteAsync(IRequest request)
        {
            if (request is Event e)
            {
                client.EventsReceived?.Invoke(client, e);
                return Task.FromResult<IResponse>(new Ok<VoidResult>(VoidResult.Instance));
            }
            else
            {
                throw new NotSupportedException("Only events are supported");
            }
        }

        public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request)
        {
            return (await TryExecuteAsync(request as IRequest) as Response<TResult>)!;
        }
    }
}


