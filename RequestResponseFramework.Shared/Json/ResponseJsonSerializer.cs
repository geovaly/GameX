using System.Diagnostics;
using System.Text.Json;

namespace RequestResponseFramework.Shared.Json
{
    public static class ResponseJsonSerializer
    {
        public static IResponse ResponseFromJson(this IRequest request, string responseJson,
            JsonSerializerOptions jsonOptions)
        {
            var visitor = new ToResponseRequestVisitor(responseJson, jsonOptions);
            request.Accept(visitor);
            return visitor.Response!;
        }

        public static string ResponseToJson(this IRequest request, IResponse response,
            JsonSerializerOptions jsonOptions)
        {
            var visitor = new ToJsonRequestVisitor(response, jsonOptions);
            request.Accept(visitor);
            return visitor.Data!;
        }

        private class ToResponseRequestVisitor(string responseJson, JsonSerializerOptions options) : IRequestVisitor
        {
            public IResponse? Response { get; private set; }

            public void Visit<TRequest, TResult>(TRequest request) where TRequest : Request<TResult>
            {
                using JsonDocument doc = JsonDocument.Parse(responseJson);
                if (doc.RootElement.TryGetProperty("Result", out _))
                {
                    Response = JsonSerializer.Deserialize<Ok<TResult>>(responseJson, options);
                    return;
                }
                if (doc.RootElement.TryGetProperty("Exception", out _))
                {
                    Response = JsonSerializer.Deserialize<NotOk<TResult>>(responseJson, options);
                    return;
                }

                throw new UnreachableException();
            }
        }

        private class ToJsonRequestVisitor(IResponse data, JsonSerializerOptions options) : IRequestVisitor
        {
            public string? Data { get; private set; }

            public void Visit<TRequest, TResult>(TRequest request) where TRequest : Request<TResult>
            {
                Data = data switch
                {
                    NotOk<TResult> notOk => JsonSerializer.Serialize(notOk, options),
                    Ok<TResult> ok => JsonSerializer.Serialize(ok, options),
                    _ => throw new UnreachableException()
                };
            }
        }
    }


}
