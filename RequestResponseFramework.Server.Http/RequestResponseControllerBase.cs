using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RequestResponseFramework.Json;
using RequestResponseFramework.RequestExceptions;
using System.Net;
using System.Text.Json;

namespace RequestResponseFramework.Server.Http
{
    public class RequestResponseControllerBase
        : ControllerBase
    {
        private readonly IServerRequestExecutor _serverRequestExecutor;
        private readonly ILogger _logger;

        protected RequestResponseControllerBase(ILogger logger, IServerRequestExecutor serverRequestExecutor,
            IJsonSerializerOptionsProvider jsonSerializerOptionsProvider)
        {
            _logger = logger;
            _serverRequestExecutor = serverRequestExecutor;
            JsonSerializerOptions = jsonSerializerOptionsProvider.Options;
        }

        private JsonSerializerOptions JsonSerializerOptions { get; }

        protected async Task<ContentResult> BaseExecute(IRequest request)
        {
            _logger.LogInformation("[Server] Received Request: {RequestJson}", JsonSerializer.Serialize(request, JsonSerializerOptions));
            var response = await _serverRequestExecutor.TryExecuteAsync(request);
            var responseJson = request.ResponseToJson(response, JsonSerializerOptions);
            _logger.LogInformation("[Server] Sent Response: {ResponseJson}", responseJson);
            return new ContentResult
            {
                Content = responseJson,
                ContentType = "application/json",
                StatusCode = (int)MapStatusCode(response)
            };
        }
        protected Task<ContentResult> BaseExecute(JsonElement body)
        {
            var request = body.Deserialize<IRequest>(JsonSerializerOptions)!;
            return BaseExecute(request);
        }

        protected HttpStatusCode MapStatusCode(IResponse response)
        {
            if (response.IsOk()) return HttpStatusCode.OK;
            var exception = response.GetException();
            return exception switch
            {
                BadRequestException => HttpStatusCode.BadRequest,
                UnauthorizedException => HttpStatusCode.Unauthorized,
                ForbiddenException => HttpStatusCode.Forbidden,
                NotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };
        }

    }
}
