using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;

namespace MCJanitor.Web.Features.StorageSystem.ValueObjects;

[GenerateSerializer, Immutable]
public readonly record struct Slot(
    [property: Id(0)] int SlotIndex,
    [property: Id(1)] ItemStack ItemStack) : IValueObject
{
    public static Slot FromSlotInfo(SlotInfo slot) => 
        new(slot.SlotIndex, ItemStack.FromItemStackInfo(slot.Item));
}