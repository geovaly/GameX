namespace RequestResponseFramework.Shared;

public record VoidResult
{
    public static readonly VoidResult Instance = new();

    private VoidResult() { }
}