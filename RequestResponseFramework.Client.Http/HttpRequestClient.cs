using Microsoft.Extensions.Logging;
using RequestResponseFramework.Json;
using System.Text;
using System.Text.Json;

namespace RequestResponseFramework.Client.Http;

public class HttpRequestClient(
    ILogger<HttpRequestClient> logger,
    HttpRequestClientSettings clientSettings,
    IJsonSerializerOptionsProvider jsonSerializerOptionsProvider) : IDisposable, IRequestExecutor
{
    private readonly HttpClient _httpClient = new() { BaseAddress = clientSettings.Uri };


    private JsonSerializerOptions JsonSerializerOptions { get; } = jsonSerializerOptionsProvider.Options;

    public async Task<IResponse> TryExecuteAsync(IRequest request)
    {
        var requestJson = JsonSerializer.Serialize(request, JsonSerializerOptions);
        logger.LogInformation("[Client] Sending Request: {RequestJson}", requestJson);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var responseMessage = await _httpClient.PostAsync("/Rpc", content);
        var responseJson = await responseMessage.Content.ReadAsStringAsync();
        var response = request.ResponseFromJson(responseJson, JsonSerializerOptions);
        logger.LogInformation("[Client] Received Response: {ResponseJson}", responseJson);
        return response;
    }

    public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request)
        => (await TryExecuteAsync(request as IRequest) as Response<TResult>)!;

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
