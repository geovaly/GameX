using System.Text.Json;

namespace RequestResponseFramework.Json
{
    public record RequestResponseMessage(string Type, string Data, string RequestId)
    {
        public const string RequestType = "Request";
        public const string ResponseType = "Response";
        public static RequestResponseMessage CreateRequest(IRequest data, JsonSerializerOptions options) =>
            new(RequestType, JsonSerializer.Serialize(data, options), Guid.NewGuid().ToString());

        public static RequestResponseMessage CreateResponse(IResponse data, IRequest request, string requestId,
            JsonSerializerOptions options)
            => new(ResponseType, request.ResponseToJson(data, options), requestId);




    }
}
