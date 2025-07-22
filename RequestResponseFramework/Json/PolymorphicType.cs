using System.Reflection;

namespace RequestResponseFramework.Json;

public record PolymorphicType(Type BaseType, IReadOnlyCollection<Assembly> Assemblies);