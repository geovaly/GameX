namespace SuperPlay.GameX.Backend.GameServer.DomainLayer.UnitOfWork
{
    internal class UnitOfWorkConcurrencyException(Exception innerException) : Exception(null, innerException)
    {

    }
}
