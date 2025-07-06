using System.Text.Json;
using System.Text.Json.Serialization;

namespace SuperPlay.GameX.Shared.GenericLayer.Json
{
    public class StringWrapperConverter<T>(Func<string, T> factory, Func<T, string> extract) : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var primitive = reader.GetString() ?? throw new JsonException($"Expected string for {typeToConvert.Name}");
            return factory(primitive);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(extract(value));
        }
    }
}
