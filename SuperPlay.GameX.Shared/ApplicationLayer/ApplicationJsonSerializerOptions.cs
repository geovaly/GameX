using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Json;
using SuperPlay.GameX.Shared.DomainLayer;
using System.Text.Json;

namespace SuperPlay.GameX.Shared.ApplicationLayer
{
    public static class ApplicationJsonSerializerOptions
    {
        public static readonly IReadOnlyList<Type> RequestTypes = GetAllTypesSubclassOf<Request>().ToList();
        public static readonly IReadOnlyList<Type> RequestResultTypes = GetAllTypesSubclassOf<RequestResult>()
            .Concat(RequestResponseJsonSerializerOptions.RequestResultTypes).ToList();
        public static readonly IReadOnlyList<Type> RequestExceptionTypes = GetAllTypesSubclassOf<RequestException>()
            .Concat(RequestResponseJsonSerializerOptions.RequestExceptionTypes).ToList();


        public static readonly JsonSerializerOptions Options = new();

        static ApplicationJsonSerializerOptions()
        {
            Build(Options);
        }

        public static void Build(JsonSerializerOptions options)
        {
            DomainJsonSerializerOptions.Build(options);

            var polymorphicJsonConverterFactory = new PolymorphicJsonConverterFactory()
                .RegisterType<Request>(RequestTypes)
                .RegisterType<RequestResult>(RequestResultTypes)
                .RegisterType<RequestException>(RequestExceptionTypes)
                .RegisterType<ResponseData>(RequestResponseJsonSerializerOptions.ResponseDataTypeMap);

            options.Converters.Add(polymorphicJsonConverterFactory);
        }

        private static IEnumerable<Type> GetAllTypesSubclassOf<T>()
        {
            return typeof(ApplicationJsonSerializerOptions).Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T)));
        }
    }
}
