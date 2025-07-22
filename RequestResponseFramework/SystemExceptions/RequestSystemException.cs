namespace RequestResponseFramework.SystemExceptions
{
    public class RequestSystemException(RequestException requestException) : Exception
    {
        public RequestException RequestException => requestException;
    }

}
