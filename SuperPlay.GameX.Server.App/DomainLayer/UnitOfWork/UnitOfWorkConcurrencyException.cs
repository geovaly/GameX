namespace SuperPlay.GameX.Server.App.DomainLayer.UnitOfWork
{
    internal class UnitOfWorkConcurrencyException(Exception innerException) : Exception(null, innerException)
    {

    }
}
