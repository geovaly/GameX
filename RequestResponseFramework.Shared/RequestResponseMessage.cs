using System.Text.Json;

namespace RequestResponseFramework.Shared
{
    public record RequestResponseMessage(string Type, string Data, string RequestId)
    {
        public const string RequestType = "Request";
        public const string ResponseType = "Response";
        public static RequestResponseMessage CreateRequest(Request data, JsonSerializerOptions options) =>
            new(RequestType, JsonSerializer.Serialize(data, options), Guid.NewGuid().ToString());

        public static RequestResponseMessage CreateResponse(ResponseData data, string requestId, JsonSerializerOptions options) =>
            new(ResponseType, JsonSerializer.Serialize(data, options), requestId);
    }
}
