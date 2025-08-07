namespace SuperPlay.GameX.Server.DomainLayer.UnitOfWork
{
    internal class UnitOfWorkConcurrencyException(Exception innerException) : Exception(null, innerException)
    {

    }
}
