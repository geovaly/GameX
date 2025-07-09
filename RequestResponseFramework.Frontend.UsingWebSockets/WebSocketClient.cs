using RequestResponseFramework.Shared;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace RequestResponseFramework.Frontend.UsingWebSockets;

public class WebSocketClient(
    IRequestExecutor clientRequestExecutor,
    IRequestResponseLogger logger,
    string uri,
    JsonSerializerOptions jsonSerializerOptions) : IRequestExecutor
{
    private const int BufferSize = 1024 * 4;
    private readonly byte[] _buffer = new byte[BufferSize];
    private readonly ClientWebSocket _client = new();
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly ConcurrentDictionary<string, WaitingRequest> _waitingRequests = new();

    public bool IsRunning { get; private set; }

    private class WaitingRequest(Request request, TaskCompletionSource<Response> taskCompletionSource)
    {
        public Request Request { get; } = request;
        public TaskCompletionSource<Response> TaskCompletionSource { get; } = taskCompletionSource;
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

    public async Task<Response> TryExecuteAsync(Request request)
    {
        var message = RequestResponseMessage.CreateRequest(request, jsonSerializerOptions);
        var messageJson = JsonSerializer.Serialize(message, jsonSerializerOptions);
        var requestBytes = Encoding.UTF8.GetBytes(messageJson);
        var waitingRequest = new WaitingRequest(request, new TaskCompletionSource<Response>(TaskCreationOptions.RunContinuationsAsynchronously));
        _waitingRequests.TryAdd(message.RequestId, waitingRequest);
        await SendAsync(_client, new ArraySegment<byte>(requestBytes), _sendLock);
        logger.Debug("[Client] Sent Request: {RequestJson}", JsonSerializer.Serialize(request, jsonSerializerOptions));
        var result = await waitingRequest.TaskCompletionSource.Task;
        return result;
    }

    public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request)
        where TResult : RequestResult
        => (await TryExecuteAsync(request as Request) as Response<TResult>)!;

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
                var message = JsonSerializer.Deserialize<RequestResponseMessage>(messageString, jsonSerializerOptions)!;

                switch (message.Type)
                {
                    case RequestResponseMessage.RequestType:
                        var clientRequest = JsonSerializer.Deserialize<Request>(message.Data, jsonSerializerOptions)!;
                        await clientRequestExecutor.ExecuteAsync(clientRequest);
                        var requestJson = JsonSerializer.Serialize(clientRequest, jsonSerializerOptions);
                        logger.Debug("[Client] Received Request: {RequestJson}", requestJson);
                        break;

                    case RequestResponseMessage.ResponseType:
                        var responseData =
                            JsonSerializer.Deserialize<ResponseData>(message.Data, jsonSerializerOptions)!;
                        if (!_waitingRequests.TryRemove(message.RequestId, out var waitingRequest))
                            throw new InvalidOperationException("No waiting request found for received response.");
                        var response = Response.FromData(waitingRequest.Request, responseData);
                        var responseJson = JsonSerializer.Serialize(response.ToData(), jsonSerializerOptions);
                        logger.Debug("[Client] Received Response: {ResponseJson}", responseJson);
                        waitingRequest.TaskCompletionSource.SetResult(response);
                        break;

                    default:
                        throw new InvalidOperationException($"Unexpected message type: {message.Type}");
                }
            }
            catch (NetworkSystemException ex)
            {
                SetExceptionForWaitingRequests(ex);
                logger.Error("[Client] NetworkSystemException Error");
                return;
            }
            catch (Exception ex)
            {
                SetExceptionForWaitingRequests(ex);
                logger.Error(ex, "[Client] ReceiveLoopAsync Error");
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

            logger.Information("[Client] Disconnected From Server");
        }
        catch (WebSocketException)
        {
        }
    }

    private async Task ConnectAsync()
    {

        try
        {
            await _client.ConnectAsync(new Uri(uri), CancellationToken.None);
            logger.Information("[Client] Connected To Server");
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
