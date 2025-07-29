using RequestResponseFramework;
using RequestResponseFramework.Client;
using RequestResponseFramework.Client.WebSockets;

namespace SuperPlay.GameX.Frontend.GameClient.ApiLayer;

public class WebSocketGameClient(WebSocketRequestClient client) : IGameClient
{

    public bool IsRunning => client.IsRunning;
    public ValueTask DisposeAsync() => client.DisposeAsync();

    public void SetClientRequestExecutor(IClientRequestExecutor clientRequestExecutor)
        => client.SetClientRequestExecutor(clientRequestExecutor);

    public Task StartAsync() => client.StartAsync();

    public Task<IResponse> TryExecuteAsync(IRequest request) => client.TryExecuteAsync(request);

    public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request)
        => (await TryExecuteAsync(request as IRequest) as Response<TResult>)!;

}