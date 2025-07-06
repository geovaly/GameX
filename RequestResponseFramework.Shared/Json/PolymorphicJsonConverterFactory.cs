using System.Text.Json;
using System.Text.Json.Serialization;

namespace RequestResponseFramework.Shared.Json;


public class PolymorphicJsonConverterFactory : JsonConverterFactory
{
    const string DefaultDiscriminatorProperty = "$type";

    private readonly Dictionary<Type, TypeMapping> _typeMappings = new();

    public PolymorphicJsonConverterFactory RegisterType<TBase>(
        IReadOnlyCollection<Type> types,
        string discriminatorProperty = DefaultDiscriminatorProperty)
    {
        var typeMap = types.ToDictionary(x => x.Name, x => x);
        return RegisterType<TBase>(typeMap, discriminatorProperty);
    }
    public PolymorphicJsonConverterFactory RegisterType<TBase>(
        IReadOnlyDictionary<string, Type> typeMap,
        string discriminatorProperty = DefaultDiscriminatorProperty)
    {
        _typeMappings[typeof(TBase)] = new TypeMapping
        {
            TypeDiscriminator = discriminatorProperty,
            TypeMap = new Dictionary<string, Type>(typeMap),
            ReverseTypeMap = typeMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key)
        };
        return this;
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return _typeMappings.ContainsKey(typeToConvert);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var mapping = _typeMappings[typeToConvert];
        var converterType = typeof(PolymorphicJsonConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType, mapping)!;
    }

    private class TypeMapping
    {
        public required string TypeDiscriminator { get; init; }
        public required Dictionary<string, Type> TypeMap { get; init; }
        public required Dictionary<Type, string> ReverseTypeMap { get; init; }
    }


    private class PolymorphicJsonConverter<T>(TypeMapping mapping) : JsonConverter<T>
    {

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObject token");
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty(mapping.TypeDiscriminator, out var typeProperty))
            {
                throw new JsonException($"Missing type discriminator '{mapping.TypeDiscriminator}'");
            }

            var typeValue = typeProperty.GetString();
            if (string.IsNullOrEmpty(typeValue))
            {
                throw new JsonException($"Type discriminator '{mapping.TypeDiscriminator}' cannot be null or empty");
            }

            if (!mapping.TypeMap.TryGetValue(typeValue, out var targetType))
            {
                throw new JsonException($"Unknown type discriminator value: {typeValue}. Known types: {string.Join(", ", mapping.TypeMap.Keys)}");
            }

            var jsonWithoutDiscriminator = CreateJsonWithoutDiscriminator(root, mapping.TypeDiscriminator);
            return (T)JsonSerializer.Deserialize(jsonWithoutDiscriminator, targetType, options)!;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            var actualType = value.GetType();

            if (!mapping.ReverseTypeMap.TryGetValue(actualType, out var typeValue))
            {
                var registeredType = mapping.ReverseTypeMap.Keys.FirstOrDefault(t => t.IsAssignableFrom(actualType));
                if (registeredType != null)
                {
                    typeValue = mapping.ReverseTypeMap[registeredType];
                }
                else
                {
                    throw new JsonException($"Type {actualType.Name} is not registered. Registered types: {string.Join(", ", mapping.ReverseTypeMap.Keys.Select(t => t.Name))}");
                }
            }

            writer.WriteStartObject();
            writer.WriteString(mapping.TypeDiscriminator, typeValue);

            var json = JsonSerializer.Serialize(value, actualType, options);
            using var doc = JsonDocument.Parse(json);

            foreach (var property in doc.RootElement.EnumerateObject())
            {
                property.WriteTo(writer);
            }

            writer.WriteEndObject();
        }

        private static string CreateJsonWithoutDiscriminator(JsonElement element, string discriminatorName)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            writer.WriteStartObject();
            foreach (var property in element.EnumerateObject())
            {
                if (property.Name != discriminatorName)
                {
                    property.WriteTo(writer);
                }
            }
            writer.WriteEndObject();
            writer.Flush();

            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}



