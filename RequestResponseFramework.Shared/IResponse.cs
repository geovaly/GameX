namespace RequestResponseFramework.Shared
{
    public interface IResponse
    {
        bool IsOk();
        bool IsNotOk();
        object GetResult();
        RequestException GetException();
    }

    public sealed record Ok<T>(T Result) : Response<T>
    {
    }

    public sealed record NotOk<T>(RequestException Exception) : Response<T>
    {
    }

}
