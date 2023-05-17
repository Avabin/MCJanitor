using System.Collections.Immutable;

namespace MCJanitor.Web.Features.StorageSystem.ValueObjects;

[GenerateSerializer, Immutable]
public readonly record struct DetailedItemInfo([property: Id(0)] ImmutableDictionary<string, string> Properties) : IValueObject;