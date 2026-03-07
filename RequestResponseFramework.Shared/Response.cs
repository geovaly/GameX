using RequestResponseFramework.Shared.SystemExceptions;
using System.Diagnostics;

namespace RequestResponseFramework.Shared
{
    public abstract record Response<T> : IResponse
    {
        public bool IsOk() => this is Ok<T>;

        public bool IsNotOk() => this is NotOk<T>;

        object IResponse.GetResult() => GetResult()!;

        public T GetResult()
        {
            return this switch
            {
                Ok<T> ok => ok.Result,
                NotOk<T> notOk => throw new RequestSystemException(notOk.Exception),
                _ => throw new UnreachableException()
            };
        }

        public RequestException GetException()
        {
            return this switch
            {
                NotOk<T> notOk => notOk.Exception,
                Ok<T> ok => throw new InvalidOperationException(),
                _ => throw new UnreachableException()
            };
        }
    }

}
