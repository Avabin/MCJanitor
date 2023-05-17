using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;

namespace MCJanitor.Web.Features.Infrastructure.MinecraftInterop;

public interface IMinecraftComputer
{
    bool IsConnected { get; }
    int Id { get; }
    Task<string> PingAsync();
    Task<IReadOnlyList<InventoryInfo>> GetInventoriesAsync();
    
    Task<IReadOnlyList<SlotInfo>> GetInventoryAsync(string name);

    Task<SlotInfo?> GetSlotAsync(string inventoryName, int slotId);

    Task<int> PullItemsAsync(string source, string target, int fromSlot, int limit = 64, int? toSlot = null);
    Task<int> PushItemsAsync(string source, string target, int fromSlot, int limit = 64, int? toSlot = null);
}