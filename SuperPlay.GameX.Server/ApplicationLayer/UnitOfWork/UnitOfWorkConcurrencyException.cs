namespace SuperPlay.GameX.Server.ApplicationLayer.UnitOfWork
{
    internal class UnitOfWorkConcurrencyException(Exception innerException) : Exception(null, innerException)
    {

    }
}
