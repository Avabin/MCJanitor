namespace MCJanitor.Web.Features.StorageSystem.ValueObjects;

[GenerateSerializer, Immutable]
public readonly record struct Slot(
    [property: Id(0)] int SlotCapacity,
    [property: Id(1)] ItemStack ItemStack) : IValueObject;