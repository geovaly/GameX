namespace RequestResponseFramework;

public interface IRequestVisitor
{
    void Visit<TRequest, TResult>(TRequest request) where TRequest : Request<TResult>;
}