using SuperPlay.GameX.Shared.DomainLayer.Data;
using SuperPlay.GameX.Shared.GenericLayer.Json;
using System.Text.Json;

namespace SuperPlay.GameX.Shared.DomainLayer.Json
{
    public class DomainJsonSerializerOptions
    {
        public static void Build(JsonSerializerOptions options)
        {
            options.Converters.Add(
                new IntWrapperConverter<PlayerId>(
                    value => new PlayerId(value),
                    wrapper => wrapper.Value
                )
            );

            options.Converters.Add(
                new StringWrapperConverter<DeviceId>(
                    value => new DeviceId(value),
                    wrapper => wrapper.Value
                )
            );

            options.Converters.Add(
                new IntWrapperConverter<ResourceValue>(
                    value => new ResourceValue(value),
                    wrapper => wrapper.Value
                )
            );
        }
    }
}
