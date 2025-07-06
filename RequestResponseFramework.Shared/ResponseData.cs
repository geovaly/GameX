namespace RequestResponseFramework.Shared
{

    public abstract record ResponseData;

    public record OkData(RequestResult Result) : ResponseData
    {
    }

    public record NotOkData(RequestException Exception) : ResponseData
    {
    }
}
