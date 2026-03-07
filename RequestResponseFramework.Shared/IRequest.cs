namespace RequestResponseFramework.Shared
{
    public interface IRequest
    {
        void Accept(IRequestVisitor visitor);
        Type GetResultType();
    }

}
