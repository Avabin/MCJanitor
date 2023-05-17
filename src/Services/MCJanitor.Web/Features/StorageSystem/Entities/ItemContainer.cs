using System.Collections.Immutable;
using MCJanitor.Web.Features.StorageSystem.ValueObjects;

namespace MCJanitor.Web.Features.StorageSystem.Entities;

[GenerateSerializer, Immutable]
public readonly record struct ItemContainer(
    [property: Id(0)] ItemContainerKey Id,
    [property: Id(1)] Coordinate Location,
    [property: Id(2)] ImmutableList<Slot> Slots) : IEntity<ItemContainerKey>
{
}

[GenerateSerializer, Immutable]
public readonly record struct ItemContainerKey([property: Id(0)]int ComputerId,[property: Id(1)] string PeripheralName) : IValueObject;