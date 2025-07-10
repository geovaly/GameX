using RequestResponseFramework.Shared.Json;

namespace RequestResponseFramework.Shared
{
    public static class RequestResponseJsonSerializerOptions
    {
        public static readonly IReadOnlyDictionary<string, Type> ResponseDataTypeMap = new Dictionary<string, Type>
        {
            { "Ok" , typeof(OkData) },
            { "NotOk" , typeof(NotOkData) },
        };

        public static readonly IReadOnlyList<Type> RequestExceptionTypes = [typeof(GenericRequestException)];

        public static readonly IReadOnlyList<Type> RequestResultTypes = [typeof(VoidResult)];

    }
}