using System.Diagnostics;

namespace RequestResponseFramework.Shared
{

    public abstract record ResponseData
    {

        public static ResponseData FromResponse(Response response)
        {
            if (response.GetIsOk())
            {
                return new OkData(response.GetResult());
            }
            if (response.GetIsNotOk())
            {
                return new NotOkData(response.GetException());
            }

            throw new UnreachableException();
        }

        public static Response ToResponse(Request request, ResponseData data)
        {
            var visitor = new FromDataRequestVisitor(data);
            request.Accept(visitor);
            return visitor.Response!;
        }

        private class FromDataRequestVisitor(ResponseData data) : IRequestVisitor
        {
            public Response? Response { get; private set; }

            public void Visit<TRequest, TResult>(TRequest request) where TRequest : Request<TResult> where TResult : RequestResult
            {
                Response = data switch
                {
                    OkData okData => new Ok<TResult>((TResult)okData.Result),
                    NotOkData notOkData => new NotOk<TResult>(notOkData.Exception),
                    _ => throw new UnreachableException()
                };
            }
        }
    }

    public record OkData(RequestResult Result) : ResponseData
    {
    }

    public record NotOkData(RequestException Exception) : ResponseData
    {
    }
}
