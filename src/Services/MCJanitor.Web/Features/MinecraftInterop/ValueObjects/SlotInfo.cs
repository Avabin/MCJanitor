namespace MCJanitor.Web.Features.MinecraftInterop.ValueObjects;

/// <summary>
/// Represents a Minecraft inventory slot.
/// </summary>
/// <param name="SlotIndex">Slot index.</param>
/// <param name="Item">Item stack in the slot.</param>
public readonly record struct SlotInfo(int SlotIndex, ItemStackInfo Item) : IValueObject;