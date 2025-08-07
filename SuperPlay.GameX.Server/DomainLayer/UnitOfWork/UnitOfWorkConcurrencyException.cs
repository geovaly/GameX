namespace SuperPlay.GameX.Backend.DomainLayer.UnitOfWork
{
    internal class UnitOfWorkConcurrencyException(Exception innerException) : Exception(null, innerException)
    {

    }
}
