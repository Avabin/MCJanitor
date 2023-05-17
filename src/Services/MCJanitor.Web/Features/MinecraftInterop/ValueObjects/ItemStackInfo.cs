using System.Collections.Immutable;

namespace MCJanitor.Web.Features.MinecraftInterop.ValueObjects;

/// <summary>
/// Represents an item stack in a slot.
/// </summary>
/// <param name="Count">Item count.</param>
/// <param name="Name">Item name (ID).</param>
/// <param name="Nbt">Optional NBT data hash.</param>
public readonly record struct ItemStackInfo(int Count, string Name, string? Nbt) : IValueObject;