namespace RequestResponseFramework.Shared;

public interface IRequestVisitor
{
    void Visit<TRequest, TResult>(TRequest request) where TRequest : Request<TResult> where TResult : RequestResult;
}