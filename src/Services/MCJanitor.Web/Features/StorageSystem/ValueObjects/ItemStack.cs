namespace MCJanitor.Web.Features.StorageSystem.ValueObjects;

[GenerateSerializer, Immutable]
public readonly record struct ItemStack([property: Id(0)] int MaxStack,
    [property: Id(1)] string ItemId,
    [property: Id(2)] int CurrentStack) : IValueObject;