namespace RequestResponseFramework.Shared;

public record VoidResult : RequestResult
{
    public static readonly VoidResult Instance = new();

    private VoidResult() { }
}

public abstract record RequestResult
{

}