using Microsoft.Extensions.Logging;
using RequestResponseFramework.Json;
using RequestResponseFramework.SystemExceptions;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace RequestResponseFramework.Client.WebSockets;

public record WebSocketsRequestClientSettings(Uri ServerUri);


public class WebSocketsRequestClient(

    ILogger<WebSocketsRequestClient> logger,
    WebSocketsRequestClientSettings settings,
    IJsonSerializerOptionsProvider jsonSerializerOptionsProvider) : IRequestExecutor
{
    private const int BufferSize = 1024 * 4;
    private readonly byte[] _buffer = new byte[BufferSize];
    private readonly ClientWebSocket _client = new();
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly ConcurrentDictionary<string, WaitingRequest> _waitingRequests = new();
    private IClientRequestExecutor? _clientRequestExecutor;

    public void SetClientRequestExecutor(IClientRequestExecutor clientRequestExecutor)
    {
        _clientRequestExecutor = clientRequestExecutor;
    }

    private JsonSerializerOptions JsonSerializerOptions { get; } = jsonSerializerOptionsProvider.Options;


    public bool IsRunning { get; private set; }

    private class WaitingRequest(IRequest request, TaskCompletionSource<IResponse> taskCompletionSource)
    {
        public IRequest Request { get; } = request;
        public TaskCompletionSource<IResponse> TaskCompletionSource { get; } = taskCompletionSource;
    }

    public async ValueTask DisposeAsync()
    {
        if (!IsRunning) return;
        IsRunning = false;
        await CloseAsync(_client);
        _client.Dispose();
    }


    public async Task StartAsync()
    {
        if (IsRunning)
            throw new InvalidOperationException("Client is already running.");

        await ConnectAsync();
        IsRunning = true;
        _ = Task.Run(ReceiveLoopAsync);
    }

    public async Task<IResponse> TryExecuteAsync(IRequest request)
    {
        var message = RequestResponseMessage.CreateRequest(request, JsonSerializerOptions);
        var messageJson = JsonSerializer.Serialize(message, JsonSerializerOptions);
        var requestBytes = Encoding.UTF8.GetBytes(messageJson);
        var waitingRequest = new WaitingRequest(request, new TaskCompletionSource<IResponse>(TaskCreationOptions.RunContinuationsAsynchronously));
        _waitingRequests.TryAdd(message.RequestId, waitingRequest);
        await SendAsync(_client, new ArraySegment<byte>(requestBytes), _sendLock);
        logger.LogDebug("[Client] Sent Request: {RequestJson}", JsonSerializer.Serialize(request, JsonSerializerOptions));
        var result = await waitingRequest.TaskCompletionSource.Task;
        return result;
    }

    public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request)
        => (await TryExecuteAsync(request as IRequest) as Response<TResult>)!;

    private async Task ReceiveLoopAsync()
    {
        while (IsRunning && _client.State == WebSocketState.Open)
        {
            try
            {
                var receiveResult = await ReceiveAsync();
                if (receiveResult.MessageType == WebSocketMessageType.Close)
                    return;

                var messageString = Encoding.UTF8.GetString(_buffer, 0, receiveResult.Count);
                var message = JsonSerializer.Deserialize<RequestResponseMessage>(messageString, JsonSerializerOptions)!;

                switch (message.Type)
                {
                    case RequestResponseMessage.RequestType:
                        var clientRequest = JsonSerializer.Deserialize<IRequest>(message.Data, JsonSerializerOptions)!;
                        if (_clientRequestExecutor != null)
                        {
                            await _clientRequestExecutor.ExecuteAsync(clientRequest);
                        }
                        else
                        {
                            throw new InvalidOperationException("_clientRequestExecutor is null");
                        }
                        var requestJson = JsonSerializer.Serialize(clientRequest, JsonSerializerOptions);
                        logger.LogInformation("[Client] Received Request: {RequestJson}", requestJson);
                        break;

                    case RequestResponseMessage.ResponseType:
                        if (!_waitingRequests.TryRemove(message.RequestId, out var waitingRequest))
                            throw new InvalidOperationException("No waiting request found for received response.");
                        var response = waitingRequest.Request.ResponseFromJson(message.Data, JsonSerializerOptions);
                        logger.LogInformation("[Client] Received Response: {ResponseJson}", message.Data);
                        waitingRequest.TaskCompletionSource.SetResult(response);
                        break;

                    default:
                        throw new InvalidOperationException($"Unexpected message type: {message.Type}");
                }
            }
            catch (NetworkSystemException ex)
            {
                SetExceptionForWaitingRequests(ex);
                logger.LogError("[Client] NetworkSystemException Error");
                return;
            }
            catch (Exception ex)
            {
                SetExceptionForWaitingRequests(ex);
                logger.LogError(ex, "[Client] ReceiveLoopAsync Error");
                return;
            }
        }
    }

    private void SetExceptionForWaitingRequests(Exception ex)
    {
        var waitingRequests = _waitingRequests.Values.ToList();
        waitingRequests.Clear();
        foreach (var waitingRequest in waitingRequests)
        {
            waitingRequest.TaskCompletionSource.SetException(ex);
        }
    }

    private async Task CloseAsync(ClientWebSocket client)
    {
        try
        {
            if (client.State == WebSocketState.Open || client.State == WebSocketState.Connecting)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", CancellationToken.None);
            }

            logger.LogInformation("[Client] Disconnected From Server");
        }
        catch (WebSocketException)
        {
        }
    }

    private async Task ConnectAsync()
    {

        try
        {
            await _client.ConnectAsync(settings.ServerUri, CancellationToken.None);
            logger.LogInformation("[Client] Connected To Server");
        }
        catch (WebSocketException e)
        {
            if (IsNetworkSystemException(e))
            {
                throw new NetworkSystemException(e);
            }
            throw;
        }
    }
    private async Task<WebSocketReceiveResult> ReceiveAsync()
    {
        try
        {
            return await _client.ReceiveAsync(new ArraySegment<byte>(_buffer), CancellationToken.None);
        }
        catch (WebSocketException e)
        {
            if (IsNetworkSystemException(e))
            {
                throw new NetworkSystemException(e);
            }
            throw;
        }
    }

    private static async Task SendAsync(WebSocket webSocket, ArraySegment<byte> data, SemaphoreSlim sendLock)
    {
        await sendLock.WaitAsync();
        try
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(data, WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None);
            }
            else
            {
                throw new NetworkSystemException();
            }
        }
        catch (WebSocketException e)
        {
            if (IsNetworkSystemException(e))
            {
                throw new NetworkSystemException(e);
            }

            throw;
        }
        finally
        {
            sendLock.Release();
        }
    }

    private static bool IsNetworkSystemException(WebSocketException e)
    {
        return true;
    }
}
