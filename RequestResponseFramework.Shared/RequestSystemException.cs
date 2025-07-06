namespace RequestResponseFramework.Shared
{
    public class RequestSystemException(RequestException requestException) : Exception
    {
        public RequestException RequestException => requestException;
    }

}
