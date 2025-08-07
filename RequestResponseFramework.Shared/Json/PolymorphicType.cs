using System.Reflection;

namespace RequestResponseFramework.Shared.Json;

public record PolymorphicType(Type BaseType, IReadOnlyCollection<Assembly> Assemblies);