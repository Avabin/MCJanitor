using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;

namespace MCJanitor.Web.Features.StorageSystem.ValueObjects;

[GenerateSerializer, Immutable]
public readonly record struct ItemStack([property: Id(0)] int Count,
    [property: Id(1)] string Name,
    [property: Id(2)] string? Nbt) : IValueObject
{
    public static ItemStack FromItemStackInfo(ItemStackInfo itemStackInfo) => 
        new(itemStackInfo.Count, itemStackInfo.Name, itemStackInfo.Nbt);
}