namespace RequestResponseFramework;

public record VoidResult
{
    public static readonly VoidResult Instance = new();

    private VoidResult() { }
}