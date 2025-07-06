using System.Text.Json;
using System.Text.Json.Serialization;

namespace SuperPlay.GameX.Shared.GenericLayer.Json
{
    public class IntWrapperConverter<T>(Func<int, T> factory, Func<T, int> extract) : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var primitive = reader.GetInt32();
            return factory(primitive);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(extract(value));
        }
    }
}
