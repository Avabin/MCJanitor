using System.Collections.Immutable;
using System.Text.RegularExpressions;
using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;
using MCJanitor.Web.Features.StorageSystem.ValueObjects;

namespace MCJanitor.Web.Features.StorageSystem.Entities;

[GenerateSerializer, Immutable]
public readonly record struct ItemContainer(
    [property: Id(0)] ItemContainerKey Id,
    [property: Id(1)] ImmutableList<Slot> Slots,
    [property: Id(2)] string? Nbt) : IEntity<ItemContainerKey>
{
    public static ItemContainer Empty => new(ItemContainerKey.Empty, ImmutableList<Slot>.Empty, null);
    public bool IsEmpty() => this == Empty;
    
    public static ItemContainer FromSlotEnumerable(ItemContainerKey key, IEnumerable<SlotInfo> slots)
    {
        var slotList = slots.Select(Slot.FromSlotInfo).ToImmutableList();
        return new ItemContainer(key, slotList, null);
    }
}

[GenerateSerializer, Immutable]
public readonly partial record struct ItemContainerKey([property: Id(0)] int ComputerId,
    [property: Id(1)] string PeripheralName) : IValueObject
{
    public static ItemContainerKey Of(int computerId, string peripheralName) => new(computerId, peripheralName);
    
    public static ItemContainerKey Empty => new(-1, string.Empty);
    
    public bool IsEmpty() => this == Empty;
    
    public override string ToString() => $"{ComputerId}:{PeripheralName}";
    
    public static implicit operator string(ItemContainerKey key) => key.ToString();
    
    public static implicit operator ItemContainerKey(string key) => Parse(key);
    
    public static implicit operator ItemContainerKey((int ComputerId, string PeripheralName) key) => Of(key.ComputerId, key.PeripheralName);
    
    public static implicit operator (int ComputerId, string PeripheralName)(ItemContainerKey key) => (key.ComputerId, key.PeripheralName);

    private static ItemContainerKey Parse(string key)
    {
        var match = _regex.Match(key);
        if (!match.Success)
        {
            throw new ArgumentException($"Key {key} is not in the correct format");
        }
        return Of(int.Parse(match.Groups[1].Value), match.Groups[2].Value);
    }
    
    private static Regex _regex = KeyRegex();

    [GeneratedRegex("^(\\d+):(.+)$")]
    private static partial Regex KeyRegex();
}