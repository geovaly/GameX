using RequestResponseFramework.Shared.SystemExceptions;
using System.Diagnostics;

namespace RequestResponseFramework.Shared
{
    public abstract record Response
    {
        public abstract bool IsOk { get; }
        public abstract bool IsNotOk { get; }
        public abstract RequestResult GetResult();
        public abstract RequestException GetException();

    }

    public abstract record Response<T> : Response where T : RequestResult
    {
        public override bool IsOk => this is Ok<T>;

        public override bool IsNotOk => this is NotOk<T>;

        public override T GetResult()
        {
            return this switch
            {
                Ok<T> ok => ok.Result,
                NotOk<T> notOk => throw new RequestSystemException(notOk.Exception),
                _ => throw new UnreachableException()
            };
        }

        public override RequestException GetException()
        {
            return this switch
            {
                NotOk<T> notOk => notOk.Exception,
                Ok<T> ok => throw new InvalidOperationException(),
                _ => throw new UnreachableException()
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
