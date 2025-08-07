using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Json;
using RequestResponseFramework.Shared.RequestExceptions;
using System.Net;
using System.Text.Json;

namespace RequestResponseFramework.Server.Http
{
    public class RequestControllerBase
        : ControllerBase
    {
        private readonly IServerRequestExecutor _serverRequestExecutor;
        private readonly ILogger _logger;

        protected RequestControllerBase(ILogger logger, IServerRequestExecutor serverRequestExecutor,
            IJsonSerializerOptionsProvider jsonSerializerOptionsProvider)
        {
            _logger = logger;
            _serverRequestExecutor = serverRequestExecutor;
            JsonSerializerOptions = jsonSerializerOptionsProvider.Options;
        }

        private JsonSerializerOptions JsonSerializerOptions { get; }

        protected async Task<ContentResult> Execute(IRequest request)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Execute Error");
                throw;
            }
        }

        protected Task<ContentResult> Execute(JsonElement body)
        {
            var request = body.Deserialize<IRequest>(JsonSerializerOptions)!;
            return Execute(request);
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
