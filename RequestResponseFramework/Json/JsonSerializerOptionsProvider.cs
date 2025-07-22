using System.Text.Json;

namespace RequestResponseFramework.Json
{

    public interface IJsonSerializerOptionsProvider
    {
        JsonSerializerOptions Options { get; }
    }
}
