namespace RequestResponseFramework.Shared;


public abstract record RequestResult
{

}

public record VoidResult : RequestResult
{
    public static readonly VoidResult Instance = new();

    private VoidResult() { }
}