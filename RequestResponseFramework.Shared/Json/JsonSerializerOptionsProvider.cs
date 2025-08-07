using System.Text.Json;

namespace RequestResponseFramework.Shared.Json
{

    public interface IJsonSerializerOptionsProvider
    {
        JsonSerializerOptions Options { get; }
    }
}
