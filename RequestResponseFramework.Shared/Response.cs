namespace RequestResponseFramework.Shared
{


    public abstract record Response
    {
        public abstract bool GetIsOk();
        public abstract bool GetIsNotOk();
        public abstract RequestResult GetResult();
        public abstract RequestException GetException();

        public ResponseData ToData()
        {
            if (GetIsOk())
            {
                return new OkData(GetResult());
            }
            if (GetIsNotOk())
            {
                return new NotOkData(GetException());
            }

            throw new ArgumentException();
        }

        public static Response FromData(Request request, ResponseData data)
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
                    _ => throw new ArgumentException()
                };
            }
        }
    }

    public abstract record Response<T> : Response where T : RequestResult
    {
        public override bool GetIsOk() => this is Ok<T>;
        public override bool GetIsNotOk() => this is NotOk<T>;

        public override T GetResult()
        {
            return this switch
            {
                Ok<T> ok => ok.Result,
                NotOk<T> notOk => throw new RequestSystemException(notOk.Exception),
                _ => throw new InvalidOperationException()
            };
        }

        public override RequestException GetException()
        {
            return this switch
            {
                NotOk<T> ok => ok.Exception,
                _ => throw new InvalidOperationException()
            };
        }
    }

    public sealed record Ok<T>(T Result) : Response<T> where T : RequestResult
    {
    }

    public sealed record NotOk<T>(RequestException Exception) : Response<T> where T : RequestResult
    {
    }

}
