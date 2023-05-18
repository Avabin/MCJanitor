using System.Collections.Immutable;
using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;

namespace MCJanitor.Web.Features.StorageSystem.ValueObjects;

[GenerateSerializer, Immutable]
public readonly record struct DetailedItemInfo([property: Id(0)] ImmutableDictionary<string, string> Properties) : IValueObject
{
}