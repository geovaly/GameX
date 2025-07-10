namespace RequestResponseFramework.Shared.SystemExceptions
{
    public class RequestSystemException(RequestException requestException) : Exception
    {
        public RequestException RequestException => requestException;
    }

}
