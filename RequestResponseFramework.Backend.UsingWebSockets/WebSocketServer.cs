using RequestResponseFramework.Shared;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace RequestResponseFramework.Backend.UsingWebSockets;

public class WebSocketServer(
    IRequestResponseLogger logger,
    IApplicationRequestExecutor requestExecutor,
    JsonSerializerOptions jsonSerializerOptions,
    string prefix)
{
    private const int BufferSize = 1024 * 4;
    public bool IsRunning { get; private set; }

    public async Task StartAsync()
    {
        if (IsRunning)
            throw new InvalidOperationException("Server is already running.");

        IsRunning = true;
        var listener = new HttpListener();
        listener.Prefixes.Add(prefix);
        listener.Start();
        logger.Information("[Server] Started on {Prefix}", prefix);

        while (true)
        {
            var context = await GetContextAsync(listener);
            if (context.Request.IsWebSocketRequest)
            {
                _ = HandleWebSocketAsync(context);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private static async Task<HttpListenerContext> GetContextAsync(HttpListener listener)
    {
        try
        {
            return await listener.GetContextAsync();
        }
        catch (WebSocketException ex)
        {
            if (IsNetworkSystemException(ex))
            {
                throw new NetworkSystemException(ex);
            }

            throw;
        }
    }

    private async Task HandleWebSocketAsync(HttpListenerContext context)
    {
        logger.Information("[Server] Client Connected");

        WebSocket? webSocket = null;
        ClientConnection? clientConnection = null;
        var sendLock = new SemaphoreSlim(1, 1);

        try
        {
            var wsContext = await AcceptWebSocketAsync(context);
            webSocket = wsContext.WebSocket;
            var buffer = new byte[BufferSize];
            clientConnection = new ClientConnection(logger, webSocket, sendLock, jsonSerializerOptions);

            while (webSocket.State == WebSocketState.Open)
            {
                var receiveResult = await ReceiveAsync(webSocket, buffer);
                if (receiveResult.MessageType == WebSocketMessageType.Close)
                    break;

                var messageString = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                var message = JsonSerializer.Deserialize<RequestResponseMessage>(messageString, jsonSerializerOptions)!;

                switch (message.Type)
                {
                    case RequestResponseMessage.RequestType:
                        var request = JsonSerializer.Deserialize<Request>(message.Data, jsonSerializerOptions)!;
                        var requestJson = JsonSerializer.Serialize(request, jsonSerializerOptions);
                        logger.Debug("[Server] Received Request: {RequestJson}", requestJson);
                        var result = await requestExecutor.TryExecuteAsync(request, clientConnection);
                        var resultData = result.ToData();
                        var responseMsg = JsonSerializer.Serialize(
                            RequestResponseMessage.CreateResponse(resultData, message.RequestId, jsonSerializerOptions),
                            jsonSerializerOptions);
                        var responseBytes = Encoding.UTF8.GetBytes(responseMsg);
                        await SendAsync(webSocket, new ArraySegment<byte>(responseBytes), sendLock);
                        var responseJson = JsonSerializer.Serialize(resultData, jsonSerializerOptions);
                        logger.Debug("[Server] Sent Response: {RequestResultJson}", responseJson);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown message type: {message.Type}");
                }
            }

            await CloseAsync(webSocket);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "[Server] HandleWebSocketAsync Error");
        }
        finally
        {
            clientConnection?.RemoveConnection();
            webSocket?.Dispose();
            sendLock.Dispose();
            logger.Information("[Server] Client Disconnected");
        }
    }

    private static async Task CloseAsync(WebSocket webSocket)
    {
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
    }

    private static async Task<WebSocketReceiveResult> ReceiveAsync(WebSocket webSocket, byte[] buffer)
    {
        try
        {
            return await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        catch (WebSocketException ex)
        {
            if (IsNetworkSystemException(ex))
            {
                throw new NetworkSystemException(ex);
            }

            throw;
        }
    }

    private static async Task<HttpListenerWebSocketContext> AcceptWebSocketAsync(HttpListenerContext context)
    {
        try
        {
            return await context.AcceptWebSocketAsync(subProtocol: null);
        }
        catch (WebSocketException ex)
        {
            if (IsNetworkSystemException(ex))
            {
                throw new NetworkSystemException(ex);
            }

            throw;
        }

    }

    private class ClientConnection(IRequestResponseLogger logger, WebSocket webSocket, SemaphoreSlim sendLock, JsonSerializerOptions jsonSerializerOptions)
        : IClientConnection
    {
        public event ConnectionRemovedHandler? ConnectionRemoved;

        public void RemoveConnection()
        {
            ConnectionRemoved?.Invoke();
            ConnectionRemoved = null;
        }

        public void SendClientRequest(Request request)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                _ = SendClientRequestAsync(request);
            }
        }
        private async Task SendClientRequestAsync(Request request)
        {
            try
            {
                var msg = JsonSerializer.Serialize(
                    RequestResponseMessage.CreateRequest(request, jsonSerializerOptions),
                    jsonSerializerOptions);
                var msgBytes = Encoding.UTF8.GetBytes(msg);

                await SendAsync(webSocket, new ArraySegment<byte>(msgBytes), sendLock);

                var requestJson = JsonSerializer.Serialize(request, jsonSerializerOptions);
                logger.Debug("[Server] Sent Request: {RequestJson}", requestJson);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Server] SendClientRequestAsync Error");
            }
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
        }
        catch (WebSocketException ex)
        {
            if (IsNetworkSystemException(ex))
            {
                throw new NetworkSystemException(ex);
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
