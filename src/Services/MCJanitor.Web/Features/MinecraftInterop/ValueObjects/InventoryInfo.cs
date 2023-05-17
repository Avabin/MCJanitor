namespace MCJanitor.Web.Features.MinecraftInterop.ValueObjects;

/// <summary>
/// Represents a Minecraft inventory.
/// </summary>
/// <param name="Name">Peripheral name accessible from the computer.</param>
/// <param name="Size">Number of slots in the inventory.</param>
public readonly record struct InventoryInfo(string Name, int Size) : IValueObject;